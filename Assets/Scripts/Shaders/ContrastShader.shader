Shader "Custom/ContrastShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Contrast("Contrast", Float) = 0
		_Br("Brightness", Float) = 0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			uniform float _Contrast;
			uniform float _Br;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 color = tex2D(_MainTex, i.uv);
				color += (_Br / 255);
				color = color - _Contrast * (color - 1.0) * color *(color - 0.5);
				return color;
			}
			ENDCG
		}
	}
}
