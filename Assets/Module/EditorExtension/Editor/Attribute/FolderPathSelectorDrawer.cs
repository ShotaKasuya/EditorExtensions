using System.IO;
using Module.EditorExtension.Runtime.Attribute;
using UnityEditor;
using UnityEngine;

namespace Module.EditorExtension.Editor.Attribute
{
    [CustomPropertyDrawer(typeof(FolderPathSelectorAttribute))]
    public class FolderPathSelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // テキストフィールドとボタンの分割
            const int buttonWidth = 22;
            var textRect = new Rect(position.x, position.y, position.width - (buttonWidth + 3), position.height);
            var buttonRect = new Rect(position.x + position.width - buttonWidth,
                position.y, buttonWidth, position.height
            );

            // テキストフィールド表示
            EditorGUI.PropertyField(textRect, property, label);

            if (GUI.Button(buttonRect, "…"))
            {
                var defaultFile = property.stringValue;
                // デフォルトファイル名決定
                if (string.IsNullOrEmpty(defaultFile))
                {
                    defaultFile = "./";
                }

                var directory = Path.GetDirectoryName(defaultFile);
                if (string.IsNullOrEmpty(directory))
                {
                    directory = Application.dataPath;
                }

                var folderPath = EditorUtility.OpenFolderPanel(
                    "Select Folder",
                    directory,
                    string.Empty
                );

                if (!string.IsNullOrEmpty(folderPath))
                {
                    // Assets/ への変換
                    if (folderPath.StartsWith(Application.dataPath))
                    {
                        folderPath = "Assets/" + folderPath.Substring(Application.dataPath.Length + 1);
                    }

                    property.stringValue = folderPath;
                }
            }
        }
    }
}