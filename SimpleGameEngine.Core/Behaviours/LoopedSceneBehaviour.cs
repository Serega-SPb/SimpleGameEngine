using System.Windows;
using SimpleGameEngine.Core.SceneObjects;

namespace SimpleGameEngine.Core.Behaviours
{
    public class LoopedSceneBehaviour: BaseBehaviour
    {
        private Size _sceneSize;
        
        public LoopedSceneBehaviour(SceneObject sceneObject) : base(sceneObject)
        {
        }
        
        protected override void Start()
        {
            _sceneSize = SceneObject.Parent.MainCanvas.RenderSize;
        }

        protected override void Update()
        {
            if (SceneObject.PositionPoint.X > _sceneSize.Width + SceneObject.Size.Width)
                SceneObject.PositionPoint = new MutablePoint(-SceneObject.Size.Width, SceneObject.PositionPoint.Y);
            
            else if (SceneObject.PositionPoint.X < -SceneObject.Size.Width)
                SceneObject.PositionPoint = new MutablePoint(_sceneSize.Width + SceneObject.Size.Width, SceneObject.PositionPoint.Y);
            
            else if (SceneObject.PositionPoint.Y > _sceneSize.Height + SceneObject.Size.Height)
                SceneObject.PositionPoint = new MutablePoint(SceneObject.PositionPoint.X, -SceneObject.Size.Height);
            
            else if (SceneObject.PositionPoint.Y < -SceneObject.Size.Height)
                SceneObject.PositionPoint = new MutablePoint(SceneObject.PositionPoint.X, _sceneSize.Height + SceneObject.Size.Height);
        }
    }
}