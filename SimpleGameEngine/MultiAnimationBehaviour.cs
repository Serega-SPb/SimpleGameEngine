using System.Windows.Input;
using SimpleGameEngine.Core.Animation;
using SimpleGameEngine.Core.SceneObjects;
using InputManager = SimpleGameEngine.Core.InputManager;

namespace SimpleGameEngine
{
    public class MultiAnimationBehaviour: MoveBehaviour
    {
        private Animator _animator;

        public MultiAnimationBehaviour(SceneObject sceneObject) : base(sceneObject)
        {
        }

        protected override void Start()
        {
            base.Start();
            AnimatorSettings();
        }

        private const string Idle = "Idle";
        private const string Walk = "Run";
        private const string JumpUp = "Jump_up";
        private const string JumpDown = "Jump_down";

        private void AnimatorSettings()
        {
            _animator = new Animator(SceneObject as AnimatedSceneObject);
            _animator.AddAnimation(Idle, AnimationManager.Instance.GetAnimationByName(Idle));
            _animator.AddAnimation(Walk, AnimationManager.Instance.GetAnimationByName(Walk));
            _animator.AddAnimation(JumpUp, AnimationManager.Instance.GetAnimationByName(JumpUp));
            _animator.AddAnimation(JumpDown, AnimationManager.Instance.GetAnimationByName(JumpDown));
            _animator.AddRelation(JumpUp, JumpDown);
            _animator.AddRelation(JumpDown, Idle);
        }

        private bool _isJumpUp;
        private bool _isJumpDown;

        protected override void Update()
        {
            Control();

            _isJumpUp = _animator.CurrentState == JumpUp;
            _isJumpDown = _animator.CurrentState == JumpDown;
            
            
            if (_isJumpUp)
                Alt = _jumpSpeed;
            else if (_isJumpDown)
                Alt = -_jumpSpeed;
            else
                Alt = 0;

            if (_isJumpUp || _isJumpDown)
                _moveMult /= 2;
            
            if (_moveMult != 0)
                _scaleTransform.ScaleX = _moveMult > 0 ? 1d : -1d;
            
            if (!_isJumpUp && !_isJumpDown)
                _animator.CurrentState = _moveMult == 0 ? Idle : Walk;
            Movement();
        }

        private void Control()
        {
            if (InputManager.KeyDown(Key.D))
                _moveMult = 1;
            else if (InputManager.KeyDown(Key.A))
                _moveMult = -1;
            else
                _moveMult = 0;
            
            if (!_isJumpUp && !_isJumpDown && InputManager.KeyDown(Key.Space))
                _animator.CurrentState = JumpUp;
        }
    }
}