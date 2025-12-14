using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Module.EditorExtension.Runtime.VisualDebugger
{
    public static class NormalVisualizer
    {
        internal static Dictionary<string, Vector3> Normals { get; } = new();
        // ********************
        // 外部 API
        // ********************

        [UsedImplicitly]
        public static void SetNormal(string id, Vector3 normal)
        {
            Normals[id] = normal.normalized;
        }

        [UsedImplicitly]
        public static void Remove(string id)
        {
            Normals.Remove(id);
        }

        [UsedImplicitly]
        public static void Clear()
        {
            Normals.Clear();
        }
    }
}