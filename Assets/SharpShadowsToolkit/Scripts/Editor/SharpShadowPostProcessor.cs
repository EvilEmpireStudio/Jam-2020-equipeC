namespace SharpShadowsToolkit
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;
    using System.IO;

#if SHARP_SHADOWS_DEBUG
    using Debug = UnityEngine.Debug;
#else
    using Debug = DebugNoOp;
#endif

    public class SharpShadowPostProcessor : AssetPostprocessor
    {
        public override uint GetVersion()
        {
            return 6;
        }

        protected static Dictionary<string, List<string>> ModelDependencies = new Dictionary<string, List<string>>(); // dependency path, model paths

        protected static void AddModelDependency(string modelPath, string dependencyPath)
        {
            if (!ModelDependencies.ContainsKey(dependencyPath))
            {
                ModelDependencies.Add(dependencyPath, new List<string>());
            }
            ModelDependencies[dependencyPath].Add(modelPath);
        }

        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            // Handle delayed post-processing
            foreach (var importedPath in importedAssets)
            {
                if (ModelDependencies.TryGetValue(importedPath, out List<string> modelPaths))
                {
                    var dependency = AssetDatabase.LoadMainAssetAtPath(importedPath);
                    if (dependency is ProjectSettings || dependency is ShadowAssetCollection)
                    {
                        Debug.Log("Dependency has now been imported, re-importing affected models so that post-processing can be performed...");
                        foreach (var modelPath in modelPaths)
                        {
                            AssetDatabase.ImportAsset(modelPath);
                        }
                    }
                    ModelDependencies.Remove(importedPath);
                }
            }
        }

        public void OnPostprocessModel(GameObject go)
        {
            if (assetPath.ToLower().Contains("noshadow"))
            {
                return;
            }

            Debug.Log("Post processing asset '" + assetPath + "' ...");

            var projectSettings = ProjectSettings.GetOrCreateSettings();
            if (projectSettings == null)
            {
                Debug.Log("Project settings exists but has not yet been imported, delaying post-processing...");
                AddModelDependency(assetPath, ProjectSettings.Path);
                return;
            }

            if (!CreateMainAsset(
                out ShadowAssetCollection mainAsset,
                out Dictionary<string, ShadowAsset> existingShadowAssets))
            {
                return;
            }

            var shadowAssets = new Dictionary<Mesh, ShadowAsset>();
            var usedMeshes = new HashSet<Mesh>();
            var usedShadowAssets = new HashSet<ShadowAsset>();

            // Non-skinned meshes
            var meshFilters = go.GetComponentsInChildren<MeshFilter>();
            foreach (var meshFilter in meshFilters)
            {
                var sourceMesh = meshFilter.sharedMesh;
                if (sourceMesh == null)
                {
                    continue;
                }

                // Create shadow asset if new source mesh
                if (!shadowAssets.TryGetValue(sourceMesh, out ShadowAsset shadowAsset))
                {
                    UpdateSubAsset(sourceMesh, projectSettings, existingShadowAssets, out shadowAsset);
                    if (shadowAsset == null)
                    {
                        continue;
                    }
                    shadowAssets.Add(sourceMesh, shadowAsset);
                    usedMeshes.Add(shadowAsset.shadowMesh);
                    usedShadowAssets.Add(shadowAsset);
                }

                // Add component
                var sharpShadow = meshFilter.gameObject.AddComponent<SharpShadow>();
                sharpShadow.shadowAsset = shadowAsset;
            }

            // Skinned meshes
            var index = 0;
            var skinnedMeshRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var renderer in skinnedMeshRenderers)
            {
                var sourceMesh = renderer.sharedMesh;
                if (sourceMesh == null)
                {
                    continue;
                }

                // Create shadow asset if new source mesh
                if (!shadowAssets.TryGetValue(sourceMesh, out ShadowAsset shadowAsset))
                {
                    UpdateSubAsset(sourceMesh, projectSettings, existingShadowAssets, out shadowAsset);
                    if (shadowAsset == null)
                    {
                        continue;
                    }
                    shadowAssets.Add(sourceMesh, shadowAsset);
                    usedMeshes.Add(shadowAsset.shadowMesh);
                    usedShadowAssets.Add(shadowAsset);
                }

                // Add shadow game object with component
                var shadowGo = new GameObject("Shadow" + index++);
                shadowGo.transform.parent = renderer.transform;
                shadowGo.transform.localPosition = Vector3.zero;
                shadowGo.transform.localRotation = Quaternion.identity;
                shadowGo.transform.localScale = Vector3.one;

                var shadowRenderer = shadowGo.AddComponent<SkinnedMeshRenderer>();
                shadowRenderer.sharedMesh = shadowAsset.shadowMesh;
                shadowRenderer.rootBone = renderer.rootBone;
                shadowRenderer.bones = renderer.bones;

                var sharpShadow = shadowGo.AddComponent<SharpShadow>();
                sharpShadow.shadowAsset = shadowAsset;
            }

            // No need to use context.DependsOnSourceAsset() here as the component does this indirectly (such a call also
            // introduces bug where the post processor is run every time the scene is saved!)

            RemoveOldSubAssets(mainAsset, usedMeshes, usedShadowAssets);

            AssetDatabase.SaveAssets();
        }

        protected static string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path).Replace('\\', '/');
        }

        protected static string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        protected static string CombinePath(params string[] paths)
        {
            return Path.Combine(paths).Replace('\\', '/');
        }

        protected static string GetShadowPath(string assetPath)
        {
            return CombinePath(GetDirectoryName(assetPath), GetFileNameWithoutExtension(assetPath) + "_shadows.asset");
        }

        protected bool CreateMainAsset(out ShadowAssetCollection mainAsset, out Dictionary<string, ShadowAsset> existingShadowAssets)
        {
            var path = GetShadowPath(assetPath);
            var asset = AssetDatabase.LoadAssetAtPath<ShadowAssetCollection>(path);
            if (!asset)
            {
                if (File.Exists(path))
                {
                    Debug.Log("Shadow asset exists but has not yet been imported, delaying post-processing...");
                    AddModelDependency(assetPath, path);

                    mainAsset = null;
                    existingShadowAssets = null;
                    return false;
                }
                asset = ScriptableObject.CreateInstance<ShadowAssetCollection>();
            }

            if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(asset)))
            {
                Debug.Log("Could not find pre-existing asset, creating a new one...");
                AssetDatabase.CreateAsset(asset, path);
            }

            mainAsset = asset;
            existingShadowAssets = new Dictionary<string, ShadowAsset>();
            var objs = AssetDatabase.LoadAllAssetsAtPath(path);
            foreach (var obj in objs)
            {
                var shadowAsset = obj as ShadowAsset;
                if (shadowAsset)
                {
                    existingShadowAssets.Add(shadowAsset.name, shadowAsset);
                }
            }
            return true;
        }

        protected void UpdateSubAsset(Mesh sourceMesh, ProjectSettings settings, Dictionary<string, ShadowAsset> existingShadowAssets, out ShadowAsset outShadowAsset)
        {
            if (sourceMesh == null)
            {
                throw new System.ArgumentNullException(nameof(sourceMesh));
            }

            var shadowAssetName = sourceMesh.name + "_shadow_asset";
            var newAsset = !existingShadowAssets.TryGetValue(shadowAssetName, out ShadowAsset shadowAsset);

            if (newAsset)
            {
                Debug.LogFormat("Creating shadow asset for source mesh '{0}'...", sourceMesh.name);
                shadowAsset = ScriptableObject.CreateInstance<ShadowAsset>();
                shadowAsset.name = shadowAssetName;
                shadowAsset.allowCameraInShadow = settings.allowCameraInShadow;
                shadowAsset.renderLayer = settings.renderLayer;
            }
            else
            {
                Debug.LogFormat("Updating shadow asset for source mesh '{0}'...", sourceMesh.name);
            }

            ShadowMesh.Create(
                sourceMesh,
                settings.boundsPadFactor,
                ref shadowAsset.shadowMesh,
                out shadowAsset.isAnimated,
                out shadowAsset.isTwoManifold,
                out shadowAsset.usesThirtyTwoBitIndices,
                out shadowAsset.vertexCount,
                out shadowAsset.triangleCount,
                out shadowAsset.boundsPadFactor);

            if (shadowAsset.shadowMesh == null)
            {
                Debug.LogError("Could not create shadow mesh");
                outShadowAsset = null;
                return;
            }

            if (newAsset)
            {
                var path = GetShadowPath(assetPath);
                AssetDatabase.AddObjectToAsset(shadowAsset.shadowMesh, path);
                AssetDatabase.AddObjectToAsset(shadowAsset, path);
            }

            outShadowAsset = shadowAsset;
        }

        protected void RemoveOldSubAssets(ShadowAssetCollection mainAsset, HashSet<Mesh> usedMeshes, HashSet<ShadowAsset> usedShadowAssets)
        {
            var changed = false;
            var path = GetShadowPath(assetPath);
            var objs = AssetDatabase.LoadAllAssetsAtPath(path);
            foreach (var obj in objs)
            {
                var mesh = obj as Mesh;
                if (mesh && !usedMeshes.Contains(mesh))
                {
                    Object.DestroyImmediate(mesh, true);
                    changed = true;
                }
                var shadowAsset = obj as ShadowAsset;
                if (shadowAsset && !usedShadowAssets.Contains(shadowAsset))
                {
                    Object.DestroyImmediate(shadowAsset, true);
                    changed = true;
                }
            }
            if (changed)
            {
                EditorUtility.SetDirty(mainAsset);
            }
        }
    }
}
