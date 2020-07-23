using SimpleGameEngine.Core.Behaviours;
using SimpleGameEngine.Core.SceneObjects;

namespace SimpleGameEngine.Examples.Behaviours
{
    public class TakeItemBehaviour: TriggerBehaviour
    {
        
        public string Item { get; set; }

        private SceneObject _itemObject;
        private bool _isTakenItem;
        
        public TakeItemBehaviour(SceneObject sceneObject) : base(sceneObject)
        {
        }


        protected override void Update()
        {
            base.Update();

            if (!_isTakenItem && IsActive && _itemObject is null)
                _itemObject = SceneObject.Parent.GetObjectByName(Item);

            if (Status && _itemObject != null)
            {
                SceneObject.Parent.RemoveObject(_itemObject);
                _itemObject = null;
                _isTakenItem = true;
            }
        }
    }
}