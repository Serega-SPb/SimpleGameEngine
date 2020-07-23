using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;
using BaseProperties = SimpleGameEngine.Core.Serialization.SerializeExtensions.SceneObjectProperties;
using Properties = SimpleGameEngine.Core.Serialization.SerializeExtensions.ImageSceneObjectProperties;

namespace SimpleGameEngine.Core.SceneObjects
{
    public class ImageSceneObject : SceneObject
    {

        public ImageSceneObject()
        {
            
        }
        
        public ImageSceneObject(string name,MutablePoint positionPoint, MutableSize size, Scene parent=null) : 
            base(name, positionPoint, size, parent)
        {
        }
        
        public virtual BitmapImage FrameContent { get; set; } = new BitmapImage();
        
        protected override void PrepareElement()
        {
            if (!(Frame is Image) || Size != Frame.RenderSize || FrameContent != ((Image)Frame).Source)
                Frame = new Image()
                {
                    Width = Size.Width,
                    Height = Size.Height,
                    Source = FrameContent,
                    RenderTransform = RenderTransform,
                    RenderTransformOrigin = new Point(0.5d, 0.5d)
                };
            else
            {
                ((Image)Frame).Source = FrameContent;
            }
        }
        
        public new static ImageSceneObject FromJson(JObject jsObj)
        {
            var props = GetPropertyValues(jsObj);

            var name = props[BaseProperties.Name];
            var point = props[BaseProperties.PositionPoint];
            var size = props[BaseProperties.Size];
            var imgObj = new ImageSceneObject(name, point, size);
            imgObj.Direction = props[BaseProperties.Direction];
            BehavioursFromJson(jsObj, imgObj);

            var img = jsObj[Properties.Image].Value<string>();
            imgObj.FrameContent = new BitmapImage(new Uri(img));
            
            return imgObj;
        }
    }
}