Shader "Example/Slices" {
	Properties{
	  _MainTex("Texture", 2D) = "white" {}
	  _BumpMap("Bumpmap", 2D) = "bump" {}
	  _Strength("Strength",float) = 0 
	_Value ("value",float) = 0
		  _Speed("Speed",float) = 0
	}
		SubShader{
		  Tags { "RenderType" = "Opaque" }
		  Cull Off
		  CGPROGRAM
		  #pragma surface surf Lambert
		  struct Input {
			  float2 uv_MainTex;
			  float2 uv_BumpMap;
			  float3 worldPos;
		  };
		  sampler2D _MainTex;
		  sampler2D _BumpMap;
		  float _Strength;
		  float _Value;
		  float _Speed;
		  void surf(Input IN, inout SurfaceOutput o) {
			  clip(frac((IN.worldPos.y + IN.worldPos.z*0.1) * _Strength+sin(_Time.y * _Speed)) - _Value);
			  o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
			  o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		  }
		  ENDCG
	}
		Fallback "Diffuse"
}