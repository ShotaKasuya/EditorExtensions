using UnityEditor;
using UnityEngine;

namespace Module.EditorExtension.Editor.AnimatorParamGenerator
{
    /// <summary>
    /// AnimatorControllerで定義された`Parameter`の定数をフィールドに持つ
    /// 静的クラスを生成するエディターウィンドウ
    /// </summary>
    public class GeneratorWindow : EditorWindow
    {
        private AnimatorParamGeneratorSetting _settings;
        private Vector2 _scroll;

        [MenuItem("Tools/Animator Parameter Settings")]
        private static void Open()
        {
            var w = GetWindow<GeneratorWindow>("Animator Param Settings");
            w.minSize = new Vector2(480, 320);
        }


        private void OnEnable()
        {
            _settings = AnimatorParamGeneratorSetting.LoadOrCreate();
        }

        private void OnGUI()
        {
            if (_settings == null)
            {
                _settings = AnimatorParamGeneratorSetting.LoadOrCreate();
            }

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            EditorGUILayout.LabelField("Setting Asset", EditorStyles.boldLabel);
            _settings = (AnimatorParamGeneratorSetting)EditorGUILayout.ObjectField(
                _settings,
                typeof(AnimatorParamGeneratorSetting),
                false,
                null
            );

            if (GUILayout.Button("Generate"))
            {
                GenerationLogic.GenerateForGroup(_settings);
            }

            EditorGUILayout.EndScrollView();
        }
    }
}