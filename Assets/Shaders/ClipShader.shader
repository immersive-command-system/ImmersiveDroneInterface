Shader "Custom/ClipShader" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}
	SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200
		Cull Off

		CGPROGRAM

		#pragma surface surf Lambert vertex:vert addshadow
		#pragma target 3.0

		sampler2D _MainTex;
		float _Radius;
		float3 _Origin;

		struct Input {
			float2 uv_MainTex;
			float3 pos_ws : TEXCOORD4;
		};

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);
			//compute object vertices position in world space
			o.pos_ws = mul(unity_ObjectToWorld, v.vertex).xyz;
		}

		void surf(Input i, inout SurfaceOutput o) 
		{
			_Radius = 1;
			_Origin = (0, 0, 0);

			float deltaX = i.pos_ws.x - _Origin.x;
			float deltaZ = i.pos_ws.z - _Origin.z;

			if ((deltaX * deltaX + deltaZ * deltaZ) > (_Radius * _Radius))
			{
			discard;
			}

			fixed4 c = tex2D(_MainTex, i.uv_MainTex);
			//o.Emission = _BoxPos.xyz;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
		
	}
}
