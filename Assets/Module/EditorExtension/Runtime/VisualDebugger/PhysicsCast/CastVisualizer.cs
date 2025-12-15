using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Module.EditorExtension.Runtime.VisualDebugger.PhysicsCast;

[DefaultExecutionOrder(-100)]
public class CastVisualizer : MonoBehaviour
{
    private static CastVisualizer _instance;

    internal static CastVisualizer UseInstance()
    {
        if (_instance is not null) return _instance;

        _instance = FindFirstObjectByType<CastVisualizer>();

        if (_instance is not null) return _instance;

        var go = new GameObject(nameof(CastVisualizer));
        _instance = go.AddComponent<CastVisualizer>();
        return _instance;
    }

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

    private const int PoolSize = 128;
    internal List<RayRecord> RayRecordPool { get; } = new(PoolSize);
    internal List<SphereRecord> SphereRecordPool { get; } = new(PoolSize);

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

    private void Update()
    {
        RayRecordPool.Clear();
        SphereRecordPool.Clear();
    }
}