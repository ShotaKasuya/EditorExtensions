using UnityEngine;

namespace Module.EditorExtension.Runtime.Attribute
{
    public class FilePathAttribute: PropertyAttribute
    {
        public string Extension { get; }
        public string DefaultName { get; }

        public FilePathAttribute(string extension = "cs", string defaultName = "NewFile")
        {
            Extension = extension;
            DefaultName = defaultName;
        }
    }
}