using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using FilePathAttribute = Module.EditorExtension.Runtime.Attribute.FilePathAttribute;


namespace Module.EditorExtension.Editor.AnimatorParamGenerator
{
    internal class AnimatorParamGeneratorSetting : ScriptableObject
    {
        public string namespaceName;
        [FilePath] public string outputFile;
        public List<AnimatorSettingGroup> settingGroupList;

        public static AnimatorParamGeneratorSetting LoadOrCreate()
        {
            const string path = "Assets/AnimatorParamGeneratorSettings.asset";
            var asset = AssetDatabase.LoadAssetAtPath<AnimatorParamGeneratorSetting>(path);
            if (asset == null)
            {
                asset = CreateInstance<AnimatorParamGeneratorSetting>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
            }

            return asset;
        }
    }

    [Serializable]
    internal class AnimatorSettingGroup
    {
        public AnimatorController controller;
        public string className;
    }
}