using UnityEngine;

namespace SharpShadowsToolkit
{
    [System.Serializable]
    public class ShadowAsset : ScriptableObject
    {
        [Header("Build-time constants")]
        [ReadOnly] public Mesh shadowMesh;
        [ReadOnly] public bool isAnimated;
        [ReadOnly] public bool isTwoManifold;
        [ReadOnly] public bool usesThirtyTwoBitIndices;
        [ReadOnly] public int vertexCount;
        [ReadOnly] public int triangleCount;
        [ReadOnly] public float boundsPadFactor;

        [Header("Configurables")]

        [Tooltip(Docs.Tooltip.AllowCameraInShadow)]
        public bool allowCameraInShadow;

        [Tooltip(Docs.Tooltip.RenderLayer)]
        public SingleLayer renderLayer;
    }
}
