using System.Collections;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Module.EditorExtension.Editor.ToolBar
{
    [UsedImplicitly]
    public static class InitCheckToolbarButton
    {
        /// <summary>
        /// PlayModeを維持するフレーム数
        /// </summary>
        private const int CheckFrameCount = 5;

        private const string ID = Constant.Path + "InitCheck";

        [MainToolbarElement(ID)]
        public static MainToolbarElement CreateButton()
        {
            var icon = EditorGUIUtility.IconContent("console.erroricon.inactive.sml").image as Texture2D;
            var content = new MainToolbarContent(icon, tooltip: $"初期化チェック ({CheckFrameCount}フレーム語に終了");
            return new MainToolbarButton(content, OnButtonClicked);
        }

        private static void OnButtonClicked()
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning("[InitCheck] already in PlayMode");
                return;
            }

            Debug.Log("[InitCheck] start PlayMode");

            EditorApplication.EnterPlaymode();

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                EditorCoroutineHelper.Start(WaitFramesThenExit(CheckFrameCount));
            }
        }

        private static IEnumerator WaitFramesThenExit(int frames)
        {
            for (int i = 0; i < frames; i++)
            {
                yield return null;
                Debug.Log($"[Init Check] {i + 1} / {frames} frame");
            }

            Debug.Log("[Init Check] end PlayMode");
            EditorApplication.ExitPlaymode();
        }
    }

    internal static class EditorCoroutineHelper
    {
        private static IEnumerator _current;

        public static void Start(IEnumerator coroutine)
        {
            _current = coroutine;
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
        }

        private static void Tick()
        {
            if (_current == null)
            {
                EditorApplication.update -= Tick;
                return;
            }

            if (!_current.MoveNext())
            {
                _current = null;
                EditorApplication.update -= Tick;
            }
        }
    }
}