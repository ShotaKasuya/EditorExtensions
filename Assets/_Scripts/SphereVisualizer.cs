using System.Threading;
using Module.EditorExtension.Runtime.VisualDebugger;
using Module.EditorExtension.Runtime.VisualDebugger.PhysicsCast;
using UnityEngine;

namespace _Scripts
{
    public class SphereVisualizer : MonoBehaviour
    {
        [Header("Sphere")]

        private Transform _selfTransform;
        [SerializeField] private float sphereDistance;
        [SerializeField] private float sphereRadius;
        [SerializeField] private bool sphereHit;

        private void Awake()
        {
            _selfTransform = transform;
        }

        private void Update()
        {
            DebugLogger.Log("update thread", Thread.CurrentThread.ManagedThreadId.ToString());
            CastVisualizer.StoreSphere(
                new Ray(_selfTransform.position, Vector3.forward), sphereDistance, sphereRadius, sphereHit
            );
        }
    }
}