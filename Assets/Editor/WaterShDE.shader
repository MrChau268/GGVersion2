// Create a Shader Graph or a simple Shader (Unlit)
Shader "Custom/RadialWave"
{
    Properties
    {
        _WaveRadius ("Wave Radius", Float) = 0
        _WaveWidth ("Wave Width", Float) = 0.05
        _WaveColor ("Wave Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _WaveRadius;
            float _WaveWidth;
            float4 _WaveColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2 - 1; // Center UV at 0,0
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float dist = length(i.uv);
                float alpha = smoothstep(_WaveRadius, _WaveRadius - _WaveWidth, dist);
                return _WaveColor * alpha;
            }
            ENDCG
        }
    }
}
