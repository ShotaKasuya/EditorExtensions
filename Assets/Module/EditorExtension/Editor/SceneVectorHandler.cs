using Module.EditorExtension.Runtime;
using System.Reflection;
using Module.EditorExtension.Runtime.Attribute;
using UnityEditor;
using UnityEngine;

namespace Module.EditorExtension.Editor
{
    [InitializeOnLoad]
    public static class SceneVectorHandler
    {
        static SceneVectorHandler()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (Selection.activeGameObject == null)
            {
                return;
            }

            var components = Selection.activeGameObject.GetComponents<Component>();

            foreach (var component in components)
            {
                if (component == null)
                {
                    continue;
                }

                var serializedObject = new SerializedObject(component);
                var propertyIterator = serializedObject.GetIterator();
                
                while (propertyIterator.NextVisible(true))
                {
                    var field = component.GetType().GetField(propertyIterator.name,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    if (field != null)
                    {
                        var attributes = field.GetCustomAttributes(typeof(SceneVectorAttribute), true);
                        if (attributes.Length > 0)
                        {
                            var sceneVectorAttr = attributes[0] as SceneVectorAttribute;
                            if (propertyIterator.propertyType == SerializedPropertyType.Vector2 ||
                                propertyIterator.propertyType == SerializedPropertyType.Vector3)
                            {
                                DrawHandle(propertyIterator.Copy(), component.transform, sceneVectorAttr.Space);
                            }
                        }
                    }
                }
            }
        }

        private static void DrawHandle(SerializedProperty property, Transform transform, CoordinateSpace space)
        {
            Vector3 point = property.propertyType == SerializedPropertyType.Vector3
                ? property.vector3Value
                : (Vector2)property.vector2Value;

            Vector3 worldPosition = (space == CoordinateSpace.Local) ? transform.TransformPoint(point) : point;

            Handles.color = Color.yellow;
            float handleSize = HandleUtility.GetHandleSize(worldPosition) * 0.1f;
            Handles.Label(worldPosition + Vector3.up * handleSize, property.displayName);

            EditorGUI.BeginChangeCheck();
            worldPosition = Handles.PositionHandle(worldPosition, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(property.serializedObject.targetObject, "Move Scene Vector");

                Vector3 newPosition = (space == CoordinateSpace.Local)
                    ? transform.InverseTransformPoint(worldPosition)
                    : worldPosition;

                if (property.propertyType == SerializedPropertyType.Vector3)
                {
                    property.vector3Value = newPosition;
                }
                else
                {
                    property.vector2Value = newPosition;
                }

                property.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
        }
    }
}

