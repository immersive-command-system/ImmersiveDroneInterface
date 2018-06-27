// UNITY_SHADER_NO_UPGRADE
Shader "Custom/TransparentSelected"
{
	Properties{
		_Color("Rim Color", Color) = (0.5,0.5,0.5,0.5)
		_FPOW("FPOW Fresnel", Float) = 5.0
		_R0("R0 Fresnel", Float) = 0.05
		_MainTex("Bumpmap", 2D) = "bump" {}
	}

	Category{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite On

		SubShader{
			
			LOD 200
			Cull Off

			CGPROGRAM

			#pragma surface surf Lambert alpha vertex:vert addshadow
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

			Pass{

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0
				#include "UnityCG.cginc"

				sampler2D _MainTex;
				fixed4 _Color;
				float _FPOW;
				float _R0;
				float _Radius;
				float3 _Origin;

				struct appdata_t {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					float3 normal : NORMAL;
				};

				struct v2f {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				float4 _MainTex_ST;

				v2f vert(appdata_t v)
				{
					v2f o;
			#if UNITY_VERSION >= 560
					o.vertex = UnityObjectToClipPos(v.vertex);
			#else
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
			#endif
					o.color = v.color;
					o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);

					float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
					half fresnel = saturate(1.0 - dot(v.normal, viewDir));
					fresnel = pow(fresnel, _FPOW);
					fresnel = _R0 + (1.0 - _R0) * fresnel;
					o.color *= fresnel;
					return o;
				}

				fixed4 frag(v2f i) : COLOR
				{
					return 2.0f * i.color * _Color * tex2D(_MainTex, i.texcoord);
				}

				ENDCG
			}
		}
	}
}