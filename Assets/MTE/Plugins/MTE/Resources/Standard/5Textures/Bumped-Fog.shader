Shader "MTE/Standard/5 Textures/Bumped/Fog"
{
	Properties
	{
		_Control ("Control (RGBA)", 2D) = "red" {}
		_ControlExtra ("Control Extra (R)", 2D) = "black" {}
		_Splat0 ("Layer 1", 2D) = "white" {}
		_Splat1 ("Layer 2", 2D) = "white" {}
		_Splat2 ("Layer 3", 2D) = "white" {}
		_Splat3 ("Layer 4", 2D) = "white" {}
		_Splat4 ("Layer 5", 2D) = "white" {}

		_Normal0 ("Normalmap 1", 2D) = "bump" {}
		_Normal1 ("Normalmap 2", 2D) = "bump" {}
		_Normal2 ("Normalmap 3", 2D) = "bump" {}
		_Normal3 ("Normalmap 4", 2D) = "bump" {}
		_Normal4 ("Normalmap 5", 2D) = "bump" {}

		[Gamma] _Metallic0("Metallic 0", Range(0.0, 1.0)) = 0.0
		[Gamma] _Metallic1("Metallic 1", Range(0.0, 1.0)) = 0.0
		[Gamma] _Metallic2("Metallic 2", Range(0.0, 1.0)) = 0.0
		[Gamma] _Metallic3("Metallic 3", Range(0.0, 1.0)) = 0.0
		_Smoothness0("Smoothness 0", Range(0.0, 1.0)) = 1.0
		_Smoothness1("Smoothness 1", Range(0.0, 1.0)) = 1.0
		_Smoothness2("Smoothness 2", Range(0.0, 1.0)) = 1.0
		_Smoothness3("Smoothness 3", Range(0.0, 1.0)) = 1.0
	}

	CGINCLUDE
		#pragma surface surf Standard vertex:MTE_SplatmapVert finalcolor:MTE_SplatmapFinalColor finalprepass:MTE_SplatmapFinalPrepass finalgbuffer:MTE_SplatmapFinalGBuffer
		#pragma multi_compile_fog

		struct Input
		{
			float2 tc_Control  : TEXCOORD0;
			float4 tc_Splat01  : TEXCOORD1;
			float4 tc_Splat23  : TEXCOORD2;
			UNITY_FOG_COORDS(3)
		};

		sampler2D _Control,_ControlExtra;
		float4 _Control_ST,_ControlExtra_ST;
		sampler2D _Splat0,_Splat1,_Splat2,_Splat3,_Splat4;
		float4 _Splat0_ST,_Splat1_ST,_Splat2_ST,_Splat3_ST;
		sampler2D _Normal0,_Normal1,_Normal2,_Normal3,_Normal4;
		float4 _Normal0_ST,_Normal1_ST,_Normal2_ST,_Normal3_ST,_Normal4_ST;

		#define MTE_STANDARD_SHADER
		#include "UnityPBSLighting.cginc"
		#include "../../MTE Common.cginc"

		half _Metallic0;
		half _Metallic1;
		half _Metallic2;
		half _Metallic3;
		half _Smoothness0;
		half _Smoothness1;
		half _Smoothness2;
		half _Smoothness3;

		void MTE_SplatmapVert(inout appdata_full v, out Input data)
		{
			UNITY_INITIALIZE_OUTPUT(Input, data);
			data.tc_Control.xy = TRANSFORM_TEX(v.texcoord, _Control);
			data.tc_Splat01.xy = TRANSFORM_TEX(v.texcoord, _Splat0);
			data.tc_Splat01.zw = TRANSFORM_TEX(v.texcoord, _Splat1);
			data.tc_Splat23.xy = TRANSFORM_TEX(v.texcoord, _Splat2);
			data.tc_Splat23.zw = TRANSFORM_TEX(v.texcoord, _Splat3);
			// Because the UNITY_FOG_COORDS(3) occupied one TEXCOORD, Splat4 have to share texcoord with Splat3 - both using tc_Splat23.zw.
			float4 pos = UnityObjectToClipPos (v.vertex);
			UNITY_TRANSFER_FOG(data, pos);

			v.tangent.xyz = cross(v.normal, float3(0,0,1));
			v.tangent.w = -1;
		}

		fixed4 MTE_SplatmapMix(Input IN, out half4 splat_control, half4 defaultAlpha, inout fixed3 mixedNormal)
		{
			splat_control = tex2D (_Control, IN.tc_Control.xy);
			half4 splat_control_extra = tex2D (_ControlExtra, IN.tc_Control.xy);
			fixed4 mixedDiffuse = 0.0f;
			mixedDiffuse += splat_control.r * tex2D(_Splat0, IN.tc_Splat01.xy) * half4(1.0, 1.0, 1.0, defaultAlpha.r);
			mixedDiffuse += splat_control.g * tex2D(_Splat1, IN.tc_Splat01.zw) * half4(1.0, 1.0, 1.0, defaultAlpha.g);
			mixedDiffuse += splat_control.b * tex2D(_Splat2, IN.tc_Splat23.xy) * half4(1.0, 1.0, 1.0, defaultAlpha.b);
			mixedDiffuse += splat_control.a * tex2D(_Splat3, IN.tc_Splat23.zw) * half4(1.0, 1.0, 1.0, defaultAlpha.a);
			mixedDiffuse += splat_control_extra.r * tex2D(_Splat4, IN.tc_Splat23.zw) * half4(1.0, 1.0, 1.0, defaultAlpha.a);//Splat4 shares Smoothness with Spalt3.

			fixed4 nrm = 0.0f;
			nrm += splat_control.r * tex2D(_Normal0, IN.tc_Splat01.xy);
			nrm += splat_control.g * tex2D(_Normal1, IN.tc_Splat01.zw);
			nrm += splat_control.b * tex2D(_Normal2, IN.tc_Splat23.xy);
			nrm += splat_control.a * tex2D(_Normal3, IN.tc_Splat23.zw);
			nrm += splat_control_extra.r * tex2D(_Normal4, IN.tc_Splat23.zw);
			mixedNormal = UnpackNormal(nrm);

			return mixedDiffuse;
		}

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			half4 splat_control;
			half4 defaultSmoothness = half4(_Smoothness0, _Smoothness1, _Smoothness2, _Smoothness3);
			fixed4 mixedDiffuse = MTE_SplatmapMix(IN, splat_control, defaultSmoothness, o.Normal);
			o.Albedo = mixedDiffuse.rgb;
			o.Alpha = 1;
			o.Smoothness = mixedDiffuse.a;
			o.Metallic = dot(splat_control, half4(_Metallic0, _Metallic1, _Metallic2, _Metallic3));
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

	Fallback "MTE/Standard/5 Textures/Diffuse/Fog"
	CustomEditor "MTE.MTEStandardShaderGUI"
}
