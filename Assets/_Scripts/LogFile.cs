using Module.EditorExtension.Runtime.Attribute;
using UnityEngine;

namespace _Scripts
{
    public class LogFile:MonoBehaviour
    {
        [SerializeField, FolderPathSelector] private string folder;

        private void Start()
        {
            Debug.Log(folder);
        }
    }
}