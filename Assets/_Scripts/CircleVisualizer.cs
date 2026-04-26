using Module.EditorExtension.Runtime.VisualDebugger.PhysicsCast;
using UnityEngine;

namespace _Scripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class CircleVisualizer : MonoBehaviour
    {
        private Rigidbody2D _selfRigidbody;
        private Collider2D _selfCollider;
        private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[8];

        [SerializeField] private float rayDistance;

        private void Awake()
        {
            _selfRigidbody = GetComponent<Rigidbody2D>();
            _selfCollider = GetComponent<Collider2D>();
        }

        private void Update()
        {
            _selfRigidbody.CastVisualized(
                _selfCollider, Vector2.right, _hitBuffer, rayDistance
            );
        }

    }
}