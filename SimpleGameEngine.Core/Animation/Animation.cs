using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace SimpleGameEngine.Core.Animation
{

    public enum AnimationState
    {
        Stopped,
        Playing,
        Paused,
        Finished
    }
    
    public class Animation
    {
        private int _delayTicks;
        private int _currentLoop = 0;
        private readonly IEnumerator<BitmapImage> _slideEnumerator;
        private AnimationState _state = AnimationState.Stopped;

        public Animation(string name, IEnumerable<BitmapImage> slides, int delayTicks)
        {
            Name = name;
            _slideEnumerator = slides.GetEnumerator();
            _delayTicks = delayTicks;
        }
        
        ~Animation() => Stop();
        
        public string Name { get; }
        public bool IsLooped { get; set; } = true;

        public AnimationState State
        {
            get => _state;
            private set
            {
                if (_state == value)
                    return;
                _state = value;
                OnAnimationStateChanged(value);
            }
        }

        public BitmapImage Current
        {
            get
            {
                if (_slideEnumerator.Current == null)
                    MoveNext();
                return _slideEnumerator.Current;
            }
        }


        public void Start()
        {
            GameLoop.RenderEvent += Render;
            State = AnimationState.Playing;
        }

        public void Pause()
        {
            GameLoop.RenderEvent -= Render;
            State = AnimationState.Paused;
        }

        public void Stop()
        {
            GameLoop.RenderEvent -= Render;
            _slideEnumerator.Reset();
            _currentLoop = 0;
            State = AnimationState.Stopped;
        }

        private void MoveNext()
        {
            if (_slideEnumerator.MoveNext()) 
                return;
            if (IsLooped)
                _slideEnumerator.Reset();
            else
            {
                Stop();
                State = AnimationState.Finished;
            }
        }
        
        private void Render()
        {
            if (_currentLoop >= _delayTicks)
            {
                MoveNext();
                _currentLoop = 0;
            }
            _currentLoop++;
        }

        public delegate void AnimationStateHandler(AnimationState state);

        public event AnimationStateHandler AnimationStateChanged;

        protected virtual void OnAnimationStateChanged(AnimationState state)
        {
            AnimationStateChanged?.Invoke(state);
        }
    }
}