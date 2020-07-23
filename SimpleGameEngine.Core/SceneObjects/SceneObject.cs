using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json.Linq;
using SimpleGameEngine.Core.Behaviours;
using SimpleGameEngine.Core.Enums;
using Properties = SimpleGameEngine.Core.Serialization.SerializeExtensions.SceneObjectProperties;
using BehaviourProperties = SimpleGameEngine.Core.Serialization.SerializeExtensions.BaseBehaviourProperties;

namespace SimpleGameEngine.Core.SceneObjects
{
    public abstract class SceneObject
    {
        protected SceneObject()
        {
            Name = GetType().Name;
            PositionPoint = new MutablePoint(0,0);
            Size = new MutableSize(10,10);
        }
        
        protected SceneObject(string name, MutablePoint positionPoint, MutableSize size, Scene parent=null)
        {
            Name = name;
            PositionPoint = positionPoint;
            Size = size;
            Parent = parent;
            parent?.AddSceneObject(this);
        }
        public string Name { get; set; }
        public MutablePoint PositionPoint { get; set; }
        public MutableSize Size { get; set; }
        public Scene Parent { get; private set; }

        public List<BaseBehaviour> Behaviours { get; } = new List<BaseBehaviour>();

        public PointDirection Direction { get; set; } = PointDirection.BottomLeft;

        public TransformGroup RenderTransform { get; } = new TransformGroup();

        protected FrameworkElement Frame;

        protected abstract void PrepareElement();

        public void Render()
        {
            PrepareElement();
            if (Frame is null)
                return;
            Parent.MainCanvas.Children.Add(Frame);
            SetPosition(Frame);
        }

        public void SetParent(Scene scene)
        {
            Parent?.RemoveObject(this);
            Parent = scene;
            scene.AddSceneObject(this);
        }

        public void AddBehaviour(BaseBehaviour behaviour) => Behaviours.Add(behaviour);
        
        public void RemoveBehaviour(BaseBehaviour behaviour) => Behaviours.Remove(behaviour);

        public BaseBehaviour GetBehaviourByType(Type behType) 
            => Behaviours.FirstOrDefault(b => b.GetType() == behType);

        public IEnumerable<BaseBehaviour> GetBehavioursByType(Type behType)
            => Behaviours.Where(b => b.GetType() == behType);
        
        private void SetPosition(UIElement element)
        {
            var top = double.NaN;
            var bottom = double.NaN;
            var left = double.NaN;
            var right = double.NaN;

            var concretePoint = Parent.ConvertPoint(PositionPoint.ToPoint());

            switch (Direction)
            {
                case PointDirection.TopLeft:
                    top = concretePoint.Y;
                    left = concretePoint.X;
                    break;
                case PointDirection.TopRight:
                    top = concretePoint.Y;
                    right = concretePoint.X;
                    break;
                case PointDirection.BottomLeft:
                    bottom = concretePoint.Y;
                    left = concretePoint.X;
                    break;
                case PointDirection.BottomRight:
                    bottom = concretePoint.Y;
                    right = concretePoint.X;
                    break;
            }
            
            Canvas.SetTop(element, top);
            Canvas.SetBottom(element, bottom);
            Canvas.SetLeft(element, left);
            Canvas.SetRight(element, right);
        }

        public static SceneObject FromJson(JObject jsObj)
        {
            throw new NotImplementedException($"{nameof(SceneObject)} is abstract class");
        }

        protected static Dictionary<string, dynamic> GetPropertyValues(JObject jsObj)
        {
            var props = new Dictionary<string, dynamic>();
            props.Add(Properties.Name, jsObj[Properties.Name].Value<string>());

            var posPointValues = jsObj[Properties.PositionPoint].Values<double>().ToArray();
            props.Add(Properties.PositionPoint, new MutablePoint(posPointValues[0], posPointValues[1]));
            
            var sizeValues = jsObj[Properties.Size].Values<double>().ToArray();
            props.Add(Properties.Size, new MutableSize(sizeValues[0], sizeValues[1]));

            var dirValue = jsObj[Properties.Direction].Value<string>();
            props.Add(Properties.Direction, (PointDirection)Enum.Parse(typeof(PointDirection), dirValue));
            
            return props;
        }
        
        protected static void BehavioursFromJson(JObject jsObj, SceneObject parent)
        {
            var behaviourTypes = InheritedClassMethods.GetInheritedClasses<BaseBehaviour>();
            foreach (var beh in jsObj[Properties.Behaviours])
            {
                var type = behaviourTypes.First(x => x.Name.Equals(beh[BehaviourProperties.Type].Value<string>()));
                var instance = (BaseBehaviour)Activator.CreateInstance(type, parent);
                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!property.CanWrite || beh[property.Name] is null)
                        continue;
                    
                    var fromJsonMethod = property.PropertyType.GetMethods().FirstOrDefault(x => x.Name == "FromJson");
                    var value = fromJsonMethod != null 
                        ? fromJsonMethod.Invoke(null, new object[] {beh[property.Name], parent}) 
                        : beh[property.Name].ToObject(property.PropertyType);
                    property.SetValue(instance, value);
                }
            }
        } 
    }
}