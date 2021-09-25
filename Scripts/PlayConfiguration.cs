using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EditorPlayConfiguration
{
    public static class PlayConfiguration
    {
        private static PlayConfigurationSettings _configurationSettings;

        [InitializeOnLoadMethod]
        private static void OnInitLoad()
        {
            EditorApplication.playModeStateChanged += LogPlayModeState;
            _configurationSettings = PlayConfigurationSettings.Instance;
        }

        [MenuItem("Tools/Play Configuration/Show Settings", false)]
        private static void ShowSettings()
        {
            if (_configurationSettings == null)
            {
                _configurationSettings = PlayConfigurationSettings.Instance;
            }

            Selection.SetActiveObjectWithContext(_configurationSettings, _configurationSettings);
        }

        [MenuItem("Tools/Play Configuration/Update scenes status", false)]
        private static void UpdateStatus()
        {
            UpdateSceneStatusOnEnterPlayMode(_configurationSettings.Scenes);
        }

        [MenuItem("Tools/Play Configuration/Load all scenes", false)]
        private static void LoadAll()
        {
            LoadAllScenes();
        }

        private static void LogPlayModeState(PlayModeStateChange state)
        {
            if (!_configurationSettings.EnablePlayConfiguration)
            {
                return;
            }

            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                    UpdateSceneStatusOnEnterPlayMode(_configurationSettings.Scenes);

                    break;

                case PlayModeStateChange.EnteredEditMode:
                {
                    var scenesPaths = PlayConfigurationUtils.LoadStoredScenes();
                    PlayConfigurationUtils.DeleteTempData();

                    if (scenesPaths == null)
                    {
                        break;
                    }

                    if (_configurationSettings.RestoreScenesAfterPlayModeEnded && scenesPaths != null)
                    {
                        for (int i = 0; i < scenesPaths.Length; i++)
                        {
                            EditorSceneManager.OpenScene(scenesPaths[i], OpenSceneMode.Additive);
                        }
                    }
                    break;
                }
            }

        }

        private static void UpdateSceneStatusOnEnterPlayMode(List<SceneStatus> sceneStatus)
        {
            var activeScenes = sceneStatus.Where(a => a.ActiveOnPlay).ToList();
            var inActiveScenes = sceneStatus.Where(a => !a.ActiveOnPlay).ToList();

            List<Scene> editorScenes = new List<Scene>();
            List<string> loadedScenePaths = new List<string>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                editorScenes.Add(scene);

                if (scene.isLoaded)
                {
                    loadedScenePaths.Add(scene.path);
                }
                if (scene.isDirty)
                {
                    EditorSceneManager.SaveScene(scene);
                }
            }

            PlayConfigurationUtils.StoreLoadedScenes(loadedScenePaths);

            for (int i = 0; i < activeScenes.Count; i++)
            {
                if (editorScenes.Any(t => t.name == activeScenes[i].SceneName))
                {
                    var scene = editorScenes.First(t => t.name == activeScenes[i].SceneName);
                    EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Additive);
                }
            }

            for (int i = 0; i < inActiveScenes.Count; i++)
            {
                if (editorScenes.Any(t => t.name == inActiveScenes[i].SceneName))
                {
                    var scene = editorScenes.First(t => t.name == inActiveScenes[i].SceneName);
                    var closeSuccess = EditorSceneManager.CloseScene(scene, false);

                    if (!closeSuccess)
                    {
                        Debug.LogWarning("[PlayConfiguration] Closing " +
                                         editorScenes.First(t => t.name == inActiveScenes[i].SceneName).path +
                                         " failed. Please note that you need at least one open scene in the editor. Check your settings.");
                    }
                }
            }
        }

        private static void LoadAllScenes()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                EditorSceneManager.OpenScene(SceneManager.GetSceneAt(i).path, OpenSceneMode.Additive);
            }
        }

    }
}