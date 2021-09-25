using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EditorPlayConfiguration
{
    [CreateAssetMenu(fileName = "PlayConfigurationSettings", menuName = "Play Configuration/Create PlayConfigurationSettings", order = 1)]
    public class PlayConfigurationSettings : ScriptableObject
    {
        private static PlayConfigurationSettings _instance;

        public List<SceneStatus> Scenes;

        public bool EnablePlayConfiguration = true;
        public bool RestoreScenesAfterPlayModeEnded = true;
        public bool SearchScenesInBuildOnly;
        public bool SearchOnlyInPath;
        public string ScenePath;

        public static PlayConfigurationSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GetInstance();
                }

                return _instance;
            }
        }

        private static PlayConfigurationSettings GetInstance()
        {
            var instance = Resources.Load<PlayConfigurationSettings>(PlayConfigurationUtils.PlayConfigurationName);

            if (instance == null)
            {
                instance = CreateInstance<PlayConfigurationSettings>();

                AssetDatabase.CreateAsset(instance,
                    Path.Combine(PlayConfigurationUtils.GetResourcePath(),
                        PlayConfigurationUtils.PlayConfigurationName +
                        PlayConfigurationUtils.PlayConfigurationNameExtension));

                Debug.Log("[PlayConfiguration] Creating settings asset at " + PlayConfigurationUtils.GetResourcePath() +
                          PlayConfigurationUtils.PlayConfigurationName);

                instance.Scenes = new List<SceneStatus>();
                instance.EnablePlayConfiguration = true;
                instance.RestoreScenesAfterPlayModeEnded = true;
            }

            return instance;
        }

        public void RebuildAllScenes()
        {
            ClearList();
            AppendNewScenes();
        }

        public void ClearList()
        {
            if (Scenes == null)
            {
                Scenes = new List<SceneStatus>();
            }
            else
            {

                Scenes.Clear();
            }
        }

        public void AppendNewScenes()
        {
            if (Scenes == null)
            {
                Scenes = new List<SceneStatus>();
            }

            if (SearchScenesInBuildOnly)
            {
                string[] scenes = new string[SceneManager.sceneCountInBuildSettings];

                for (int i = 0; i < scenes.Length; i++)
                {
                    scenes[i] = SceneUtility.GetScenePathByBuildIndex(i);
                }

                ApplyAppendToScenes(scenes);
            }
            else if (SearchOnlyInPath)
            {
                var searchedGUIDs = AssetDatabase.FindAssets("t:Scene", new string[] {ScenePath});
                var searchedPaths = searchedGUIDs.Select(AssetDatabase.GUIDToAssetPath).ToArray();

                ApplyAppendToScenes(searchedPaths);

            }
            else
            {
                var searchedGUIDs = AssetDatabase.FindAssets("t:Scene");
                var searchedPaths = searchedGUIDs.Select(AssetDatabase.GUIDToAssetPath).ToArray();

                ApplyAppendToScenes(searchedPaths);
            }
        }

        private void ApplyAppendToScenes(string[] scenePaths)
        {
            for (int i = 0; i < scenePaths.Length; i++)
            {
                var sceneName = Path.GetFileNameWithoutExtension(scenePaths[i]);

                if (!Scenes.Exists(t => t.SceneName == sceneName))
                {
                    Scenes.Add(new SceneStatus(scenePaths[i], false));
                }
            }
        }
    }
}