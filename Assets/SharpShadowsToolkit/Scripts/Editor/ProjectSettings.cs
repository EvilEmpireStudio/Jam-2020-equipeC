using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System.Collections.Generic;

namespace SharpShadowsToolkit
{
    public class ProjectSettings : ScriptableObject
    {
        public const string Path = "Assets/SharpShadowsToolkit/Scripts/Editor/ProjectSettings.asset";

        [Tooltip(Docs.Tooltip.AllowCameraInShadow)]
        [HideInInspector] [SerializeField] public bool allowCameraInShadow;

        [Tooltip(Docs.Tooltip.RenderLayer)]
        [HideInInspector] [SerializeField] public SingleLayer renderLayer;

        [Tooltip(Docs.Tooltip.BoundsPadFactor)]
        [HideInInspector] [SerializeField] public float boundsPadFactor;

        internal static ProjectSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<ProjectSettings>(Path);
            if (settings == null)
            {
                if (System.IO.File.Exists(Path))
                {
                    // Not yet imported, will happen in a 'Reimport All'
                    return null;
                }
                Debug.LogFormat("Creating new project settings asset for Sharp Shadows Toolkit at '{0}'", Path);
                settings = CreateInstance<ProjectSettings>();
                settings.allowCameraInShadow = false;
                settings.renderLayer = 1; // TransparentFX
                settings.boundsPadFactor = 1.0f;
                AssetDatabase.CreateAsset(settings, Path);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }
}
