using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;
using SimpleGameEngine.Core.Animation;
using BaseProperties = SimpleGameEngine.Core.Serialization.SerializeExtensions.SceneObjectProperties;
using Properties = SimpleGameEngine.Core.Serialization.SerializeExtensions.AnimatedSceneObjectProperties;

namespace SimpleGameEngine.Core.SceneObjects
{
    public class AnimatedSceneObject : ImageSceneObject
    {
        public AnimatedSceneObject()
        {
            
        }
        
        public AnimatedSceneObject(string name, MutablePoint positionPoint, MutableSize size, string animName, Scene parent = null) :
            this(name, positionPoint, size, AnimationManager.Instance.GetAnimationByName(animName), parent)
        {
        }

        public AnimatedSceneObject(string name, MutablePoint positionPoint, MutableSize size,
            Animation.Animation animation, Scene parent = null) : base(name, positionPoint, size, parent)
        {
            Animation = animation;
            Animation.Start(); // TODO ???
        }

        public Animation.Animation Animation { get; set; }

        public override BitmapImage FrameContent => Animation?.Current;
        
        
        public new static AnimatedSceneObject FromJson(JObject jsObj)
        {
            var props = GetPropertyValues(jsObj);

            var name = props[BaseProperties.Name];
            var point = props[BaseProperties.PositionPoint];
            var size = props[BaseProperties.Size];
            var animation = jsObj[Properties.Animation].Value<string>();
            var animObj = new AnimatedSceneObject(name, point, size, animation);
            animObj.Direction = props[BaseProperties.Direction];
            BehavioursFromJson(jsObj, animObj);

            return animObj;
        }
    }
}