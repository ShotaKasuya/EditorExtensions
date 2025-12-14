using System;
using System.Threading;
using Module.EditorExtension.Runtime.VisualDebugger;
using Module.EditorExtension.Runtime.VisualDebugger.PhysicsCast;
using Module.Option.Runtime;
using UnityEngine;
using UnityEngine.LowLevel;

namespace _Scripts
{
    public class Cast : MonoBehaviour
    {
        [Header("Ray")]

        [SerializeField] private Transform rayOrigin;

        [SerializeField] private float rayDistance;
        [SerializeField] private bool rayHit;

        [Header("Sphere")]

        [SerializeField] private Transform sphereOrigin;

        [SerializeField] private float sphereDistance;
        [SerializeField] private float sphereRadius;
        [SerializeField] private bool sphereHit;

        private void Update()
        {
            DebugLogger.Log("update thread", Thread.CurrentThread.ManagedThreadId.ToString());
            CastVisualizer.StoreRay(
                new Ray(rayOrigin.position, Vector3.forward), rayDistance, rayHit
            );
            CastVisualizer.StoreSphere(
                new Ray(sphereOrigin.position, Vector3.forward), sphereDistance, sphereRadius, sphereHit
            );

            if (!_isPrinted)
            {
                var loopSystem = PlayerLoop.GetCurrentPlayerLoop();

                Debug.Log(loopSystem.ToString());
                _isPrinted = true;
            }
        }

        private bool _isPrinted = false;
    }
}