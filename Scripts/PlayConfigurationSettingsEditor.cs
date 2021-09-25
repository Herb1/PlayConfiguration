using UnityEditor;
using UnityEngine;

namespace EditorPlayConfiguration
{
    [CustomEditor(typeof(PlayConfigurationSettings))]
    public class PlayConfigurationSettingsEditor : Editor
    {
        private const string EnablePlayConfigurationInfo = "Enable PlayConfiguration";
        private const string RestoreScenesAfterPlayInfo = "Restore scenes after play mode ended";
        private const string SearchScenesInBuildOnlyInfo = "Search scenes only in build";
        private const string SearchScenesOnlyInPathInfo = "Search scenes only in path";
        private const string ScenePathInfo = "Scene path";
        private const string AppendNewScenes = "Append new scenes";
        private const string RebuildScenesList = "Rebuild scenes list";
        private const string ClearList = "Clear list";
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var configuration = (PlayConfigurationSettings) target;

            configuration.EnablePlayConfiguration =
                EditorGUILayout.Toggle(EnablePlayConfigurationInfo, configuration.EnablePlayConfiguration);

            if (!configuration.EnablePlayConfiguration)
            {
                return;
            }

            configuration.RestoreScenesAfterPlayModeEnded =
                EditorGUILayout.Toggle(RestoreScenesAfterPlayInfo, configuration.RestoreScenesAfterPlayModeEnded);

            EditorGUILayout.Space(5);

            EditorGUI.BeginDisabledGroup(configuration.SearchOnlyInPath);

            configuration.SearchScenesInBuildOnly = EditorGUILayout.Toggle(SearchScenesInBuildOnlyInfo,
                configuration.SearchScenesInBuildOnly);

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(5);

            EditorGUI.BeginDisabledGroup(configuration.SearchScenesInBuildOnly);

            configuration.SearchOnlyInPath =
                EditorGUILayout.Toggle(SearchScenesOnlyInPathInfo, configuration.SearchOnlyInPath);

            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!(!configuration.SearchScenesInBuildOnly && configuration.SearchOnlyInPath));
            configuration.ScenePath = EditorGUILayout.TextField(ScenePathInfo, configuration.ScenePath);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(15);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(configuration.Scenes)), true);

            EditorGUILayout.Space(15);

            if (GUILayout.Button(AppendNewScenes, GUILayout.Height(40)))
            {
                configuration.AppendNewScenes();
            }

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(RebuildScenesList, GUILayout.Height(20)))
            {
                configuration.RebuildAllScenes();
            }

            if (GUILayout.Button(ClearList, GUILayout.Height(20)))
            {
                configuration.ClearList();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}