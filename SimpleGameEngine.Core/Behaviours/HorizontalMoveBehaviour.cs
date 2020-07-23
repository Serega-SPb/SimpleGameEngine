using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using SimpleGameEngine.Core.SceneObjects;

namespace SimpleGameEngine.Core.Behaviours
{
    public class HorizontalMoveBehaviour : BaseBehaviour
    {
        public double Speed { get; set; } = 5d;

        private double _moveMult = 0d;
        
        public bool IsControlled { get; set; }
        public int DefaultDirection { get; set; }
        private ScaleTransform ScaleTransform { get; set; }

        public HorizontalMoveBehaviour(SceneObject sceneObject) : base(sceneObject)
        {
        }

        protected override void Start()
        {
            ScaleTransform = new ScaleTransform(1d, 1d);
            SceneObject.RenderTransform.Children.Add(ScaleTransform);
        }

        protected override void Update()
        {
            if(IsControlled)
                Control();
            else
            {
                if (DefaultDirection == 0)
                    _moveMult = 0;
                else
                    _moveMult = DefaultDirection > 0 ? 1 : -1;
            }
            Movement();
        }

        private void Control()
        {
            if (InputManager.KeyDown(Key.A))
                _moveMult = -1;

            else if (InputManager.KeyDown(Key.D))
                _moveMult = 1;
            else
                _moveMult = 0;
        }

        private void Movement()
        {
            ScaleTransform.ScaleX = _moveMult > 0 ? 1d : -1d;
            SceneObject.PositionPoint += new Vector(Speed * _moveMult / 1000, 0);
        }
    }
}