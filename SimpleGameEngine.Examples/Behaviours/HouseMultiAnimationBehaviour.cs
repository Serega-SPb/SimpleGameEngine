using SimpleGameEngine.Core.Animation;
using SimpleGameEngine.Core.Behaviours;
using SimpleGameEngine.Core.SceneObjects;

namespace SimpleGameEngine.Examples.Behaviours
{
    public class HouseMultiAnimationBehaviour: BaseBehaviour
    {
        private AnimationManager _animationManager = AnimationManager.Instance;
        private Animator _animator;

        public HouseMultiAnimationBehaviour(SceneObject sceneObject) : base(sceneObject)
        {
        }

        protected override void Start()
        {
            if (SceneObject.GetBehaviourByType(typeof(HouseRouteBehaviour)) is HouseRouteBehaviour moveBeh) 
                moveBeh.MoveEvent += ChangeMoveAnimation;
            AnimatorSettings();
        }

        private const string Idle = "Idle";
        private const string Walk = "Walk";
        private const string JumpUp = "Jump_up";
        private const string JumpDown = "Jump_down";
        private const string GoUp = "Going_up";
        private const string GoDown = "Going_down";

        private void AnimatorSettings()
        {
            _animator = new Animator(SceneObject as AnimatedSceneObject);
            _animator.AddAnimation(Idle, _animationManager.GetAnimationByName(Idle));
            _animator.AddAnimation(Walk, _animationManager.GetAnimationByName(Walk));
            _animator.AddAnimation(JumpUp, _animationManager.GetAnimationByName(JumpUp));
            _animator.AddAnimation(JumpDown, _animationManager.GetAnimationByName(JumpDown));
            _animator.AddRelation(JumpUp, JumpDown);
            _animator.AddRelation(JumpDown, Idle);
            _animator.AddAnimation(GoUp, _animationManager.GetAnimationByName(GoUp));
            _animator.AddAnimation(GoDown, _animationManager.GetAnimationByName(GoDown));
        }

        private void ChangeMoveAnimation(bool flag, int vertical)
        {
            if (!flag)
                _animator.CurrentState = Idle;
            else if (vertical == 0)
                _animator.CurrentState = Walk;
            else
                _animator.CurrentState = vertical > 0 ? GoDown: GoUp;

            //_animator.CurrentState = flag ? Walk : Idle;
        }
    }
}