using JetBrains.Annotations;
using UnityEngine;

namespace Module.EditorExtension.Runtime.VisualDebugger.PhysicsCast;

[UsedImplicitly]
public static class Rigidbody2DExtensions
{
    [UsedImplicitly]
    public static int RaycastVisualized(this Rigidbody2D rb, Vector2 direction, RaycastHit2D[] results, float distance = Mathf.Infinity, int layerMask = Physics2D.AllLayers)
    {
        return rb.RaycastVisualized(direction, new ContactFilter2D { layerMask = layerMask, useLayerMask = true }, results, distance);
    }

    [UsedImplicitly]
    public static int RaycastVisualized(this Rigidbody2D rb, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, float distance = Mathf.Infinity)
    {
        int count = rb.Cast(direction, contactFilter, results, distance);
        bool hit = count > 0;
        float dist = hit ? results[0].distance : (float.IsPositiveInfinity(distance) ? 10f : distance);
        CastVisualizer2D.StoreRay(rb.position, direction, dist, hit);
        return count;
    }

    [UsedImplicitly]
    public static int CircleCastVisualized(this Rigidbody2D rb, float radius, Vector2 direction, RaycastHit2D[] results, float distance = Mathf.Infinity, int layerMask = Physics2D.AllLayers)
    {
        return rb.CircleCastVisualized(radius, direction, new ContactFilter2D { layerMask = layerMask, useLayerMask = true }, results, distance);
    }

    [UsedImplicitly]
    public static int CircleCastVisualized(this Rigidbody2D rb, float radius, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, float distance = Mathf.Infinity)
    {
        int count = Physics2D.CircleCast(rb.position, radius, direction, contactFilter, results, distance);
        bool hit = count > 0;
        float dist = hit ? results[0].distance : (float.IsPositiveInfinity(distance) ? 10f : distance);
        CastVisualizer2D.StoreCircle(rb.position, radius, direction, dist, hit);
        return count;
    }

    [UsedImplicitly]
    public static int BoxCastVisualized(this Rigidbody2D rb, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance = Mathf.Infinity, int layerMask = Physics2D.AllLayers)
    {
        return rb.BoxCastVisualized(size, angle, direction, new ContactFilter2D { layerMask = layerMask, useLayerMask = true }, results, distance);
    }

    [UsedImplicitly]
    public static int BoxCastVisualized(this Rigidbody2D rb, Vector2 size, float angle, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, float distance = Mathf.Infinity)
    {
        int count = Physics2D.BoxCast(rb.position, size, angle, direction, contactFilter, results, distance);
        bool hit = count > 0;
        float dist = hit ? results[0].distance : (float.IsPositiveInfinity(distance) ? 10f : distance);
        CastVisualizer2D.StoreBox(rb.position, size, angle, direction, dist, hit);
        return count;
    }

    [UsedImplicitly]
    public static int CapsuleCastVisualized(this Rigidbody2D rb, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, float distance = Mathf.Infinity, int layerMask = Physics2D.AllLayers)
    {
        return rb.CapsuleCastVisualized(size, capsuleDirection, angle, direction, new ContactFilter2D { layerMask = layerMask, useLayerMask = true }, results, distance);
    }

    [UsedImplicitly]
    public static int CapsuleCastVisualized(this Rigidbody2D rb, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, float distance = Mathf.Infinity)
    {
        int count = Physics2D.CapsuleCast(rb.position, size, capsuleDirection, angle, direction, contactFilter, results, distance);
        bool hit = count > 0;
        float dist = hit ? results[0].distance : (float.IsPositiveInfinity(distance) ? 10f : distance);
        CastVisualizer2D.StoreCapsule(rb.position, size, capsuleDirection, angle, direction, dist, hit);
        return count;
    }

    [UsedImplicitly]
    public static int CastVisualized(this Rigidbody2D rb, Collider2D col, Vector2 direction, RaycastHit2D[] results, float distance = Mathf.Infinity)
    {
        int count = rb.Cast(direction, results, distance);
        bool hit = count > 0;
        float dist = hit ? results[0].distance : (float.IsPositiveInfinity(distance) ? 10f : distance);
        StoreCastVisualization(rb, col, direction, dist, hit);
        return count;
    }

    [UsedImplicitly]
    public static int CastVisualized(this Rigidbody2D rb, Collider2D col, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, float distance = Mathf.Infinity)
    {
        int count = rb.Cast(direction, contactFilter, results, distance);
        bool hit = count > 0;
        float dist = hit ? results[0].distance : (float.IsPositiveInfinity(distance) ? 10f : distance);
        StoreCastVisualization(rb, col, direction, dist, hit);
        return count;
    }

    [UsedImplicitly]
    private static void StoreCastVisualization(Rigidbody2D rb, Collider2D col, Vector2 direction, float dist, bool hit)
    {
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
    }
}
