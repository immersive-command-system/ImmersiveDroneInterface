Shader "Custom/XZGrid"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "black" {}
	_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_GridStep("Grid size", Float) = 0.001
		_GridWidth("Grid width", Float) = 0.01
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		sampler2D _MainTex;

	struct Input
	{
		float2 uv_MainTex;
		float3 worldPos;
	};

	half _Glossiness;
	half _Metallic;
	fixed4 _Color;
	float _GridStep;
	float _GridWidth;

	void surf(Input IN, inout SurfaceOutputStandard o) {
		// Albedo comes from a texture tinted by color
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

		// grid overlay
		float2 pos = IN.worldPos.xz / _GridStep;
		float2 f = abs(frac(pos) - .5);
		float2 df = fwidth(pos) * _GridWidth;
		float2 g = smoothstep(-df ,df , f);
		float grid = 1.0 - saturate(g.x * g.y);
		c.rgb = lerp(c.rgb, float3(1,1,1), grid);

		o.Albedo = c.rgb;
		// Metallic and smoothness come from slider variables
		o.Metallic = _Metallic;
		o.Smoothness = _Glossiness;
		o.Alpha = c.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}