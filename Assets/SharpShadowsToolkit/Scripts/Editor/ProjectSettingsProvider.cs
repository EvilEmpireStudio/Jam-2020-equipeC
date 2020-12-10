using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System.Collections.Generic;

namespace SharpShadowsToolkit
{
    public class ProjectSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new SettingsProvider("Project/Sharp Shadows Toolkit", SettingsScope.Project)
            {
                guiHandler = (searchContext) =>
                {
                    var s = ProjectSettings.GetSerializedSettings();
                    EditorGUILayout.LabelField("The following settings are project-level settings and will be checked into version control");
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Model shadow asset generation defaults", EditorStyles.boldLabel);
                    EditorGUILayout.HelpBox("These defaults are used when creating a new shadow asset in the model post-processor. Note that it is always possibe to change the initial values manually on a per-shadow-asset basis by selecting the asset in the Project window.", MessageType.None);
                    EditorGUILayout.PropertyField(s.FindProperty("allowCameraInShadow"), new GUIContent("Allow Camera In Shadow"));
                    EditorGUILayout.PropertyField(s.FindProperty("renderLayer"), new GUIContent("Render Layer"));
                    EditorGUILayout.PropertyField(s.FindProperty("boundsPadFactor"), new GUIContent("Bounds Pad Factor"));
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("Tip: To update a large number of models, change these settings, remove the model shadow asset collections ('x_shadows.asset' files) and then re-import the models", MessageType.Info);
                    s.ApplyModifiedProperties();

                },
                keywords = new HashSet<string>(
                    new []
                    {
                        "allow camera in shadow",
                        "render layer",
                        "bounds pad factor",
                    }),
            };
            return provider;
        }
    }
}
