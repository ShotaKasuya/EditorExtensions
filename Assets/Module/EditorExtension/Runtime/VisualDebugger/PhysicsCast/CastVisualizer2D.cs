using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Module.EditorExtension.Runtime.VisualDebugger.PhysicsCast;

public class CastVisualizer2D : MonoBehaviour
{
    #region Property

    private static CastVisualizer2D _instance;

    private const int PoolSize = 128;
    private List<RayRecord> RayRecordPool { get; } = new(PoolSize);
    private List<CircleRecord> CircleRecordPool { get; } = new(PoolSize);
    private List<BoxRecord> BoxRecordPool { get; } = new(PoolSize);
    private List<CapsuleRecord2D> CapsuleRecordPool { get; } = new(PoolSize);

    #endregion

    private static CastVisualizer2D UseInstance()
    {
        if (_instance is not null) return _instance;
        _instance = FindFirstObjectByType<CastVisualizer2D>();
        if (_instance is not null) return _instance;

        var go = new GameObject(nameof(CastVisualizer2D));
        _instance = go.AddComponent<CastVisualizer2D>();
        return _instance;
    }

    #region API

    [UsedImplicitly]
    [System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void StoreRay(Vector2 origin, Vector2 direction, float distance, bool isHit)
    {
        var common = new CommonRecord(new Ray(origin, direction), distance, isHit);
        var record = new RayRecord(common);
        var pool = UseInstance().RayRecordPool;
        if (!pool.Contains(record)) pool.Add(record);
    }

    [UsedImplicitly]
    [System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void StoreCircle(Vector2 origin, float radius, Vector2 direction, float distance, bool isHit)
    {
        var common = new CommonRecord(new Ray(origin, direction), distance, isHit);
        var record = new CircleRecord(radius, common);
        var pool = UseInstance().CircleRecordPool;
        if (!pool.Contains(record)) pool.Add(record);
    }

    [UsedImplicitly]
    [System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void StoreBox(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, bool isHit)
    {
        var common = new CommonRecord(new Ray(origin, direction), distance, isHit);
        var record = new BoxRecord(size * 0.5f, Quaternion.Euler(0, 0, angle), common);
        var pool = UseInstance().BoxRecordPool;
        if (!pool.Contains(record)) pool.Add(record);
    }

    [UsedImplicitly]
    [System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void StoreCapsule(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, bool isHit)
    {
        var common = new CommonRecord(new Ray(origin, direction), distance, isHit);
        var record = new CapsuleRecord2D(size, capsuleDirection, angle, common);
        var pool = UseInstance().CapsuleRecordPool;
        if (!pool.Contains(record)) pool.Add(record);
    }

    #endregion

    #region DrawLogic

    private void OnDrawGizmos()
    {
        var instance = UseInstance();
        foreach (var r in instance.RayRecordPool) DrawRay(r);
        foreach (var r in instance.CircleRecordPool) DrawCircle(r);
        foreach (var r in instance.BoxRecordPool) DrawBox(r);
        foreach (var r in instance.CapsuleRecordPool) DrawCapsule(r);
        ClearRecord();
    }

    private void ClearRecord()
    {
        RayRecordPool.Clear();
        CircleRecordPool.Clear();
        BoxRecordPool.Clear();
        CapsuleRecordPool.Clear();
    }

    private void DrawRay(in RayRecord record)
    {
        Gizmos.color = record.CommonRecord.IsHit ? Color.red : Color.green;
        Vector3 start = record.CommonRecord.Ray.origin;
        Vector3 end = start + record.CommonRecord.Ray.direction * record.CommonRecord.CheckDistance;
        Gizmos.DrawLine(start, end);
    }

    private void DrawCircle(in CircleRecord record)
    {
        Gizmos.color = record.CommonRecord.IsHit ? new Color(1f, 0.4f, 0.4f) : new Color(0.4f, 1f, 0.4f);
        Vector3 start = record.CommonRecord.Ray.origin;
        Vector3 end = start + record.CommonRecord.Ray.direction * record.CommonRecord.CheckDistance;
        Gizmos.DrawWireSphere(start, record.Radius);
        Gizmos.DrawWireSphere(end, record.Radius);
        DrawConnectors(start, end, record.CommonRecord.Ray.direction, record.Radius);
    }

    private void DrawBox(in BoxRecord record)
    {
        Gizmos.color = record.CommonRecord.IsHit ? new Color(1f, 0.4f, 0.4f) : new Color(0.4f, 1f, 0.4f);
        Vector3 start = record.CommonRecord.Ray.origin;
        Vector3 end = start + record.CommonRecord.Ray.direction * record.CommonRecord.CheckDistance;
        
        var oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(start, record.Orientation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, record.HalfExtents * 2);
        Gizmos.matrix = Matrix4x4.TRS(end, record.Orientation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, record.HalfExtents * 2);
        Gizmos.matrix = oldMatrix;

        // Connect corners
        var right = record.Orientation * Vector3.right * record.HalfExtents.x;
        var up = record.Orientation * Vector3.up * record.HalfExtents.y;
        Gizmos.DrawLine(start + right + up, end + right + up);
        Gizmos.DrawLine(start + right - up, end + right - up);
        Gizmos.DrawLine(start - right + up, end - right + up);
        Gizmos.DrawLine(start - right - up, end - right - up);
    }

    private void DrawCapsule(in CapsuleRecord2D record)
    {
        Gizmos.color = record.CommonRecord.IsHit ? new Color(1f, 0.4f, 0.4f) : new Color(0.4f, 1f, 0.4f);
        Vector3 start = record.CommonRecord.Ray.origin;
        Vector3 end = start + record.CommonRecord.Ray.direction * record.CommonRecord.CheckDistance;
        Quaternion rot = Quaternion.Euler(0, 0, record.Angle);

        DrawCapsuleShape(start, record.Size, record.Direction, rot);
        DrawCapsuleShape(end, record.Size, record.Direction, rot);
        
        // Connectors (simplified)
        float radius = (record.Direction == CapsuleDirection2D.Vertical ? record.Size.x : record.Size.y) * 0.5f;
        DrawConnectors(start, end, record.CommonRecord.Ray.direction, radius);
    }

    private void DrawCapsuleShape(Vector3 pos, Vector2 size, CapsuleDirection2D dir, Quaternion rot)
    {
        var oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(pos, rot, Vector3.one);
        // Simple wireframe for capsule
        float radius = (dir == CapsuleDirection2D.Vertical ? size.x : size.y) * 0.5f;
        if (dir == CapsuleDirection2D.Vertical)
        {
            float side = Mathf.Max(0, size.y - size.x);
            Gizmos.DrawWireSphere(Vector3.up * side * 0.5f, radius);
            Gizmos.DrawWireSphere(Vector3.down * side * 0.5f, radius);
        }
        else
        {
            float side = Mathf.Max(0, size.x - size.y);
            Gizmos.DrawWireSphere(Vector3.right * side * 0.5f, radius);
            Gizmos.DrawWireSphere(Vector3.left * side * 0.5f, radius);
        }
        Gizmos.matrix = oldMatrix;
    }

    private void DrawConnectors(Vector3 start, Vector3 end, Vector3 dir, float radius)
    {
        Vector3 side = new Vector3(-dir.y, dir.x, 0).normalized * radius;
        Gizmos.DrawLine(start + side, end + side);
        Gizmos.DrawLine(start - side, end - side);
    }

    #endregion
}
