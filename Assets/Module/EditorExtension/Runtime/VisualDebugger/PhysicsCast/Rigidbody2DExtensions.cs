using UnityEngine;

namespace Module.EditorExtension.Runtime.VisualDebugger.PhysicsCast;

public static class Rigidbody2DExtensions
{
    public static int RaycastVisualized(this Rigidbody2D rb, Vector2 direction, RaycastHit2D[] results, float distance = Mathf.Infinity, int layerMask = Physics2D.AllLayers)
    {
        int count = rb.Cast(direction, results, distance);
        bool hit = count > 0;
        float dist = hit ? results[0].distance : (distance == Mathf.Infinity ? 10f : distance);
        CastVisualizer2D.StoreRay(rb.position, direction, dist, hit);
        return count;
    }

    public static int CircleCastVisualized(this Rigidbody2D rb, float radius, Vector2 direction, RaycastHit2D[] results, float distance = Mathf.Infinity, int layerMask = Physics2D.AllLayers)
    {
        int count = Physics2D.CircleCast(rb.position, radius, direction, new ContactFilter2D { layerMask = layerMask, useLayerMask = true }, results, distance);
        bool hit = count > 0;
        float dist = hit ? results[0].distance : (distance == Mathf.Infinity ? 10f : distance);
        CastVisualizer2D.StoreCircle(rb.position, radius, direction, dist, hit);
        return count;
    }

    public static int BoxCastVisualized(this Rigidbody2D rb, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance = Mathf.Infinity, int layerMask = Physics2D.AllLayers)
    {
        int count = Physics2D.BoxCast(rb.position, size, angle, direction, new ContactFilter2D { layerMask = layerMask, useLayerMask = true }, results, distance);
        bool hit = count > 0;
        float dist = hit ? results[0].distance : (distance == Mathf.Infinity ? 10f : distance);
        CastVisualizer2D.StoreBox(rb.position, size, angle, direction, dist, hit);
        return count;
    }

    public static int CapsuleCastVisualized(this Rigidbody2D rb, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, float distance = Mathf.Infinity, int layerMask = Physics2D.AllLayers)
    {
        int count = Physics2D.CapsuleCast(rb.position, size, capsuleDirection, angle, direction, new ContactFilter2D { layerMask = layerMask, useLayerMask = true }, results, distance);
        bool hit = count > 0;
        float dist = hit ? results[0].distance : (distance == Mathf.Infinity ? 10f : distance);
        CastVisualizer2D.StoreCapsule(rb.position, size, capsuleDirection, angle, direction, dist, hit);
        return count;
    }

    public static int CastVisualized(this Rigidbody2D rb, Vector2 direction, RaycastHit2D[] results, float distance = Mathf.Infinity)
    {
        int count = rb.Cast(direction, results, distance);
        bool hit = count > 0;
        float dist = hit ? results[0].distance : (distance == Mathf.Infinity ? 10f : distance);

        var col = rb.GetComponent<Collider2D>();
        if (col is CircleCollider2D circle)
        {
            float worldRadius = circle.radius * Mathf.Max(rb.transform.lossyScale.x, rb.transform.lossyScale.y);
            CastVisualizer2D.StoreCircle(rb.position + (Vector2)(rb.transform.rotation * circle.offset), worldRadius, direction, dist, hit);
        }
        else if (col is BoxCollider2D box)
        {
            Vector2 worldSize = Vector2.Scale(box.size, rb.transform.lossyScale);
            CastVisualizer2D.StoreBox(rb.position + (Vector2)(rb.transform.rotation * box.offset), worldSize, rb.rotation, direction, dist, hit);
        }
        else if (col is CapsuleCollider2D capsule)
        {
            Vector2 worldSize = Vector2.Scale(capsule.size, rb.transform.lossyScale);
            CastVisualizer2D.StoreCapsule(rb.position + (Vector2)(rb.transform.rotation * capsule.offset), worldSize, capsule.direction, rb.rotation, direction, dist, hit);
        }
        else
        {
            CastVisualizer2D.StoreRay(rb.position, direction, dist, hit);
        }

        return count;
    }
}
