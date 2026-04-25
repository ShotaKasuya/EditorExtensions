using System.IO;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Module.EditorExtension.Editor.ToolBar
{
    [UsedImplicitly]
    public class SceneSelectToolbar
    {
        private const string ID = Constant.Path + "Scene Select";

        private static string[] _scenePathList;

        [UsedImplicitly]
        [MainToolbarElement(ID, defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement CreateSceneSelectDropdown()
        {
            string activeSceneName = Application.isPlaying switch
            {
                true => SceneManager.GetActiveScene().name,
                false => EditorSceneManager.GetActiveScene().name,
            };

            var icon = EditorGUIUtility.IconContent("UnityLogo").image as Texture2D;
            var context = new MainToolbarContent(activeSceneName, icon, "Select Active Scene");
            return new MainToolbarDropdown(context, ShowDropdownMenu);
        }

        private static void ShowDropdownMenu(Rect dropDownRect)
        {
            var menu = new GenericMenu();
            if (_scenePathList.Length == 0)
            {
                menu.AddDisabledItem(new GUIContent("No Scene in Project"));
            }

            foreach (var scenePath in _scenePathList)
            {
                var sceneName = Path.GetFileNameWithoutExtension(scenePath);
                menu.AddItem(new GUIContent(sceneName), false, () => SwitchScene(scenePath));
            }

            menu.DropDown(dropDownRect);
        }

        private static void SwitchScene(string sceneName)
        {
            if (Application.isPlaying)
            {
                Debug.LogError("Please End PlayMode");
                return;
            }

            if (!File.Exists(sceneName))
            {
                Debug.LogError($"Scene at path '{sceneName}' does not exist.");
                return;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            Debug.Log($"Switching to scene: {sceneName}");
            EditorSceneManager.OpenScene(sceneName);
        }

        private static void RefreshSceneList()
        {
            _scenePathList = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories);
        }

        private static void SceneSwitched(Scene oldScene, Scene newScene)
        {
            MainToolbar.Refresh(ID);
        }

        static SceneSelectToolbar()
        {
            RefreshSceneList();
            EditorApplication.projectChanged += RefreshSceneList;
            SceneManager.activeSceneChanged += SceneSwitched;
            EditorSceneManager.activeSceneChangedInEditMode += SceneSwitched;
        }
    }
}