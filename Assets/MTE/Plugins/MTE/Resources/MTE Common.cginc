#ifndef MTE_COMMON_CGINC_INCLUDED
#define MTE_COMMON_CGINC_INCLUDED

#ifdef MTE_STANDARD_SHADER
#define Output SurfaceOutputStandard
#else
#define Output SurfaceOutput
#endif

void MTE_SplatmapFinalColor(Input IN, Output o, inout fixed4 color)
{
	color *= o.Alpha;
	UNITY_APPLY_FOG(IN.fogCoord, color);
}

void MTE_SplatmapFinalPrepass(Input IN, Output o, inout fixed4 normalSpec)
{
	normalSpec *= o.Alpha;
}

void MTE_SplatmapFinalGBuffer(Input IN, Output o, inout half4 diffuse, inout half4 specSmoothness, inout half4 normal, inout half4 emission)
{
	diffuse.rgb *= o.Alpha;
	specSmoothness *= o.Alpha;
	normal.rgb *= o.Alpha;
	emission *= o.Alpha;
}


#endif // MTE_COMMON_CGINC_INCLUDED