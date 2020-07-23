using System.Windows.Input;
using SimpleGameEngine.Core.SceneObjects;

namespace SimpleGameEngine.Core.Behaviours
{
    public class TriggerBehaviour:BaseBehaviour
    {
        private bool _status;

        public bool IsActive { get; set; }
        public bool IsControlled { get; set; }
        public Key TriggerKey { get; set; }

        protected bool Status
        {
            get => _status;
            set
            {
                if (_status == value)
                    return;
                _status = value;
                OnTriggerEvent(value);
            }
        }

        public TriggerBehaviour(SceneObject sceneObject) : base(sceneObject)
        {
        }

        protected override void Update()
        {
            if (!IsActive)
                return;

            Status = !IsControlled || InputManager.KeyDown(TriggerKey);
        }

        public delegate void TriggerHandler(bool status);

        public event TriggerHandler TriggerEvent;

        protected virtual void OnTriggerEvent(bool status) => TriggerEvent?.Invoke(status);
    }
}