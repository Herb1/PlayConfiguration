using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace EditorPlayConfiguration
{
    public static class PlayConfigurationUtils
    {
        private const string ResourcesPath = "Assets/Plugins/PlayConfiguration/Resources/";
        public const string PlayConfigurationName = "PlayConfigurationSettings";
        public const string PlayConfigurationNameExtension = ".asset";
        private const string _tempLoadedScenesListNames = "scenesLoadedOnPlay~";
        
        public static string GetResourcePath()
        {
            Directory.CreateDirectory(ResourcesPath);
            return ResourcesPath;
        }
        
        public static void StoreLoadedScenes(List<string> loadedScenePaths)
        {
            File.WriteAllLines(ResourcesPath + _tempLoadedScenesListNames, loadedScenePaths);
        }

        public static string[] LoadStoredScenes()
        {
            if (!File.Exists(ResourcesPath + _tempLoadedScenesListNames))
            {
                return null;
            }

            return File.ReadAllLines(ResourcesPath + _tempLoadedScenesListNames);
        }

        public static void DeleteTempData()
        {
            File.Delete(ResourcesPath + _tempLoadedScenesListNames);
        }
    }
}