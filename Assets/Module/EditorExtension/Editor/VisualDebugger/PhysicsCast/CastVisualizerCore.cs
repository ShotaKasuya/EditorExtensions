using Module.EditorExtension.Runtime.VisualDebugger.PhysicsCast;
using UnityEditor;
using UnityEngine;

namespace Module.EditorExtension.Editor.VisualDebugger.PhysicsCast;

[InitializeOnLoad]
public static class CastVisualizerCore
{
    // todo ゲームプレイ中のみ描画
    static CastVisualizerCore()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }
        else if (state == PlayModeStateChange.ExitingPlayMode)
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }
    }

    // ********************
    // GUI描画
    // ********************

    private static void OnSceneGUI(SceneView scene)
    {
        var rayRecord = CastVisualizer.UseInstance().RayRecordPool;
        var sphereRecord = CastVisualizer.UseInstance().SphereRecordPool;

        foreach (var record in rayRecord)
        {
            Draw(record);
        }

        foreach (var record in sphereRecord)
        {
            Draw(record);
        }
    }


    private static void Draw(in RayRecord rayRecord)
    {
        var common = rayRecord.CommonRecord;

        var ray = common.Ray;
        var distance = common.CheckDistance;

        var prev = Handles.color;
        Handles.color = common.IsHit ? Color.red : Color.green;

        var end = ray.origin + ray.direction * distance;

        Handles.DrawLine(ray.origin, end);
        Handles.SphereHandleCap(
            0,
            end,
            Quaternion.identity,
            HandleUtility.GetHandleSize(end) * 0.05f,
            EventType.Repaint
        );

        Handles.color = prev;
    }

    private static void Draw(in SphereRecord sphereRecord)
    {
        var common = sphereRecord.CommonRecord;

        var ray = common.Ray;
        var distance = common.CheckDistance;
        var radius = sphereRecord.Radius;

        Color prev = Handles.color;
        Handles.color = common.IsHit ? new Color(1f, 0.4f, 0.4f) : new Color(0.4f, 1f, 0.4f);

        Vector3 start = ray.origin;
        Vector3 end = ray.origin + ray.direction * distance;

        // 始点球
        Handles.SphereHandleCap(
            0,
            start,
            Quaternion.identity,
            radius * 2f,
            EventType.Repaint
        );

        // 終点球
        Handles.SphereHandleCap(
            0,
            end,
            Quaternion.identity,
            radius * 2f,
            EventType.Repaint
        );

        // 中央ライン
        Handles.DrawLine(start, end);

        Handles.color = prev;
    }
}