using System;
using SimpleGameEngine.Core.Behaviours;
using SimpleGameEngine.Core.SceneObjects;

namespace SimpleGameEngine.Examples.Behaviours
{
    public class HouseRouteBehaviour : MovePathBehaviour
    {

        private bool _waitTrigger;
        private bool _triggerUsed;
        private TriggerBehaviour _takeItemTrigger;
        private TriggerBehaviour _changeSceneTrigger;

        public HouseRouteBehaviour(SceneObject sceneObject) : base(sceneObject)
        {
        }
        
        protected override void Update()
        {

            if (_takeItemTrigger is null)
                _takeItemTrigger = Route.CurrentPoint.GetBehaviourByType(typeof(TakeItemBehaviour)) as TakeItemBehaviour;

            if (_changeSceneTrigger is null)
                _changeSceneTrigger = Route.CurrentPoint.GetBehaviourByType(typeof(ChangeSceneBehaviour)) as ChangeSceneBehaviour;
            
            if (_waitTrigger)
                return;

            base.Update();
            OnMoveEvent(IsMove, GetVerticalInt());
        }

        private int GetVerticalInt()
        {
            var vert = MoveVector.Y;
            if (Math.Abs(vert) < 0.5)
                return 0;
            return vert > 0 ? 1 : -1;
        }
        
        protected override void Movement()
        {
            MoveVector = Route.GetMoveVector();
            if (!_triggerUsed && _takeItemTrigger != null && MoveVector.Length < 1)
            {
                _takeItemTrigger.IsActive = true;
                _waitTrigger = true;
                _takeItemTrigger.TriggerEvent += OnTakeItemTrigger;
                IsMove = false;
            }

            if (_changeSceneTrigger != null && MoveVector.Length < 1)
                _changeSceneTrigger.IsActive = true;
            
            if (MoveVector.Length == 0)
                IsMove = false;
            ScaleTransform.ScaleX = MoveVector.X > 0 ? 1d : -1d;
            if (MoveVector.Length > 0)
                MoveVector.Normalize();
            SceneObject.PositionPoint += (MoveVector / 1000) * Speed;
        }

        private void OnTakeItemTrigger(bool status)
        {
            if (!status)
                return;
            
            _triggerUsed = true;
            _waitTrigger = false;
            _takeItemTrigger.TriggerEvent -= OnTakeItemTrigger;
            _takeItemTrigger = null;
        }
        
        public delegate void MoveHandler(bool flag, int vertical);
        public event MoveHandler MoveEvent;
        protected virtual void OnMoveEvent(bool flag, int vertical) => MoveEvent?.Invoke(flag, vertical);
    }
}