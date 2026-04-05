using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Module.EditorExtension.Editor.ToolBar
{
    [UsedImplicitly]
    public class TimeScaleToolbarSlider
    {
        private const string ID = "CustomUtility/TimeScaleSlider";
        private const float MinTimeScale = 0f;
        private const float MaxTimeScale = 2f;

        [UsedImplicitly]
        [MainToolbarElement(ID, defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarSlider TimeSlider()
        {
            // 仮アイコン
            var icon = EditorGUIUtility.IconContent("UnityEditor.ProfilerWindow").image as Texture2D;
            var context = new MainToolbarContent("Time Scale", icon, "Time Scale");
            var slider =
                new MainToolbarSlider(context, Time.timeScale, MinTimeScale, MaxTimeScale, OnSliderValueChanged);

            slider.populateContextMenu = menu =>
            {
                menu.AppendAction("Reset", _ =>
                {
                    Time.timeScale = 1f;
                    MainToolbar.Refresh(ID);
                });
            };

            return slider;
        }

        private static void OnSliderValueChanged(float value)
        {
            Time.timeScale = value;
        }
    }
}