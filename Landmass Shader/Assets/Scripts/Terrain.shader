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
		float baseTextureScales[maxLayerCount];

		float minHeight;
		float maxHeight;

        sampler2D testTexture;
        float testScale;

        UNITY_DECLARE_TEX2DARRAY(baseTextures);

		struct Input {
			float3 worldPos;
            float3 worldNormal;
		};

		float inverseLerp(float a, float b, float value) {
			return saturate((value-a)/(b-a));
		}

        float3 triplanar(float3 worldPos, float scale, float3 blendAxes, int textureIndex)
        {
            // xz means projecting onto xz plane, along the y axis
            // Blend xz, xy, yz based on normal of each point
            float3 scaledWorldPosition = worldPos / scale;
            float3 xProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPosition.y, scaledWorldPosition.z, textureIndex)) * blendAxes.x;
            float3 yProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPosition.x, scaledWorldPosition.z, textureIndex)) * blendAxes.y;
            float3 zProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPosition.x, scaledWorldPosition.y, textureIndex)) * blendAxes.z;

            return xProjection + yProjection + zProjection;
        }

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float heightPercent = inverseLerp(minHeight,maxHeight, IN.worldPos.y);
            float3 blendAxes = abs(IN.worldNormal);
            // Ensure rgb values not exceeding 1 to produce the correct brightness
            blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;

			for (int i = 0; i < layerCount; i ++) {
                float halfBlend = baseBlends[i];
				float drawStrength = inverseLerp(-halfBlend - 1e-4, halfBlend, heightPercent - baseStartHeights[i]);
                float3 baseColour = baseColours[i] * baseColourStrength[i];
                float3 textureColour = triplanar(IN.worldPos, baseTextureScales[i], blendAxes, i) * (1 - baseColourStrength[i]);

				o.Albedo = o.Albedo * (1-drawStrength) + (baseColour + textureColour) * drawStrength;
			}
		}
		ENDCG
	}
	FallBack "Diffuse"
}
