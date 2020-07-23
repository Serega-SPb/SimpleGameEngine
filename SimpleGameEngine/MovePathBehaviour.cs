using System.Windows;
using System.Windows.Media;
using SimpleGameEngine.Core;
using SimpleGameEngine.Core.Behaviours;
using SimpleGameEngine.Core.SceneObjects;

namespace SimpleGameEngine
{
    public class OldMovePathBehaviour : BaseBehaviour
    {
        private Size _sceneSize;
        private ScaleTransform _scaleTransform;
        
        public double Speed { get; set; } = 5d;
        public double Alt { get; set; } = 0d;
        
        protected double _moveMult = 0d;
        protected double _jumpSpeed = 3d;
        
        private Vector moveVector = new Vector(0,0);
        private Route _route;

//        private SceneObject _point1;
//        private SceneObject _point2;
        

        public OldMovePathBehaviour(SceneObject sceneObject) : base(sceneObject)
        {
        }
        
        protected override void Start()
        {
            _sceneSize = SceneObject.Parent.MainCanvas.RenderSize;
            _scaleTransform = new ScaleTransform(1d, 1d);
            SceneObject.RenderTransform.Children.Add(_scaleTransform);
            _route = new Route(SceneObject) {IsLoop = true};
            _route.AddPoint(SceneObject.Parent.GetObjectByName("Point1"));
            _route.AddPoint(SceneObject.Parent.GetObjectByName("Point2"));
            _route.AddPoint(SceneObject.Parent.GetObjectByName("Point3"));
        }
        
        protected override void Update()
        {
            moveVector = _route.GetMoveVectorNormalized();
            _scaleTransform.ScaleX = moveVector.X > 0 ? 1d : -1d;
            Movement();
        }

        protected void Movement()
        {
            if (SceneObject.PositionPoint.X > _sceneSize.Width + SceneObject.Size.Width)
                SceneObject.PositionPoint = new MutablePoint(-SceneObject.Size.Width, SceneObject.PositionPoint.Y);
            
            else if (SceneObject.PositionPoint.X < -SceneObject.Size.Width)
                SceneObject.PositionPoint = new MutablePoint(_sceneSize.Width + SceneObject.Size.Width, SceneObject.PositionPoint.Y);
            
            else if (SceneObject.PositionPoint.Y > _sceneSize.Height + SceneObject.Size.Height)
                SceneObject.PositionPoint = new MutablePoint(SceneObject.PositionPoint.X, -SceneObject.Size.Height);
            
            else if (SceneObject.PositionPoint.Y < -SceneObject.Size.Height)
                SceneObject.PositionPoint = new MutablePoint(SceneObject.PositionPoint.X, _sceneSize.Height + SceneObject.Size.Height);
            else
                SceneObject.PositionPoint += (moveVector / 1000) * Speed;
        }
    }
}