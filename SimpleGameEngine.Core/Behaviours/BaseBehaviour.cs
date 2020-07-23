using Newtonsoft.Json.Linq;
using SimpleGameEngine.Core.SceneObjects;

namespace SimpleGameEngine.Core.Behaviours
{
    public abstract class BaseBehaviour
    {

        private bool _isStarted;

        public string Name { get; set; }
        public SceneObject SceneObject { get; }

        public BaseBehaviour(SceneObject sceneObject)
        {
            Name = GetType().Name;
            SceneObject = sceneObject;
            SceneObject.AddBehaviour(this);
        }
        
        public void EnableBehaviour() => GameLoop.UpdateStateEvent += UpdateState;
        public void DisableBehaviour() => GameLoop.UpdateStateEvent -= UpdateState;

        protected virtual void Start()
        {
            
        }

        protected virtual void Update()
        {
            
        }
        
        private void UpdateState()
        {
            if (!_isStarted)
            {
                Start();
                _isStarted = true;
            }
            Update();
        }

        
    }
}