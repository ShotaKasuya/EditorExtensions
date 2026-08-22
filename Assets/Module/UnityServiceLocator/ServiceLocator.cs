using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Module.UnityServiceLocator
{
    public class ServiceLocator : MonoBehaviour
    {
        private static ServiceLocator? s_Global;
        private static Dictionary<Scene, ServiceLocator> s_SceneContainers = new();

        private readonly ServiceManager m_Services = new();

        private const string k_GlobalServiceLocatorName = "ServiceLocator [Global]";
        private const string k_SceneServiceLocatorName = "ServiceLocator [Scene]";

        internal void ConfigureAsGlobal()
        {
            if (s_Global == this)
            {
                Debug.LogWarning("ServiceLocator.ConfigureAsGlobal: Already configured as global", this);
            }
            else if (s_Global != null)
            {
                Debug.LogError(
                    "ServiceLocator.ConfigureAsGlobal: Another ServiceLocator is already configured as global", this);
            }
            else
            {
                s_Global = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        internal void ConfigureForScene()
        {
            var scene = gameObject.scene;

            if (s_SceneContainers.ContainsKey(scene))
            {
                Debug.LogError(
                    "ServiceLocator.ConfigureForScene: Another ServiceLocator is already configured for scene",
                    this
                );
                return;
            }

            s_SceneContainers.Add(scene, this);
        }

        public static ServiceLocator Global
        {
            get
            {
                if (s_Global != null)
                {
                    return s_Global;
                }

                if (FindAnyObjectByType<ServiceLocatorGlobal>() is { } found)
                {
                    found.BootstrapOnDemand();
                    return s_Global.AssertNull();
                }

                var container = new GameObject(k_GlobalServiceLocatorName, typeof(ServiceLocator));
                container.AddComponent<ServiceLocatorGlobal>().BootstrapOnDemand();

                return s_Global.AssertNull();
            }
        }

        public static ServiceLocator ForSceneOf(MonoBehaviour monoBehaviour)
        {
            var scene = monoBehaviour.gameObject.scene;
            if (s_SceneContainers.TryGetValue(scene, out var container) && container != monoBehaviour)
            {
                return container;
            }

            var gameObjectList = scene.GetRootGameObjects();

            foreach (var go in gameObjectList.Where(x => x.GetComponent<ServiceLocatorScene>() != null))
            {
                if (go.TryGetComponent(out ServiceLocatorScene bootstrapper) && bootstrapper.Container != monoBehaviour)
                {
                    bootstrapper.BootstrapOnDemand();
                    return bootstrapper.Container;
                }
            }

            return s_Global.AssertNull();
        }

        public static ServiceLocator For(MonoBehaviour monoBehaviour)
        {
            var result = monoBehaviour.GetComponentInParent<ServiceLocator>().OrNull();
            result ??= ForSceneOf(monoBehaviour);

            return result;
        }

        public ServiceLocator Register<T>(T service) where T : class
        {
            m_Services.Register(service);
            return this;
        }

        public T Get<T>() where T : class
        {
            T? service;
            if (TryGetService(out service))
            {
                return service;
            }

            if (TryGetNextInHierarchy(out var container))
            {
                return container.Get<T>();
            }

            throw new ArgumentException($"Could not resolve type '{typeof(T).FullName}'.");
        }

        public bool TryGet<T>([NotNullWhen(true)] out T? service) where T : class
        {
            service = null;

            if (TryGetService(out service))
            {
                return true;
            }

            return TryGetNextInHierarchy(out ServiceLocator? container) && container.TryGet(out service);
        }

        private bool TryGetService<T>([NotNullWhen(true)] out T? service) where T : class
        {
            return m_Services.TryGet(out service);
        }

        private bool TryGetNextInHierarchy([NotNullWhen(true)] out ServiceLocator? container)
        {
            if (this == s_Global)
            {
                container = null;
                return false;
            }

            container = transform.parent.OrNull()?.GetComponentInParent<ServiceLocator>().OrNull() ?? ForSceneOf(this);
            return container != null;
        }

        private void OnDestroy()
        {
            if (this == s_Global)
            {
                s_Global = null;
            }
            else if (s_SceneContainers.ContainsValue(this))
            {
                s_SceneContainers.Remove(gameObject.scene);
            }
        }

        // https://docs.unity3d.com/ScriptReference/RuntimeInitializeOnLoadMethodAttribute.html
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            s_Global = null;
            s_SceneContainers = new Dictionary<Scene, ServiceLocator>();
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/ServiceLocator/Add Global")]
        private static void AddGlobal()
        {
            var _ = new GameObject(k_GlobalServiceLocatorName, typeof(ServiceLocatorGlobal));
        }

        [MenuItem("GameObject/ServiceLocator/Add Scene")]
        private static void AddScene()
        {
            var _ = new GameObject(k_SceneServiceLocatorName, typeof(ServiceLocatorScene));
        }
#endif
    }
}