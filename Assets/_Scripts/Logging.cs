using Module.EditorExtension.Runtime.VisualDebugger;
using UnityEngine;

namespace _Scripts
{
    public class Logging : MonoBehaviour
    {
        private void Update()
        {
            var fps = 1f / Time.deltaTime;
            DebugLogger.Log("frame rate", fps.ToString("F1"));
        }
    }
}