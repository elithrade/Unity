Shader "Custom/Terrain" {
	Properties {
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
            float3 worldPosition;
		};

        float minHeight;
        float maxHeight;

        // Function has oto be defined before the surf call
        float inverseLerp(float a, float b, float value)
        {
            // value must be within the range of min and max
            // saturate will clamp the value between 0 and 1
            return saturate((value - a) / (b - a));
        }

        // surf function is called for every pixel that mesh is visible
		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
            float heightPercent = inverseLerp(minHeight, maxHeight, IN.worldPosition.y);
			o.Albedo = heightPercent;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
