using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using SimpleGameEngine.Core;
using SimpleGameEngine.Core.Behaviours;
using SimpleGameEngine.Core.SceneObjects;
using InputManager = SimpleGameEngine.Core.InputManager;

namespace SimpleGameEngine
{
    public class MoveBehaviour : BaseBehaviour
    {
        public double Speed { get; set; } = 5d;
        public double Alt { get; set; } = 0d;
        
        protected double _moveMult = 0d;
        protected double _jumpSpeed = 3d;

        protected Size _sceneSize;
        protected ScaleTransform _scaleTransform;

        public MoveBehaviour(SceneObject sceneObject) : base(sceneObject)
        {
        }

        protected override void Start()
        {
            _sceneSize = SceneObject.Parent.MainCanvas.RenderSize;
            _scaleTransform = new ScaleTransform(1d, 1d);
            SceneObject.RenderTransform.Children.Add(_scaleTransform);
        }

        private double verSpeed = 8d;
        
        protected override void Update()
        {
//            Control();
            _moveMult = 1; 
            Movement();
            
            _scaleTransform.ScaleX = _moveMult > 0 ? 1d : -1d;
            
            
//            if (SceneObject is AnimatedSceneObject animatedSceneObject && 
//                animatedSceneObject.Animation.State == AnimationState.Stopped)
//                SceneObject.Parent.RemoveObject(SceneObject);
                
                
        }

        private void Control()
        {
            if (InputManager.KeyDown(Key.A))
                Speed = -Math.Abs(Speed);
            
            if (InputManager.KeyDown(Key.D))
                Speed = Math.Abs(Speed);

            if (InputManager.KeyDown(Key.W))
                Alt = - verSpeed / 1000;
            else if (InputManager.KeyDown(Key.S))
                Alt = verSpeed / 1000;
            else
                Alt = 0d;
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
                SceneObject.PositionPoint += new Vector(Speed * _moveMult / 1000, Alt / 1000);
        }
    }
}