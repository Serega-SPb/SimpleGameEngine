using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using SimpleGameEngine.Core.SceneObjects;

namespace SimpleGameEngine.Core.Behaviours
{
    public class MovePathBehaviour : BaseBehaviour
    {
        protected ScaleTransform ScaleTransform;
        protected Vector MoveVector = new Vector(0,0);

        public double Speed { get; set; } = 5d;
        public bool IsControlled { get; set; }
        public Route Route { get; set; }

        public Key MoveKey { get; set; } = Key.Space;

        protected bool IsMove; 


        public MovePathBehaviour(SceneObject sceneObject) : base(sceneObject)
        {
            Route = new Route(SceneObject) {IsLoop = true};
        }
        
        protected override void Start()
        {
            Route.InitPoints();
            ScaleTransform = new ScaleTransform(1d, 1d);
            SceneObject.RenderTransform.Children.Add(ScaleTransform);
        }
        
        protected override void Update()
        {
            if (IsControlled)
                Control();
            else
                IsMove = true;
            if (IsMove)
                Movement();
        }

        protected void Control() => 
            IsMove = InputManager.KeyDown(MoveKey);

        protected virtual void Movement()
        {
            MoveVector = Route.GetMoveVectorNormalized();
            ScaleTransform.ScaleX = MoveVector.X > 0 ? 1d : -1d;
            SceneObject.PositionPoint += (MoveVector / 1000) * Speed;
        }
    }
}