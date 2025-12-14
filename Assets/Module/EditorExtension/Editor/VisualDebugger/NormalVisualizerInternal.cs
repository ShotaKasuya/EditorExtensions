using Module.EditorExtension.Runtime.VisualDebugger;
using UnityEditor;
using UnityEngine;

namespace Module.EditorExtension.Editor.VisualDebugger
{
    [InitializeOnLoad]
    public class NormalVisualizerInternal
    {
        static NormalVisualizerInternal()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        // ********************
        // GUI描画
        // ********************

        private static void OnSceneGUI(SceneView scene)
        {
            Handles.BeginGUI();

            const int padding = 10;
            const int spacing = 5;

            int index = 0;
            foreach (var (id, normal) in NormalVisualizer.Normals)
            {
                // ▼ テキストサイズを計測
                GUIStyle style = GUI.skin.label;
                float slope = Vector3.Angle(normal, Vector3.up);

                Vector2 idSize = style.CalcSize(new GUIContent(id));
                Vector2 angleSize = style.CalcSize(new GUIContent($"{slope:F1}°"));

                float textWidth = Mathf.Max(idSize.x, angleSize.x);
                float textHeight = idSize.y + angleSize.y;

                // ▼ 円＋矢印部分の最低サイズ
                float iconSize = 70f;

                float boxWidth = Mathf.Max(iconSize, textWidth + 10f);
                float boxHeight = iconSize + textHeight + 10f;

                // ▼ ボックス配置（右上に縦積み）
                Rect rect = new Rect(
                    Screen.width - boxWidth - padding,
                    padding + index * (boxHeight + spacing),
                    boxWidth,
                    boxHeight
                );

                GUI.Box(rect, GUIContent.none);

                GUILayout.BeginArea(rect);

                DrawNormalInfo(id, normal, style, boxWidth, boxHeight);

                GUILayout.EndArea();

                index++;
            }

            Handles.EndGUI();
        }

        private static void DrawNormalInfo(string id, Vector3 normal, GUIStyle style, float width, float height)
        {
            float slope = Vector3.Angle(normal, Vector3.up);

            // ▼ IDと角度を表示
            GUILayout.Label(id, style);
            GUILayout.Label($"{slope:F1}°", style);

            // ▼ 円の中心（テキスト下に表示）
            float centerY = height - 50f; // 矢印円の基準位置
            Vector2 center = new Vector2(width / 2f, centerY);
            const float radius = 30f;

            // ▼ 円描画
            Handles.color = Color.gray;
            Handles.DrawWireDisc(center, Vector3.forward, radius);

            // ▼ 法線をXZ平面へ投影
            var planeWidth = new Vector2(normal.x, normal.z).magnitude;
            var planeLeft = center + new Vector2(normal.y, planeWidth) * radius;
            var planeRight = center + new Vector2(-normal.y, -planeWidth) * radius;

            // ▼ 矢印線
            Handles.color = Color.white;
            Handles.DrawLine(planeLeft, planeRight);
        }
    }
}