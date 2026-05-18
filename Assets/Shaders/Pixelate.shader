Shader "Custom/OverlayPixelate"
{
    Properties
    {
        _Color ("Tint", Color) = (1,1,1,1)
        _PixelSize ("Pixel Size", Range(0.001, 0.1)) = 0.02
        _Alpha ("Alpha", Range(0,1)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _LeftTex;

            float4 _Color;
            float _PixelSize;
            float _Alpha;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Convert to screen UV (0–1)
                float2 uv = i.screenPos.xy / i.screenPos.w;

                // Optional: flip Y if needed
                // uv.y = 1.0 - uv.y;

                // Pixelation
                uv = floor(uv / _PixelSize) * _PixelSize;

                fixed4 col = tex2D(_LeftTex, uv);

                col *= _Color;
                col.a = _Alpha;

                return col;
            }
            ENDCG
        }
    }
}