using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Module.EditorExtension.Runtime.VisualDebugger.PhysicsCast;

public class CastVisualizer : MonoBehaviour
{

    #region Property

    private static CastVisualizer _instance;

    private const int PoolSize = 128;
    private List<RayRecord> RayRecordPool { get; } = new(PoolSize);
    private List<SphereRecord> SphereRecordPool { get; } = new(PoolSize);

    #endregion

    private static CastVisualizer UseInstance()
    {
        if (_instance is not null) return _instance;

        _instance = FindFirstObjectByType<CastVisualizer>();

        if (_instance is not null) return _instance;

        var go = new GameObject(nameof(CastVisualizer));
        _instance = go.AddComponent<CastVisualizer>();
        return _instance;
    }

    #region API

    [UsedImplicitly]
    [System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void StoreRay(Ray ray, float checkDistance, bool isHit)
    {
        var common = new CommonRecord(ray, checkDistance, isHit);
        var rayRecord = new RayRecord(common);
        var recordPool = UseInstance().RayRecordPool;

        if (recordPool.Contains(rayRecord))
        {
            return;
        }

        recordPool.Add(rayRecord);
    }

    [UsedImplicitly]
    [System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void StoreSphere(Ray ray, float checkDistance, float radius, bool isHit)
    {
        var common = new CommonRecord(ray, checkDistance, isHit);
        var sphereRecord = new SphereRecord(radius, common);
        var recordPool = UseInstance().SphereRecordPool;

        if (recordPool.Contains(sphereRecord))
        {
            return;
        }

        recordPool.Add(sphereRecord);
    }

    #endregion

    #region ManageLifeTime

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    #endregion


    #region DrawLogic

    private void OnDrawGizmos()
    {
        var instance = UseInstance();

        foreach (var record in instance.RayRecordPool)
        {
            Draw(record);
        }

        foreach (var record in instance.SphereRecordPool)
        {
            Draw(record);
        }

        ClearRecord();
    }

    private void ClearRecord()
    {
        RayRecordPool.Clear();
        SphereRecordPool.Clear();
    }

    [UsedImplicitly]
    internal static void Draw(in RayRecord rayRecord)
    {
        var common = rayRecord.CommonRecord;

        Gizmos.color = common.IsHit ? Color.red : Color.green;

        var start = common.Ray.origin;
        var end = start + common.Ray.direction * common.CheckDistance;

        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, 0.05f);
    }

    [UsedImplicitly]
    internal static void Draw(in SphereRecord sphereRecord)
    {
        var common = sphereRecord.CommonRecord;

        Gizmos.color = common.IsHit
            ? new Color(1f, 0.4f, 0.4f)
            : new Color(0.4f, 1f, 0.4f);

        var start = common.Ray.origin;
        var end = start + common.Ray.direction * common.CheckDistance;

        var radius = sphereRecord.Radius;

        Gizmos.DrawWireSphere(start, radius);
        Gizmos.DrawWireSphere(end, radius);

        // ===== Capsule用の4本ライン =====
        var forward = common.Ray.direction.normalized;

        // forward に直交する2軸を生成
        var right = Vector3.Cross(forward, Vector3.up);
        if (right.sqrMagnitude < 1e-6f)
            right = Vector3.Cross(forward, Vector3.right);

        right.Normalize();
        var up = Vector3.Cross(right, forward).normalized;

        // 4方向
        Gizmos.DrawLine(start + right * radius, end + right * radius);
        Gizmos.DrawLine(start - right * radius, end - right * radius);
        Gizmos.DrawLine(start + up * radius, end + up * radius);
        Gizmos.DrawLine(start - up * radius, end - up * radius);
    }

    #endregion

}