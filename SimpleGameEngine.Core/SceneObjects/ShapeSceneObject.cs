using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;
using SimpleGameEngine.Core.Behaviours;
using BaseProperties = SimpleGameEngine.Core.Serialization.SerializeExtensions.SceneObjectProperties;
using Properties = SimpleGameEngine.Core.Serialization.SerializeExtensions.ShapeSceneObjectProperties;

namespace SimpleGameEngine.Core.SceneObjects
{
    public class ShapeSceneObject : SceneObject
    {
        public static readonly Dictionary<string, Type> Shapes = new Dictionary<string, Type>()
        {
            {"Ellipse", typeof(Ellipse)},
            {"Rectangle", typeof(Rectangle)}
        };
        
        
        private Shape _frameContent;

        public ShapeSceneObject()
        {
            FrameContent = new Ellipse() {Fill = Brushes.Black};
        }
        
        public ShapeSceneObject(string name, MutablePoint positionPoint, MutableSize size, Scene parent=null) : 
            base(name, positionPoint, size, parent)
        {
        }

        public Shape FrameContent
        {
            get => _frameContent;
            set
            {
                if (_frameContent != null)
                    value.Fill = _frameContent.Fill;
                _frameContent = value;
            }
        }

        protected override void PrepareElement()
        {
            if (FrameContent == null || Frame != null && Frame.Equals(FrameContent) && Size == Frame.DesiredSize) 
                return;
            Frame = FrameContent;
            Frame.Width = Size.Width;
            Frame.Height = Size.Height;
            Frame.RenderTransform = RenderTransform;
            Frame.RenderTransformOrigin = new Point(0.5d, 0.5d);
        }

        public new static ShapeSceneObject FromJson(JObject jsObj)
        {
            var props = GetPropertyValues(jsObj);

            var name = props[BaseProperties.Name];
            var point = props[BaseProperties.PositionPoint];
            var size = props[BaseProperties.Size];
            var shapeObj = new ShapeSceneObject(name, point, size);
            shapeObj.Direction = props[BaseProperties.Direction];

            var shape = jsObj[Properties.Shape].Value<string>();
            shapeObj.FrameContent = (Shape) Activator.CreateInstance(Shapes[shape]);
            BehavioursFromJson(jsObj, shapeObj);
            
            var fill = jsObj[Properties.Fill].Value<string>();
            var fillColor = (SolidColorBrush)new BrushConverter().ConvertFromString(fill);
            shapeObj.FrameContent.Fill = fillColor;

            return shapeObj;
        }
    }
}