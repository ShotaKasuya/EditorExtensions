using System.IO;
using UnityEditor;
using UnityEngine;
using FilePathAttribute = Module.EditorExtension.Runtime.Attribute.FilePathAttribute;

namespace Module.EditorExtension.Editor.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(FilePathAttribute))]
    public class FilePathDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (FilePathAttribute)attribute;

            // ラベル部分
            position = EditorGUI.PrefixLabel(position, label);

            // テキストフィールドとボタンの分割
            Rect textRect = new Rect(position.x, position.y, position.width - 25, position.height);
            Rect buttonRect = new Rect(position.x + position.width - 22, position.y, 22, position.height);

            // テキストフィールド表示
            EditorGUI.PropertyField(textRect, property, GUIContent.none);

            // ボタン
            if (GUI.Button(buttonRect, "…"))
            {
                string defaultFile = property.stringValue;

                // デフォルトファイル名決定
                if (string.IsNullOrEmpty(defaultFile))
                {
                    defaultFile = $"{attr.DefaultName}.{attr.Extension}";
                }

                string directory = Path.GetDirectoryName(defaultFile);
                if (string.IsNullOrEmpty(directory))
                {
                    directory = Application.dataPath;
                }

                // SaveFilePanel を開く
                var filePath = EditorUtility.SaveFilePanel(
                    "Select File",
                    directory,
                    Path.GetFileName(defaultFile),
                    attr.Extension
                );

                if (!string.IsNullOrEmpty(filePath))
                {
                    // Assets/ への変換
                    if (filePath.StartsWith(Application.dataPath))
                    {
                        filePath = "Assets/" + filePath.Substring(Application.dataPath.Length + 1);
                    }

                    property.stringValue = filePath;
                }
            }
        }
    }
}