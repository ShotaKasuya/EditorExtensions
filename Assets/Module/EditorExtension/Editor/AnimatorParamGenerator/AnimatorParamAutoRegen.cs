using UnityEditor;

namespace Module.EditorExtension.Editor.AnimatorParamGenerator
{
    public class AnimatorParamAutoRegen : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved,
            string[] movedFrom)
        {
            var settings = AnimatorParamGeneratorSetting.LoadOrCreate();
            if (settings == null || settings.settingGroupList == null) return;

            GenerationLogic.GenerateForGroup(settings);
        }
    }
}