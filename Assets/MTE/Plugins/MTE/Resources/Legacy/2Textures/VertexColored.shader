Shader "MTE/Legacy/2 Textures/Diffuse VertexColored"
{
	Properties
	{
		_Control ("Control (RGBA)", 2D) = "red" {}
		_Splat0 ("Layer 1", 2D) = "white" {}
		_Splat1 ("Layer 2", 2D) = "white" {}
	}

	CGINCLUDE
		#pragma surface surf Lambert vertex:MTE_SplatmapVert finalcolor:MTE_SplatmapFinalColor finalprepass:MTE_SplatmapFinalPrepass finalgbuffer:MTE_SplatmapFinalGBuffer
		#pragma multi_compile_fog

		struct Input
		{
			float2 tc_Control : TEXCOORD0;
			float4 tc_Splat01 : TEXCOORD1;
			UNITY_FOG_COORDS(2)
			float3 color       : COLOR;
		};

		sampler2D _Control;
		float4 _Control_ST;
		sampler2D _Splat0,_Splat1;
		float4 _Splat0_ST,_Splat1_ST;

		#include "../../MTE Common.cginc"

		void MTE_SplatmapVert(inout appdata_full v, out Input data)
		{
			UNITY_INITIALIZE_OUTPUT(Input, data);
			data.tc_Control.xy = TRANSFORM_TEX(v.texcoord, _Control);
			data.tc_Splat01.xy = TRANSFORM_TEX(v.texcoord, _Splat0);
			data.tc_Splat01.zw = TRANSFORM_TEX(v.texcoord, _Splat1);
			float4 pos = UnityObjectToClipPos(v.vertex);
			UNITY_TRANSFER_FOG(data, pos);
		}

		fixed4 MTE_SplatmapMix(Input IN)
		{
			half4 splat_control = tex2D (_Control, IN.tc_Control.xy);
			fixed4 mixedDiffuse = splat_control.r * tex2D(_Splat0, IN.tc_Splat01.xy);
			mixedDiffuse += splat_control.g * tex2D(_Splat1, IN.tc_Splat01.zw);
			return mixedDiffuse;
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 mixedDiffuse = MTE_SplatmapMix(IN);
			o.Albedo = mixedDiffuse.rgb * IN.color;
			o.Alpha = 1;
		}

	ENDCG
	
	Category
	{
		Tags
		{
			"Queue" = "Geometry-99"
			"RenderType" = "Opaque"
		}
		SubShader//for target 3.0+
		{
			CGPROGRAM
				#pragma target 3.0
			ENDCG
		}
		SubShader//for target 2.0
		{
			CGPROGRAM
			ENDCG
		}
	}

	Fallback "Diffuse"
	CustomEditor "MTE.MTEShaderGUI"
}
