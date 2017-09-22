Shader "MTE/Legacy/3 Textures/Bumped"
{
	Properties
	{
		_Control ("Control (RGBA)", 2D) = "red" {}
		_Splat0 ("Layer 1", 2D) = "white" {}
		_Splat1 ("Layer 2", 2D) = "white" {}
		_Splat2 ("Layer 3", 2D) = "white" {}

		_Normal0 ("Normalmap 1", 2D) = "bump" {}
		_Normal1 ("Normalmap 2", 2D) = "bump" {}
		_Normal2 ("Normalmap 3", 2D) = "bump" {}
	}

	CGINCLUDE
		#pragma surface surf Lambert vertex:MTE_SplatmapVert finalcolor:MTE_SplatmapFinalColor finalprepass:MTE_SplatmapFinalPrepass finalgbuffer:MTE_SplatmapFinalGBuffer
		#pragma multi_compile_fog

		struct Input
		{
			float4 tc_ControlSplat0 : TEXCOORD0;
			float4 tc_Splat12 : TEXCOORD1;
			UNITY_FOG_COORDS(2)
		};

		sampler2D _Control;
		float4 _Control_ST;
		sampler2D _Splat0,_Splat1,_Splat2;
		float4 _Splat0_ST,_Splat1_ST,_Splat2_ST;
		sampler2D _Normal0,_Normal1,_Normal2;
		float4 _Normal0_ST,_Normal1_ST,_Normal2_ST;

		#include "../../MTE Common.cginc"

		void MTE_SplatmapVert(inout appdata_full v, out Input data)
		{
			UNITY_INITIALIZE_OUTPUT(Input, data);
			data.tc_ControlSplat0.xy = TRANSFORM_TEX(v.texcoord, _Control);
			data.tc_ControlSplat0.zw = TRANSFORM_TEX(v.texcoord, _Splat0);
			data.tc_Splat12.xy = TRANSFORM_TEX(v.texcoord, _Splat1);
			data.tc_Splat12.zw = TRANSFORM_TEX(v.texcoord, _Splat2);
			float4 pos = UnityObjectToClipPos (v.vertex);
			UNITY_TRANSFER_FOG(data, pos);

			v.tangent.xyz = cross(v.normal, float3(0,0,1));
			v.tangent.w = -1;
		}

		void MTE_SplatmapMix(Input IN, out fixed4 mixedDiffuse, inout fixed3 mixedNormal)
		{
			half4 splat_control = tex2D (_Control, IN.tc_ControlSplat0.xy);
			mixedDiffuse = splat_control.r * tex2D(_Splat0, IN.tc_ControlSplat0.zw);
			mixedDiffuse += splat_control.g * tex2D(_Splat1, IN.tc_Splat12.xy);
			mixedDiffuse += splat_control.b * tex2D(_Splat2, IN.tc_Splat12.zw);

			fixed4 nrm = 0.0f;
			nrm += splat_control.r * tex2D(_Normal0, IN.tc_ControlSplat0.zw);
			nrm += splat_control.g * tex2D(_Normal1, IN.tc_Splat12.xy);
			nrm += splat_control.b * tex2D(_Normal2, IN.tc_Splat12.zw);
			mixedNormal = UnpackNormal(nrm);
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 mixedDiffuse;
			MTE_SplatmapMix(IN, mixedDiffuse, o.Normal);
			o.Albedo = mixedDiffuse.rgb;
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

	Fallback "MTE/Legacy/3 Textures/Diffuse"
	CustomEditor "MTE.MTEShaderGUI"
}
