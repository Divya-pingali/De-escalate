Shader "QuestCameraKit/CameraMapping/StereoPassthroughToonHardTransparent"
{
    Properties
    {
        _LeftTex("Left Texture", 2D) = "black" {}
        _RightTex("Right Texture", 2D) = "black" {}

        _LeftUvOffset("Left UV Offset", Vector) = (0,0,0,0)
        _RightUvOffset("Right UV Offset", Vector) = (0,0,0,0)

        _ColorSteps("Color Steps", Range(2, 6)) = 2

        _ShadowThreshold("Shadow Threshold", Range(0,1)) = 0.5
        _ShadowDarken("Shadow Darken", Range(0,1)) = 0.5

        _EdgeThreshold("Edge Threshold", Range(0.001, 0.15)) = 0.018
        _EdgeStrength("Edge Strength", Range(0, 3)) = 1.8
        _SampleOffset("Sample Offset", Range(0.0005, 0.02)) = 0.0045

        _Brightness("Brightness", Range(0.5, 2.0)) = 1.08
        _Contrast("Contrast", Range(0.5, 2.0)) = 1.25
        _Saturation("Saturation", Range(0, 2)) = 1.2

        _Tint("Tint", Color) = (1.0, 0.98, 1.03, 1)
        _TintStrength("Tint Strength", Range(0,1)) = 0.05

        _Alpha("Transparency", Range(0,1)) = 1.0
        _PreviewEye("Preview Eye (0 Left, 1 Right)", Range(0,1)) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Pass
        {
            Name "ForwardUnlit"

            Cull Off
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ STEREO_INSTANCING_ON STEREO_MULTIVIEW_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_LeftTex);
            SAMPLER(sampler_LeftTex);

            TEXTURE2D(_RightTex);
            SAMPLER(sampler_RightTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _LeftUvOffset;
                float4 _RightUvOffset;
                float _ColorSteps;
                float _ShadowThreshold;
                float _ShadowDarken;
                float _EdgeThreshold;
                float _EdgeStrength;
                float _SampleOffset;
                float _Brightness;
                float _Contrast;
                float _Saturation;
                float4 _Tint;
                float _TintStrength;
                float _Alpha;
                float _PreviewEye;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float Luminance(float3 c)
            {
                return dot(c, float3(0.299, 0.587, 0.114));
            }

            float3 ApplyContrast(float3 color, float contrast)
            {
                return (color - 0.5) * contrast + 0.5;
            }

            float3 ApplySaturation(float3 color, float saturation)
            {
                float l = Luminance(color);
                return lerp(l.xxx, color, saturation);
            }

            float3 QuantizeColor(float3 color, float steps)
            {
                return floor(color * steps) / max(steps - 1.0, 1.0);
            }

            float2 GetEyeUV(float2 uv)
            {
                #if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
                    int eyeIndex = unity_StereoEyeIndex;
                #else
                    int eyeIndex = (_PreviewEye < 0.5) ? 0 : 1;
                #endif

                return (eyeIndex == 0)
                    ? uv * _LeftUvOffset.xy + _LeftUvOffset.zw
                    : uv * _RightUvOffset.xy + _RightUvOffset.zw;
            }

            float3 SampleTex(float2 uv)
            {
                #if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
                    int eyeIndex = unity_StereoEyeIndex;
                #else
                    int eyeIndex = (_PreviewEye < 0.5) ? 0 : 1;
                #endif

                return (eyeIndex == 0)
                    ? SAMPLE_TEXTURE2D(_LeftTex, sampler_LeftTex, uv).rgb
                    : SAMPLE_TEXTURE2D(_RightTex, sampler_RightTex, uv).rgb;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = GetEyeUV(IN.uv);

                float3 c   = SampleTex(uv);
                float3 cx1 = SampleTex(uv + float2(_SampleOffset, 0));
                float3 cx2 = SampleTex(uv - float2(_SampleOffset, 0));
                float3 cy1 = SampleTex(uv + float2(0, _SampleOffset));
                float3 cy2 = SampleTex(uv - float2(0, _SampleOffset));

                c *= _Brightness;
                c = ApplyContrast(c, _Contrast);
                c = ApplySaturation(c, _Saturation);
                c = saturate(c);

                float lum  = Luminance(c);
                float lumX = abs(Luminance(cx1) - Luminance(cx2));
                float lumY = abs(Luminance(cy1) - Luminance(cy2));

                float edge = lumX + lumY;
                edge = step(_EdgeThreshold, edge) * _EdgeStrength;
                edge = saturate(edge);

                // Hard posterization
                float3 toon = QuantizeColor(c, _ColorSteps);

                // Binary shadow region
                float shadowMask = step(lum, _ShadowThreshold);
                toon *= lerp(1.0, 1.0 - _ShadowDarken, shadowMask);

                // Slight stylized tint
                toon = lerp(toon, toon * _Tint.rgb, _TintStrength);

                // Very dark outline
                float3 outlined = lerp(toon, float3(0.01, 0.01, 0.01), edge);

                return half4(saturate(outlined), _Alpha);
            }
            ENDHLSL
        }
    }
}