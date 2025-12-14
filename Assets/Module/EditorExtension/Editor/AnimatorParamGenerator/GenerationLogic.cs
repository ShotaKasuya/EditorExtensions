using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Module.EditorExtension.Editor.AnimatorParamGenerator
{
    public static class GenerationLogic
    {
        internal static void GenerateForGroup(AnimatorParamGeneratorSetting settings)
        {
            if (settings.outputFile == null)
            {
                Debug.LogWarning("AnimatorParamGenerator: Output file is not set.");
                return;
            }

            foreach (var settingGroup in settings.settingGroupList)
            {
                var outputPath = settings.outputFile;
                if (string.IsNullOrEmpty(outputPath)) return;


                var directory = Path.GetDirectoryName(outputPath);
                var filePath = Path.Combine(directory!, settingGroup.className + ".cs");


                var controller = settingGroup.controller;
                if (controller == null) return;


                var parameters = controller.parameters;

                var sb = new StringBuilder();
                sb.AppendLine("// Auto-generated on controller modification");
                sb.AppendLine("using UnityEngine;");
                sb.AppendLine();

                if (!string.IsNullOrWhiteSpace(settings.namespaceName))
                {
                    sb.AppendLine($"namespace {settings.namespaceName}");
                    sb.AppendLine("{");
                    sb.AppendLine($" public static class {settingGroup.className}");
                    sb.AppendLine(" {");
                    WriteGroupConstants(sb, parameters, 2);
                    sb.AppendLine(" }");
                }
                else
                {
                    sb.AppendLine($"public static class {settingGroup.className}");
                    sb.AppendLine("{");
                    WriteGroupConstants(sb, parameters, 1);
                }

                sb.AppendLine("}");


                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                AssetDatabase.ImportAsset(filePath);
                Debug.Log($"AnimatorParamGenerator: Regenerated {filePath}");
            }
        }


        private static void WriteGroupConstants(StringBuilder sb, AnimatorControllerParameter[] parameters, int indent)
        {
            foreach (var p in parameters)
            {
                sb.AppendLine(Ind(indent) +
                              $"public static readonly int {p.name.Replace(" ", "_")}Hash = Animator.StringToHash(\"{p.name}\");");
                sb.AppendLine();
            }
            return;

            static string Ind(int lv) => new string(' ', lv * 4);
        }
    }
}