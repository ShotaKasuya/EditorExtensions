using UnityEngine;

namespace Module.UnityServiceLocator
{
    [AddComponentMenu("ServiceLocator/ServiceLocator Global")]
    public class ServiceLocatorGlobal : Bootstrapper
    {
        protected override void Bootstrap()
        {
            Container.ConfigureAsGlobal();
        }
    }
}