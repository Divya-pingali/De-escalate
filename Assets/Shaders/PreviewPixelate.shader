Shader "QuestCameraKit/Preview/PixelatePreview"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Tint("Tint", Color) = (1,1,1,1)
        _TintStrength("Tint Strength", Range(0, 1)) = 0
        _PixelSize("Pixel Size", Range(0.001, 0.1)) = 0.02
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
            float _PixelSize;

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
                float pixelSize = max(_PixelSize, 0.0001);

                float2 uv = floor(IN.uv / pixelSize) * pixelSize;

                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

                half3 tinted = lerp(col.rgb, col.rgb * _Tint.rgb, _TintStrength);

                return half4(tinted, col.a);
            }

            ENDHLSL
        }
    }

    FallBack "Hidden/InternalErrorShader"
}