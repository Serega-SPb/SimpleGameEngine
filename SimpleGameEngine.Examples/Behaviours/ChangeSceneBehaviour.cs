using SimpleGameEngine.Core;
using SimpleGameEngine.Core.Behaviours;
using SimpleGameEngine.Core.SceneObjects;

namespace SimpleGameEngine.Examples.Behaviours
{
    public class ChangeSceneBehaviour: TriggerBehaviour
    {
        public string NextSceneName { get; set; }
        
        public ChangeSceneBehaviour(SceneObject sceneObject) : base(sceneObject)
        {
        }

        protected override void Update()
        {
            base.Update();
            
            if (IsActive && Status)
                SceneManager.Instance.ChangeScene(SceneObject.Parent, NextSceneName);
        }
    }
}