Shader "Custom/Terrain" {
	Properties {
        testTexture("Texture", 2D) = "white"
        testScale("Scale", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		const static int maxLayerCount = 8;

		int layerCount;
		float3 baseColours[maxLayerCount];
		float baseStartHeights[maxLayerCount];
		float baseBlends[maxLayerCount];
		float baseColourStrength[maxLayerCount];
		float baseTextureScale[maxLayerCount];

		float minHeight;
		float maxHeight;

        sampler2D testTexture;
        float testScale;

		struct Input {
			float3 worldPos;
            float3 worldNormal;
		};

		float inverseLerp(float a, float b, float value) {
			return saturate((value-a)/(b-a));
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float heightPercent = inverseLerp(minHeight,maxHeight, IN.worldPos.y);
			for (int i = 0; i < layerCount; i ++) {
                float halfBlend = baseBlends[i];
				float drawStrength = inverseLerp(-halfBlend - 1e-4, halfBlend, heightPercent - baseStartHeights[i]);
				o.Albedo = o.Albedo * (1-drawStrength) + baseColours[i] * drawStrength;
			}
            // xz means projecting onto xz plane, along the y axis
            // Blend xz, xy, yz based on normal of each point
            float3 scaledWorldPosition = IN.worldPos / testScale;
            float3 blendNormal = abs(IN.worldNormal);
            // Ensure rgb values not exceeding 1 to produce the correct brightness
            blendNormal /= blendNormal.x + blendNormal.y + blendNormal.z;
            float3 xProjection = tex2D(testTexture, scaledWorldPosition.yz) * blendNormal.x;
            float3 yProjection = tex2D(testTexture, scaledWorldPosition.xz) * blendNormal.y;
            float3 zProjection = tex2D(testTexture, scaledWorldPosition.xz) * blendNormal.z;

            // o.Albedo = xProjection + yProjection + zProjection;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
