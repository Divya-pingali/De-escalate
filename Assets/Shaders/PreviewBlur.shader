Shader "QuestCameraKit/Preview/FrostedGlassPreview"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Tint("Tint", Color) = (0.92,0.96,1,1)
        _TintStrength("Tint Strength", Range(0, 1)) = 0.22
        _BlurRadius("Blur Radius", Range(0.0, 0.02)) = 0.006
        _BlurStrength("Blur Strength", Range(0.0, 1.0)) = 1.0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Pass
        {
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _Tint;
            float _TintStrength;
            float _BlurRadius;
            float _BlurStrength;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                half3 accum = 0;
                float total = 0;

                static const float2 offsets[9] =
                {
                    float2(0,0),
                    float2(1,0), float2(-1,0),
                    float2(0,1), float2(0,-1),
                    float2(0.7,0.7), float2(-0.7,0.7),
                    float2(0.7,-0.7), float2(-0.7,-0.7)
                };

                for (int i = 0; i < 9; i++)
                {
                    float w = i == 0 ? 2.0 : 1.0;
                    float2 sampleUv = uv + offsets[i] * _BlurRadius;

                    accum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, sampleUv).rgb * w;
                    total += w;
                }

                half3 original = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).rgb;
                half3 blurred = accum / total;

                // Actual blur intensity control
                half3 blurMixed = lerp(original, blurred, _BlurStrength);

                half3 tinted = lerp(blurMixed, blurMixed * _Tint.rgb, _TintStrength);

                return half4(tinted, 1);
            }

            ENDHLSL
        }
    }
}