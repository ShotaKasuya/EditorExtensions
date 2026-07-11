using System;
using System.Reflection;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine.Assemblies;

namespace Module.EditorExtension.Editor.HierarchyMarker
{
    [InitializeOnLoad]
    public static class MarkerHighlighter
    {
        public static string ColorPrefKey { get; } = "ColorPrefKey";

        private static Type hierarchyWindowType;
        private static FieldInfo bindViewItemField;
        private static FieldInfo unbindViewItemField;

        private static PropertyInfo rowContainerProperty;
        private static PropertyInfo nodeProperty;
        private static PropertyInfo handlerProperty;
        private static MethodInfo getGameObjectMethod;

        private static readonly HashSet<object> BoundItems = new HashSet<object>();

        static MarkerHighlighter()
        {
            // Legacy hierarchy support
            EditorApplication.hierarchyWindowItemByEntityIdOnGUI += HandleHierarchyWindowItemOnGUI;

            // New hierarchy support (Unity 6+) via reflection
            try
            {
                hierarchyWindowType = typeof(EditorWindow).Assembly.GetType("Unity.Hierarchy.Editor.HierarchyWindow");
                if (hierarchyWindowType != null)
                {
                    bindViewItemField = hierarchyWindowType.GetField("BindViewItem", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                    unbindViewItemField = hierarchyWindowType.GetField("UnbindViewItem", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

                    var hierarchyViewItemType = CurrentAssemblies.GetLoadedAssemblies() 
                        .FirstOrDefault(a => a.GetName().Name == "UnityEngine.HierarchyModule")
                        ?.GetType("Unity.Hierarchy.HierarchyViewItem");

                    
                    if (hierarchyViewItemType != null)
                    {
                        rowContainerProperty = hierarchyViewItemType.GetProperty("RowContainer", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        nodeProperty = hierarchyViewItemType.GetProperty("Node", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        handlerProperty = hierarchyViewItemType.GetProperty("Handler", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    }

                    var hierarchyGameObjectHandlerType = typeof(EditorWindow).Assembly
                        .GetType("Unity.Hierarchy.Editor.HierarchyGameObjectHandler");

                    if (hierarchyGameObjectHandlerType != null)
                    {
                        getGameObjectMethod = hierarchyGameObjectHandlerType.GetMethod("GetGameObject", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    }

                    if (bindViewItemField != null)
                    {
                        var current = (Delegate)bindViewItemField.GetValue(null);
                        var method = typeof(MarkerHighlighter).GetMethod("HandleBindViewItem", BindingFlags.Static | BindingFlags.NonPublic);
                        var ourDelegate = Delegate.CreateDelegate(bindViewItemField.FieldType, method);
                        bindViewItemField.SetValue(null, Delegate.Combine(current, ourDelegate));
                    }

                    if (unbindViewItemField != null)
                    {
                        var current = (Delegate)unbindViewItemField.GetValue(null);
                        var method = typeof(MarkerHighlighter).GetMethod("HandleUnbindViewItem", BindingFlags.Static | BindingFlags.NonPublic);
                        var ourDelegate = Delegate.CreateDelegate(unbindViewItemField.FieldType, method);
                        unbindViewItemField.SetValue(null, Delegate.Combine(current, ourDelegate));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error initializing Hierarchy Hook: " + ex);
            }
        }

        private static void HandleHierarchyWindowItemOnGUI(EntityId instanceID, Rect selectionRect)
        {
            GameObject obj = EditorUtility.EntityIdToObject(instanceID) as GameObject;
            if (obj != null && obj.CompareTag(MarkerCreator.MarkerTag))
            {
                var markerColor = LoadColor();
                EditorGUI.DrawRect(selectionRect, markerColor);
            }
        }

        private static void HandleBindViewItem(object viewItem)
        {
            BoundItems.Add(viewItem);
            UpdateItemStyle(viewItem);
        }

        private static void HandleUnbindViewItem(object viewItem)
        {
            BoundItems.Remove(viewItem);
            if (rowContainerProperty != null)
            {
                var rowContainer = rowContainerProperty.GetValue(viewItem) as VisualElement;
                if (rowContainer != null)
                {
                    rowContainer.style.backgroundColor = StyleKeyword.Null;
                }
            }
        }

        public static void UpdateAllStyles()
        {
            foreach (var viewItem in BoundItems)
            {
                UpdateItemStyle(viewItem);
            }
        }

        private static void UpdateItemStyle(object viewItem)
        {
            if (rowContainerProperty == null || nodeProperty == null || handlerProperty == null || getGameObjectMethod == null) return;

            var rowContainer = rowContainerProperty.GetValue(viewItem) as VisualElement;
            if (rowContainer == null) return;

            var handler = handlerProperty.GetValue(viewItem);
            if (handler != null && handler.GetType().FullName == "Unity.Hierarchy.Editor.HierarchyGameObjectHandler")
            {
                var node = nodeProperty.GetValue(viewItem);
                var go = getGameObjectMethod.Invoke(handler, new object[] { node }) as GameObject;
                if (go != null && go.CompareTag(MarkerCreator.MarkerTag))
                {
                    var markerColor = LoadColor();
                    rowContainer.style.backgroundColor = new StyleColor(markerColor);
                    return;
                }
            }

            rowContainer.style.backgroundColor = StyleKeyword.Null;
        }

        public static Color LoadColor()
        {
            if (EditorPrefs.HasKey(ColorPrefKey))
            {
                string html = EditorPrefs.GetString(ColorPrefKey);
                if (ColorUtility.TryParseHtmlString("#" + html, out var color))
                    return color;
            }

            return new Color(1f, 0.9f, 0.4f, 0.3f); // fallback
        }
    }
}