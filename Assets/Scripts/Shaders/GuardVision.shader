Shader "Custom/GuardVision"
{
    Properties
    {
        [HDR]_MainTex ("Texture", 2D) = "white" {}
		[HDR]_EffectTex("Texture", 2D) = "white"{}
		_Value("Value", Float) = 0 
		
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
                float4 scrPos : TEXCOORD1;
            };
 
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.scrPos = ComputeScreenPos(o.vertex);
                return o;
            }
             
            sampler2D _MainTex;
			sampler2D _EffectTex;
			sampler2D _CameraDepthTexture;
			float _Value;
		
              fixed4 frag (v2f i) : SV_Target
            {
                fixed4 orCol = tex2D(_MainTex, i.uv);
                float depthValue = Linear01Depth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)));
                float depthValueMul = 0.5;
                fixed4 fogCol = tex2D(_EffectTex, i.uv*_Value);
				fixed4 finalColor = lerp(orCol, fogCol, fogCol.a * 0.5);
                return (depthValue < 1) ? finalColor: orCol;

            }
            ENDCG
        }
    }
}