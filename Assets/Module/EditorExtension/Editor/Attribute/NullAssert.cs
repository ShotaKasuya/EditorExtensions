using System.Linq;
using System.Reflection;
using Module.EditorExtension.Runtime.Attribute;
using UnityEditor;
using UnityEngine;

namespace Module.EditorExtension.Editor.Attribute
{
    [InitializeOnLoad]
    public static class NullAssert
    {
        static NullAssert()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            if (stateChange != PlayModeStateChange.ExitingEditMode)
            {
                return;
            }

            if (ValidateAll())
            {
                // null を発見した場合、Playモード移行をキャンセル
                EditorApplication.isPlaying = false;
            }
        }

        private static bool ValidateAll()
        {
            var hasError = false;

            var behaviours = Object.FindObjectsByType<MonoBehaviour>();

            foreach (var monoBehaviour in behaviours)
            {
                var type = monoBehaviour.GetType();

                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(p => p.GetCustomAttribute<NullAssertAttribute>() != null);

                foreach (var fieldInfo in fields)
                {
                    if (fieldInfo.GetValue(monoBehaviour) == null)
                    {
                        Debug.LogError(fieldInfo.Name, monoBehaviour);
                        hasError = true;
                    }
                }

                var properties = type
                    .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(p => p.GetCustomAttribute<NullAssertAttribute>() != null);

                foreach (var propertyInfo in properties)
                {
                    if (propertyInfo.GetValue(monoBehaviour) == null)
                    {
                        Debug.LogError(propertyInfo.Name, monoBehaviour);
                        hasError = true;
                    }
                }
            }

            return hasError;
        }
    }
}