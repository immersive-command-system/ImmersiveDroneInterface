Shader "Custom/ClipVolume" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        // Expose parameters for the minimum x, minimum z,
        // maximum x, and maximum z of the rendered volume.
        _Corners ("Min XZ / Max XZ", Vector) = (-0.15, -0.15, 0.15, 0.15)
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        // Allow back sides of the object to render.
        Cull Off

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Read the min xz/ max xz material properties.
        float4 _Corners;

        void surf (Input IN, inout SurfaceOutputStandard o) {

            // Calculate a signed distance from the clipping volume.
            float2 offset;
            offset = IN.worldPos.xz - _Corners.zw;
            float outOfBounds = max(offset.x, offset.y);
            offset = _Corners.xy - IN.worldPos.xz;
            outOfBounds = max(outOfBounds, max(offset.x, offset.y));
            // Reject fragments that are outside the clipping volume.
            clip(-outOfBounds);

            // Default surface shading.
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    // Note that the non-clipped Diffuse material will be used for shadows.
    // If you need correct shadowing with clipped material, add a shadow pass
    // that includes the same clipping logic as above.
    FallBack "Diffuse"
}