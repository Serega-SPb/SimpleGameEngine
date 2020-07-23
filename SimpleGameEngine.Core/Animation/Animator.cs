using System.Collections.Generic;
using SimpleGameEngine.Core.SceneObjects;

namespace SimpleGameEngine.Core.Animation
{

    public class Animator
    {
        private readonly Dictionary<string, Animation> _animations = new Dictionary<string, Animation>();
        private readonly Dictionary<string, string> _relations = new Dictionary<string, string>();
        private string _currentState;

        public Animator(AnimatedSceneObject animatedSceneObject)
        {
            AnimatedSceneObject = animatedSceneObject;
        }

        public void AddAnimation(string state, Core.Animation.Animation animation) =>
            _animations.Add(state, animation);

        public void AddRelation(string state, string nextState)
        {
            _relations.Add(state, nextState);
            _animations[state].IsLooped = false;
        }

        public string CurrentState
        {
            get => _currentState;
            set
            {
                if (_currentState == value)
                    return;
                if (!string.IsNullOrWhiteSpace(_currentState) && _animations.ContainsKey(_currentState))
                    _animations[_currentState].Stop();
                _currentState = value;
                ApplyState();
            }
        }

        public AnimatedSceneObject AnimatedSceneObject { get; set; }

        public Animation CurrentAnimation => GetAnimation();
    

    private Animation GetAnimation()
        {
            if (string.IsNullOrWhiteSpace(CurrentState))
                return null;

            var animation = _animations[CurrentState];
            if (animation.State == AnimationState.Stopped)
                animation.Start();
            return animation;
        }

        private void CheckState(AnimationState state)
        {
            if (state == AnimationState.Finished && _relations.ContainsKey(CurrentState))
                CurrentState = _relations[CurrentState];
        }
        
        private void ApplyState()
        {
            if (AnimatedSceneObject == null)
                return;
            AnimatedSceneObject.Animation.AnimationStateChanged -= CheckState;
            AnimatedSceneObject.Animation = CurrentAnimation;
            AnimatedSceneObject.Animation.AnimationStateChanged += CheckState;
        }
    }
}