using Module.EditorExtension.Runtime.VisualDebugger.PhysicsCast;
using UnityEngine;

namespace _Scripts
{
    public class RayVisualize : MonoBehaviour
    {
        [Header("Ray")]

        private Transform _selfTransform;

        [SerializeField] private float rayDistance;
        [SerializeField] private bool rayHit;

        private void Awake()
        {
            _selfTransform = transform;
        }

        private void Update()
        {
            CastVisualizer.StoreRay(
                new Ray(_selfTransform.position, Vector3.forward), rayDistance, rayHit
            );
        }
    }
}