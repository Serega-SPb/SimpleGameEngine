using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleGameEngine.Core.Behaviours;
using SimpleGameEngine.Core.SceneObjects;

namespace SimpleGameEngine.Core.Serialization
{
    public static class SerializeExtensions
    {
        # region Properties

        public struct SceneProperties
        {
            public const string Name = "Name";
            public const string Description = "Description";
            public const string Size = "Size";
            public const string Background = "Background";
            public const string Objects = "Objects";
        }

        public struct SceneObjectProperties
        {
            public const string Name = "Name";
            public const string Type = "Type";
            public const string PositionPoint = "PositionPoint";
            public const string Size = "Size";
            public const string Direction = "Direction";
            public const string Behaviours = "Behaviours";
        }

        public struct ShapeSceneObjectProperties
        {
            public const string Shape = "Shape";
            public const string Fill = "Fill";
        }

        public struct ImageSceneObjectProperties
        {
            public const string Image = "Image";
        }

        public struct AnimatedSceneObjectProperties
        {
            public const string Animation = "Animation";
        }
        
        public struct BaseBehaviourProperties
        {
            public const string Type = "Type";
        }
        
        public struct RouteProperties
        {
            public const string IsLoop = "IsLoop";
            public const string RoutePoints = "RoutePoints";
            public const string PointName = "PointName";
            public const string Position = "Position";
        }
        
        #endregion
        public static MethodInfo GetToDictMethod(Type type)
        {
            var assembly = type.Assembly;
            var methods = InheritedClassMethods.GetExtensionMethods(assembly, type);
            return methods.FirstOrDefault(x => x.Name == "ToDict");
        }
        
        // Scene
        public static Dictionary<string, dynamic> ToDict(this Scene scene)
        {
            return new Dictionary<string, dynamic>()
            {
                {SceneProperties.Name, scene.Name}, 
                {SceneProperties.Description, scene.Description},
                {SceneProperties.Size, new[] {scene.Size.Width, scene.Size.Height}},
                {SceneProperties.Background, scene.Background?.UriSource.AbsolutePath},
                {SceneProperties.Objects, scene.Objects.Select(sceneObject => sceneObject.ToDict()).ToList()}
            };
        }
        
        // SceneObject
        public static Dictionary<string, dynamic> ToDict(this SceneObject sceneObj)
        {
            var baseDict = new Dictionary<string, dynamic>()
            {
                {SceneObjectProperties.Name, sceneObj.Name},
                {SceneObjectProperties.Type, sceneObj.GetType().Name},
                {SceneObjectProperties.PositionPoint, new[] {sceneObj.PositionPoint.X, sceneObj.PositionPoint.Y}},
                {SceneObjectProperties.Size, new[] {sceneObj.Size.Width, sceneObj.Size.Height}},
                {SceneObjectProperties.Direction, sceneObj.Direction.ToString()},
                {SceneObjectProperties.Behaviours, sceneObj.Behaviours.Select(beh => beh.ToDict()).ToList()}
            };
            
            var concreteDict = (Dictionary<string, dynamic>) GetToDictMethod(sceneObj.GetType())?.Invoke(sceneObj, new object[]{sceneObj});
            return concreteDict is null ? baseDict : 
                baseDict.Union(concreteDict).ToDictionary(pair => pair.Key, pair => pair.Value);
        }
        
        // ShapeSceneObject
        public static Dictionary<string, dynamic> ToDict(this ShapeSceneObject shapeObj)
        {
            return new Dictionary<string, dynamic>()
            {
                {ShapeSceneObjectProperties.Shape, shapeObj.FrameContent.GetType().Name},
                {ShapeSceneObjectProperties.Fill, shapeObj.FrameContent.Fill}
            };
        }
        
        // ImageSceneObject
        public static Dictionary<string, dynamic> ToDict(this ImageSceneObject imageObj)
        {
            return new Dictionary<string, dynamic>()
            {
                {ImageSceneObjectProperties.Image, imageObj.FrameContent.UriSource.AbsolutePath}
            };
        }
        
        // AnimatedSceneObject
        public static Dictionary<string, dynamic> ToDict(this AnimatedSceneObject animObj)
        {
            return new Dictionary<string, dynamic>()
            {
                {AnimatedSceneObjectProperties.Animation, animObj.Animation.Name}
            };
        }

        // BaseBehaviour
        public static Dictionary<string, dynamic> ToDict(this BaseBehaviour behaviour)
        {
            var behType = behaviour.GetType();

            var props = behType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var dict = new Dictionary<string, dynamic>()
            {
                {BaseBehaviourProperties.Type, behType.Name}
            };
            foreach (var prop in props)
            {
                if (!prop.CanWrite)
                    continue;

                var value = prop.GetValue(behaviour);
                var toDictMethod = GetToDictMethod(value.GetType());
                if (toDictMethod != null)
                    value = toDictMethod.Invoke(value, new object[] {value});
                dict.Add(prop.Name, value);
            }
            
            return dict;
        }
        
        // Route
        public static Dictionary<string, dynamic> ToDict(this Route route)
        {
            return new Dictionary<string, dynamic>()
            {
                {RouteProperties.IsLoop, route.IsLoop},
                {RouteProperties.RoutePoints, route.RoutePoints.Select(
                    point => new Dictionary<string, dynamic>()
                    {
                        {RouteProperties.PointName, point.Name},
                        {RouteProperties.Position, new double[]{point.PositionPoint.X, point.PositionPoint.Y}}
                    }).ToList()}
            };
        }
    }
}