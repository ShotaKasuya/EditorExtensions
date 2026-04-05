using UnityEngine;

namespace Module.EditorExtension.Runtime.Attribute
{
    public class FilePathSelectorAttribute : PropertyAttribute
    {
        public string Extension { get; }
        public string DefaultName { get; }

        public FilePathSelectorAttribute(string extension = "cs", string defaultName = "NewFile")
        {
            Extension = extension;
            DefaultName = defaultName;
        }
    }
}