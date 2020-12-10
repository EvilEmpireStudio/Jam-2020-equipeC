Shader "SharpShadowsToolkit/DrawDepthFromTexture"
{
    Properties
    {
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "RenderPipeline"="LightweightPipeline"
            "IgnoreProjector"="true"
        }

        Pass
        {
            Tags { "LightMode"="LightweightForward" }
            Cull Off
            ColorMask 0
            ZTest Off

            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma vertex DrawDepthVertex
            #pragma fragment DrawDepthFragment
#if UNITY_VERSION >= 201930
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#else
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
#endif

#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
            TEXTURE2D_ARRAY_FLOAT(_CameraDepthTexture);
#else
            TEXTURE2D_FLOAT(_CameraDepthTexture);
#endif

            SAMPLER(sampler_CameraDepthTexture);

            struct Attributes
            {
                float4 position     : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                float2 uv :TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings DrawDepthVertex(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                VertexPositionInputs v = GetVertexPositionInputs(input.position.xyz);

                output.position = v.positionCS;
                output.uv = UnityStereoTransformScreenSpaceTex(input.uv).xy;
                return output;
            }

            float DrawDepthFragment(Varyings input) : SV_Depth
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
                return SAMPLE_TEXTURE2D_ARRAY(_CameraDepthTexture, sampler_CameraDepthTexture, input.uv, unity_StereoEyeIndex).r;
#else
                return SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, input.uv).r;
#endif
            }
            ENDHLSL
        }
    }
}
