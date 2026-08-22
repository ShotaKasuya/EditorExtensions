using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Module.UnityServiceLocator
{
    public class ServiceManager
    {
        private readonly Dictionary<Type, object> m_Service = new();
        public IEnumerable<object> RegisteredServices => m_Service.Values;

        public bool TryGet<T>([NotNullWhen(true)] out T? service) where T : class
        {
            var type = typeof(T);
            if (m_Service.TryGetValue(type, out object tService))
            {
                service = (T)tService;
                return true;
            }

            service = null;
            return false;
        }

        public T Get<T>() where T : class
        {
            var type = typeof(T);
            if (m_Service.TryGetValue(type, out object tService))
            {
                return (T)tService;
            }

            throw new ArgumentException($"ServiceManager.Get: Service of type {type.FullName} not registered");
        }

        public ServiceManager Register<T>(T service) where T : class
        {
            var type = typeof(T);

            if (!m_Service.TryAdd(type, service))
            {
                Debug.LogError($"ServiceManager.Register: Service of type {type.FullName} already registered");
            }

            return this;
        }
    }
}