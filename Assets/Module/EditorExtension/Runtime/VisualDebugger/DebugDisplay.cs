using System;
using System.Collections.Generic;
using UnityEngine;

namespace Module.EditorExtension.Runtime.VisualDebugger
{
    internal class DebugDisplay : MonoBehaviour
    {
        private static DebugDisplay _instance;

        public static DebugDisplay Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<DebugDisplay>();
                    if (_instance == null)
                    {
                        var go = new GameObject("DebugDisplay");
                        _instance = go.AddComponent<DebugDisplay>();
                    }
                }

                return _instance;
            }
        }

        private readonly Dictionary<string, string> _logs = new Dictionary<string, string>();
        private GUIStyle _style;
        private Texture2D _backgroundTexture;

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

        private void OnEnable()
        {
            _style = new GUIStyle
            {
                fontSize = 16,
                normal = { textColor = Color.white }
            };

            _backgroundTexture = new Texture2D(1, 1);
            _backgroundTexture.SetPixel(0, 0, new Color(0.1f, 0.1f, 0.1f, 0.7f));
            _backgroundTexture.Apply();
        }

        public void UpdateLog(string key, string value)
        {
            if (_logs.ContainsKey(key))
            {
                _logs[key] = value;
            }
            else
            {
                _logs.Add(key, value);
            }
        }

        private void OnGUI()
        {
            if (_logs.Count == 0) return;

            float yPos = 5f;
            float xPos = 5f;
            float width = 0f;
            float height = (_logs.Count * 20f) + 4f;

            foreach (var log in _logs)
            {
                var content = new GUIContent($"{log.Key}: {log.Value}");
                var size = _style.CalcSize(content);
                if (size.x > width)
                {
                    width = size.x;
                }
            }

            width += 10; // padding

            var bgRect = new Rect(xPos - 2, yPos - 2, width + 4, height);
            GUI.DrawTexture(bgRect, _backgroundTexture);

            foreach (var log in _logs)
            {
                var content = new GUIContent($"{log.Key}: {log.Value}");
                GUI.Label(new Rect(xPos, yPos, width, 20f), content, _style);
                yPos += 20f;
            }
        }
    }
}