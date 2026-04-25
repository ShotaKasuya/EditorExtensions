using UnityEngine;

namespace Module.EditorExtension.Runtime.VisualDebugger.PhysicsCast;

public static class RigidbodyExtensions
{
    public static bool RaycastVisualized(this Rigidbody rb, Vector3 direction, out RaycastHit hitInfo, float maxDistance = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        var ray = new Ray(rb.position, direction);
        bool hit = Physics.Raycast(ray, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
        float dist = hit ? hitInfo.distance : (maxDistance == Mathf.Infinity ? 100f : maxDistance);
        CastVisualizer.StoreRay(ray, dist, hit);
        return hit;
    }

    public static bool SphereCastVisualized(this Rigidbody rb, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        var ray = new Ray(rb.position, direction);
        bool hit = Physics.SphereCast(ray, radius, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
        float dist = hit ? hitInfo.distance : (maxDistance == Mathf.Infinity ? 100f : maxDistance);
        CastVisualizer.StoreSphere(ray, dist, radius, hit);
        return hit;
    }

    public static bool BoxCastVisualized(this Rigidbody rb, Vector3 halfExtents, Vector3 direction, Quaternion orientation, out RaycastHit hitInfo, float maxDistance = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        var ray = new Ray(rb.position, direction);
        bool hit = Physics.BoxCast(rb.position, halfExtents, direction, out hitInfo, orientation, maxDistance, layerMask, queryTriggerInteraction);
        float dist = hit ? hitInfo.distance : (maxDistance == Mathf.Infinity ? 100f : maxDistance);
        CastVisualizer.StoreBox(ray, dist, halfExtents, orientation, hit);
        return hit;
    }

    public static bool SweepTestVisualized(this Rigidbody rb, Vector3 direction, out RaycastHit hitInfo, float maxDistance = Mathf.Infinity, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        bool hit = rb.SweepTest(direction, out hitInfo, maxDistance, queryTriggerInteraction);
        float dist = hit ? hitInfo.distance : (maxDistance == Mathf.Infinity ? 100f : maxDistance);

        // Try to find a collider to visualize the sweep
        var col = rb.GetComponentInChildren<Collider>();
        if (col is SphereCollider sphere)
        {
            CastVisualizer.StoreSphere(new Ray(rb.position + sphere.center, direction), dist, sphere.radius * Mathf.Max(rb.transform.lossyScale.x, rb.transform.lossyScale.y, rb.transform.lossyScale.z), hit);
        }
        else if (col is BoxCollider box)
        {
            var scale = rb.transform.lossyScale;
            var halfExtents = Vector3.Scale(box.size, scale) * 0.5f;
            CastVisualizer.StoreBox(new Ray(rb.position + rb.transform.rotation * Vector3.Scale(box.center, scale), direction), dist, halfExtents, rb.transform.rotation, hit);
        }
        else
        {
            // Fallback to Ray
            CastVisualizer.StoreRay(new Ray(rb.position, direction), dist, hit);
        }

        return hit;
    }
}
