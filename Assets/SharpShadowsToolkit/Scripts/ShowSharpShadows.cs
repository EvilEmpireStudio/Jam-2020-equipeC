using UnityEngine;
#if UNITY_2019_3_OR_NEWER
using UnityEngine.Rendering.Universal;
#else
using UnityEngine.Rendering.LWRP;
#endif

namespace SharpShadowsToolkit
{
    public class ShowSharpShadows : ScriptableRendererFeature
    {
        [System.Serializable]
        public enum ShadeMode
        {
            InjectIntoScreenSpaceShadowResolveTexture,
            MultiplySceneAfterOpaque,
        }

        [System.Serializable]
        public class Settings
        {
            public bool enabled = true;

            [Tooltip(Docs.Tooltip.ShadowVolumeRenderLayer)]
            public SingleLayer shadowVolumeRenderLayer = 1; // TransparentFX

            [Delayed]
            [Tooltip(Docs.Tooltip.NearExtrusionDistance)]
            public float nearExtrusionDistance = 0.002f;

            [Delayed]
            [Tooltip(Docs.Tooltip.FarExtrusionDistance)]
            public float farExtrusionDistance = 100.0f;

            [Range(0.0f, 1.0f)]
            [Tooltip(Docs.Tooltip.ShadowIntensity)]
            public float shadowIntensity = 1.0f;

            [Tooltip(Docs.Tooltip.ShadeMode)]
            public ShadeMode shadeMode = ShadeMode.InjectIntoScreenSpaceShadowResolveTexture;

            [Tooltip(Docs.Tooltip.MitigateSelfShadowArtifacts)]
            public bool mitigateSelfShadowArtifacts = true;
        }

        public Settings settings = new Settings();

        protected InjectPass injectPass;
        protected DrawRenderersPass multiplyVolumesPass;
        protected MultiplyPass multiplyPass;

        public override void Create()
        {
            injectPass = new InjectPass(RenderPassEvent.BeforeRenderingPrepasses + 1); // right after ScreenSpaceShadowResolvePass

            multiplyVolumesPass = new DrawRenderersPass(RenderPassEvent.AfterRenderingOpaques + 1, new string[]
            {
                "UniversalForward", "LightweightForward", "SRPDefaultUnlit",
            });
            multiplyPass = new MultiplyPass(RenderPassEvent.AfterRenderingOpaques + 2);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (!settings.enabled || renderingData.lightData.mainLightIndex == -1)
            {
                return;
            }

            // Update materials
            var nearDist = settings.mitigateSelfShadowArtifacts ? settings.farExtrusionDistance : settings.nearExtrusionDistance;
            var farDist = settings.mitigateSelfShadowArtifacts ? settings.nearExtrusionDistance : settings.farExtrusionDistance;

            Shader.SetGlobalFloat("_SST_ShadowIntensity", settings.shadowIntensity);
            Shader.SetGlobalFloat("_SST_NearExtrusionDistance", nearDist);
            Shader.SetGlobalFloat("_SST_FarExtrusionDistance", farDist);

            // Queue passes
            if (settings.shadeMode == ShadeMode.InjectIntoScreenSpaceShadowResolveTexture)
            {
                injectPass.shadowVolumeLayerMask = 1 << settings.shadowVolumeRenderLayer;
                injectPass.Setup(renderingData.cameraData.cameraTargetDescriptor);
                renderer.EnqueuePass(injectPass);
            }
            else if (settings.shadeMode == ShadeMode.MultiplySceneAfterOpaque)
            {
                multiplyVolumesPass.layerMask = 1 << settings.shadowVolumeRenderLayer;
                renderer.EnqueuePass(multiplyVolumesPass);

                multiplyPass.shadowIntensity = settings.shadowIntensity;
                renderer.EnqueuePass(multiplyPass);
            }
        }
    }
}
