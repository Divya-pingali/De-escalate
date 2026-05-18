Shader "QuestCameraKit/Preview/BlockPreview"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BlockColor("Block Color", Color) = (0,0,0,1)
        _BlockStrength("Block Strength", Range(0,1)) = 0.5
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

            float4 _BlockColor;
            float _BlockStrength;

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
                half4 source = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                half3 blocked = lerp(source.rgb, _BlockColor.rgb, _BlockStrength);

                return half4(blocked, source.a);
            }

            ENDHLSL
        }
    }

    FallBack "Hidden/InternalErrorShader"
}