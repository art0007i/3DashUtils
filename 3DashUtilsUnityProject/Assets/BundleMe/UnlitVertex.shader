Shader "Custom/UnlitVertexColors" {
	Properties{
		_Alpha("Alpha Mix", Range(0,1)) = 1
		_Color("Color", Color) = (1,1,1,1)
	}

	Category{
	Tags { "Queue" = "Overlay+1" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	Lighting Off ZWrite Off ZTest Always


	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float _Alpha;
			fixed4 _Color;

			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
			};

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return fixed4(i.color.r, i.color.g, i.color.b, i.color.a * _Alpha) * _Color;
			}
			ENDCG
		}
	}
	}
}
