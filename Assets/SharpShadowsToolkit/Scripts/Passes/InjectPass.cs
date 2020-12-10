using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_2019_3_OR_NEWER
using UnityEngine.Rendering.Universal;
#else
using UnityEngine.Rendering.LWRP;
#endif

namespace SharpShadowsToolkit
{
    public class InjectPass : ScriptableRenderPass
    {
        public LayerMask shadowVolumeLayerMask;

        protected List<ShaderTagId> depthOnlyShaderTag;
        protected List<ShaderTagId> volumeShaderTags;
        protected Material drawDepthFromTextureMaterial;
        protected Material visualizeShadowsFullscreen;
        protected RenderTargetHandle screenSpaceShadowmap;
        protected RenderTextureDescriptor renderTextureDescriptor;

        public InjectPass(RenderPassEvent evt)
        {
            renderPassEvent = evt;

            depthOnlyShaderTag = new List<ShaderTagId>()
            {
                new ShaderTagId("DepthOnly"),
            };
            volumeShaderTags = new List<ShaderTagId>()
            {
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("LightweightForward"),
                new ShaderTagId("SRPDefaultUnlit"),
            };

            screenSpaceShadowmap.Init("_ScreenSpaceShadowmapTexture");
        }

        public void Setup(RenderTextureDescriptor baseDescriptor)
        {
            renderTextureDescriptor = baseDescriptor;
            renderTextureDescriptor.msaaSamples = 1;
            renderTextureDescriptor.colorFormat = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.R8)
                ? RenderTextureFormat.R8
                : RenderTextureFormat.ARGB32;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(screenSpaceShadowmap.id, renderTextureDescriptor, FilterMode.Bilinear);

            ConfigureTarget(screenSpaceShadowmap.Identifier());
            ConfigureClear(ClearFlag.All, Color.white);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (drawDepthFromTextureMaterial == null)
            {
                var shader = Shader.Find("SharpShadowsToolkit/DrawDepthFromTexture");
                if (shader == null)
                {
                    return;
                }
                drawDepthFromTextureMaterial = new Material(shader);
            }
            if (visualizeShadowsFullscreen == null)
            {
                var shader = Shader.Find("SharpShadowsToolkit/VisualizeShadowsFullscreen");
                if (shader == null)
                {
                    return;
                }
                visualizeShadowsFullscreen = new Material(shader);
            }

            var camera = renderingData.cameraData.camera;

            var cmd = CommandBufferPool.Get("SharpShadowsToolkit Inject Pass");

            // The commented code below resolves depth from the depth texture. This is only viable if a depth
            // pre-pass was executed in the forward renderer (because this pass executes before rendering
            // opaques). Currently there is no straightforward way to know if a depth pre-pass was executed or
            // not, which is why the code is unused.
            //
            // cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
            // cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, drawDepthFromTextureMaterial);
            // cmd.SetViewProjectionMatrices(camera.worldToCameraMatrix, camera.projectionMatrix);
            // context.ExecuteCommandBuffer(cmd);
            // cmd.Clear();

            // Always redraw scene to determine depth
            {
                var drawingSettings = CreateDrawingSettings(depthOnlyShaderTag, ref renderingData, SortingCriteria.CommonOpaque);
                var filteringSettings = new FilteringSettings(RenderQueueRange.opaque, ~shadowVolumeLayerMask);
                var renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings, ref renderStateBlock);
            }

            // Draw shadow volumes
            {
                var drawingSettings = CreateDrawingSettings(volumeShaderTags, ref renderingData, SortingCriteria.CommonOpaque);
                var filteringSettings = new FilteringSettings(RenderQueueRange.opaque, shadowVolumeLayerMask);
                var renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings, ref renderStateBlock);
            }

            // Visualize shadows
            cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
            cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, visualizeShadowsFullscreen);
            cmd.SetViewProjectionMatrices(camera.worldToCameraMatrix, camera.projectionMatrix);

            // Enable shader keywords so that opaque objects use the screen space shadow resolve texture we just updated
            cmd.EnableShaderKeyword("_MAIN_LIGHT_SHADOWS");
            cmd.EnableShaderKeyword("_MAIN_LIGHT_SHADOWS_CASCADE");
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (cmd == null)
            {
                return;
            }

            cmd.ReleaseTemporaryRT(screenSpaceShadowmap.id);
        }
    }
}
