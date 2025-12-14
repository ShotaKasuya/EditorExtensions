using UnityEngine;

namespace Module.EditorExtension.Runtime.VisualDebugger
{
    public static class DebugLogger
    {
        /// <summary>
        /// Displays a key-value pair on the screen for debugging.
        /// Only active in the Editor and Development builds.
        /// </summary>
        /// <param name="key">The label for the value.</param>
        /// <param name="value">The value to display.</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        public static void Log(string key, string value)
        {
            if (!Application.isPlaying) return;
            DebugDisplay.Instance.UpdateLog(key, value);
        }
    }
}