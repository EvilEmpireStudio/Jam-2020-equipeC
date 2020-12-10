namespace SharpShadowsToolkit
{
    using UnityEngine;

#if SHARP_SHADOWS_DEBUG
    using Debug = UnityEngine.Debug;
#else
    using Debug = DebugNoOp;
#endif

    [System.Serializable]
    public class RuntimeCreationSettings
    {
        [Tooltip(Docs.Tooltip.AllowCameraInShadow)]
        public bool allowCameraInShadow = false;

        [Tooltip(Docs.Tooltip.RenderLayer)]
        public SingleLayer renderLayer = 1; // TransparentFX

        [Tooltip(Docs.Tooltip.BoundsPadFactor)]
        public float boundsPadFactor = 1.0f;
    }

    [HelpURL("https://gustavolsson.com/sharp-shadows-toolkit")]
    [ExecuteAlways]
    public class SharpShadow : MonoBehaviour
    {
        protected static Material updateStencilAlwaysMaterial;
        protected static Material updateStencilOnDepthPassMaterial;

        [Tooltip(Docs.Tooltip.ShadowAsset)]
        public ShadowAsset shadowAsset;

        [Tooltip(Docs.Tooltip.CreateRuntimeShadowAsset)]
        public bool createRuntimeShadowAsset;

        [Tooltip(Docs.Tooltip.RuntimeCreationSettings)]
        public RuntimeCreationSettings runtimeCreationSettings;

        protected ShadowAsset runtimeShadowAsset;
        protected SkinnedMeshRenderer skinnedRenderer;

        public void CreateRuntimeShadowAsset()
        {
            CleanUpRuntimeShadowAsset();

            Debug.LogFormat("Creating runtime shadow asset for '{0}'", name);

            Mesh sourceMesh = null;
            var meshFilter = GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                sourceMesh = meshFilter.sharedMesh;
            }
            var skinnedMeshRenderer = transform.parent?.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null)
            {
                sourceMesh = skinnedMeshRenderer.sharedMesh;
            }
            if (sourceMesh == null)
            {
                return;
            }
            var asset = ScriptableObject.CreateInstance<ShadowAsset>();
            ShadowMesh.Create(
                sourceMesh,
                runtimeCreationSettings.boundsPadFactor,
                ref asset.shadowMesh,
                out asset.isAnimated,
                out asset.isTwoManifold,
                out asset.usesThirtyTwoBitIndices,
                out asset.vertexCount,
                out asset.triangleCount,
                out asset.boundsPadFactor);
            if (asset.shadowMesh == null)
            {
                return;
            }
            asset.allowCameraInShadow = runtimeCreationSettings.allowCameraInShadow;
            asset.renderLayer = runtimeCreationSettings.renderLayer;

            runtimeShadowAsset = asset;
        }

        protected static void DestroyAlways(Object obj)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                DestroyImmediate(obj);
            }
            else
            {
                Destroy(obj);
            }
#else
            Destroy(obj);
#endif
        }

        protected void CleanUpRuntimeShadowAsset()
        {
            if (runtimeShadowAsset != null)
            {
                Debug.LogFormat("Cleaning up runtime shadow asset for '{0}'", name);

                if (runtimeShadowAsset.shadowMesh != null)
                {
                    DestroyAlways(runtimeShadowAsset.shadowMesh);
                }
                DestroyAlways(runtimeShadowAsset);
                runtimeShadowAsset = null;
            }
        }

        protected void Initialize()
        {
            if (runtimeShadowAsset == null && createRuntimeShadowAsset)
            {
                CreateRuntimeShadowAsset();
            }

            if (shadowAsset != null && shadowAsset.isAnimated ||
                runtimeShadowAsset != null && runtimeShadowAsset.isAnimated)
            {
                skinnedRenderer = GetComponent<SkinnedMeshRenderer>();
            }
        }

        public void OnEnable()
        {
            Initialize();
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            Initialize();
        }
#endif

        public void LateUpdate()
        {
            // Update
            if (runtimeShadowAsset != null)
            {
                if (!createRuntimeShadowAsset)
                {
                    CleanUpRuntimeShadowAsset();
                    return;
                }

                runtimeShadowAsset.allowCameraInShadow = runtimeCreationSettings.allowCameraInShadow;
                runtimeShadowAsset.renderLayer = runtimeCreationSettings.renderLayer;

                if (runtimeShadowAsset.boundsPadFactor != runtimeCreationSettings.boundsPadFactor)
                {
                    CreateRuntimeShadowAsset();
                }
            }

            // Render (prefer runtime shadow asset)
            var asset = runtimeShadowAsset != null ? runtimeShadowAsset : shadowAsset;
            if (asset == null || asset.shadowMesh == null)
            {
                return;
            }
            if (updateStencilAlwaysMaterial == null)
            {
                updateStencilAlwaysMaterial = new Material(Shader.Find("SharpShadowsToolkit/VolumeUpdateStencilAlways"))
                {
                    enableInstancing = true,
                };
            }
            if (updateStencilOnDepthPassMaterial == null)
            {
                updateStencilOnDepthPassMaterial = new Material(Shader.Find("SharpShadowsToolkit/VolumeUpdateStencilOnDepthPass"))
                {
                    enableInstancing = true,
                };
            }

            if (asset.isAnimated && skinnedRenderer != null)
            {
                gameObject.layer = asset.renderLayer;

                skinnedRenderer.sharedMesh = asset.shadowMesh;
                skinnedRenderer.localBounds = asset.shadowMesh.bounds;
                skinnedRenderer.updateWhenOffscreen = false;

                var materials = skinnedRenderer.sharedMaterials;
                if (asset.allowCameraInShadow)
                {
                    if (materials.Length != 2)
                    {
                        materials = new Material[2];
                    }
                    materials[0] = updateStencilAlwaysMaterial;
                    materials[1] = updateStencilOnDepthPassMaterial;
                }
                else
                {
                    if (materials.Length != 1)
                    {
                        materials = new Material[1];
                    }
                    materials[0] = updateStencilOnDepthPassMaterial;
                }
                skinnedRenderer.sharedMaterials = materials;
            }
            else
            {
                if (asset.allowCameraInShadow)
                {
                    Graphics.DrawMesh(asset.shadowMesh, transform.localToWorldMatrix, updateStencilAlwaysMaterial, asset.renderLayer);
                }
                Graphics.DrawMesh(asset.shadowMesh, transform.localToWorldMatrix, updateStencilOnDepthPassMaterial, asset.renderLayer);
            }
        }
    }
}
