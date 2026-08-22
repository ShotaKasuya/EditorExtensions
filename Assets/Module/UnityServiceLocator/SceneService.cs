using UnityEngine;

namespace Module.UnityServiceLocator
{
    public abstract class SceneService<T> : MonoBehaviour where T : class
    {
        private void Awake()
        {
            var t = this as T;
            ServiceLocator.ForSceneOf(this).Register(t.AssertNull());
        }
    }
}