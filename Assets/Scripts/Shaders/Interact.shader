Shader "Unlit/Interact"
{
	Properties
	{
		_MainColor("Main Color", Color) = (0,0,0,1)
		_MainTex ("Texture", 2D) = "white" {}
	[HDR]_InteractColor("Interact color", Color) = (0,0,0,1)
		[HideInInspector]
		_Intensity("Intensity", Range(0.0, 3)) = 1

			_MaxIntensity("Max intensity", Range(0.0, 4)) = 1
			_TimeMultiplier("Time multiplier", Range(1,200)) = 100
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

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _InteractColor;
			float _Intensity;
			float _MaxIntensity;
			float _TimeMultiplier;
			float4 _MainColor;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
			col *= _MainColor;

			float _lerp = (sin(_Time * _TimeMultiplier) + 1) / 2;
			_Intensity = lerp(0, _MaxIntensity, _lerp);
				col += (col*_InteractColor*_Intensity);
				
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
