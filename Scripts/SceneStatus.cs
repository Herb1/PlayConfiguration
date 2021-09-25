using System;
using System.IO;

namespace EditorPlayConfiguration
{
    [Serializable]
    public class SceneStatus
    {
        public string SceneName;
        public string ScenePath;
        public bool ActiveOnPlay;

        public SceneStatus(string scenePath, bool activeOnPlay)
        {
            SceneName = Path.GetFileNameWithoutExtension(scenePath);
            ScenePath = scenePath;
            ActiveOnPlay = activeOnPlay;
        }
    }
}