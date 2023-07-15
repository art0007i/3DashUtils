Shader "Unlit/GradientShader"
{
    Properties
    {
        _ColorFrom("ColorFrom", Color) = (1,0,0,1)
        _ColorTo("ColorTo", Color) = (0,0,1,1)
        [MaterialToggle] _Vertical("Vertical", Float) = 0
        [MaterialToggle] _Hue("Rainbow", Float) = 0
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            fixed4 _ColorFrom;
            fixed4 _ColorTo;
            float _Vertical;
            float _Hue;

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed3 hsv2rgb(fixed3 input) {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(input.xxx + K.xyz) * 6.0 - K.www);

                return input.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), input.y);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                float t = i.uv.x;
                if (_Vertical > 0.5) t = i.uv.y;
                fixed4 col = lerp(_ColorFrom, _ColorTo, t);
                if (_Hue > 0.5) {
                    col = fixed4(hsv2rgb(fixed3(t, 1, 1)), 1);
                }
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
