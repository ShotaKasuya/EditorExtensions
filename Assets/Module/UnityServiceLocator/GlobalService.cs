using UnityEngine;

namespace Module.UnityServiceLocator
{
    public abstract class GlobalService<T> : MonoBehaviour where T : class
    {
        private void Awake()
        {
            if (ServiceLocator.Global.TryGetComponent<T>(out _))
            {
                Destroy(gameObject);
            }

            var t = this as T;
            ServiceLocator.Global.Register(t.AssertNull());
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void OnAwake() { }
    }
}