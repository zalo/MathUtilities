Shader "Custom/ProjectionSurface" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_ProjTex("ProjectionImage (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows finalcolor:projectedColor

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		uniform float4x4 _projectorMatrix;
		sampler2D _MainTex;
		sampler2D _ProjTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		float3 WorldToViewport(float4x4 camVP, float3 worldPoint) {
			float4 result;
			result.x = camVP[0][0] * worldPoint.x + camVP[0][1] * worldPoint.y + camVP[0][2] * worldPoint.z + camVP[0][3];
			result.y = camVP[1][0] * worldPoint.x + camVP[1][1] * worldPoint.y + camVP[1][2] * worldPoint.z + camVP[1][3];
			result.z = camVP[2][0] * worldPoint.x + camVP[2][1] * worldPoint.y + camVP[2][2] * worldPoint.z + camVP[2][3];
			float num = camVP[3][0] * worldPoint.x + camVP[3][1] * worldPoint.y + camVP[3][2] * worldPoint.z + camVP[3][3];
			num = 1.0 / num;
			result.x *= num;
			result.y *= num;

			result.x = (result.x * 0.5 + 0.5);
			result.y = (result.y * 0.5 + 0.5);

			return result.xyz;
		}

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}

		void projectedColor(Input IN, SurfaceOutputStandard o, inout fixed4 color) {
			float3 worldUV = WorldToViewport(_projectorMatrix, IN.worldPos);
			if (abs(worldUV.x - 0.5f) < 0.5 && abs(worldUV.y - 0.5) < 0.5f && worldUV.z > 0) {
				color = tex2D(_ProjTex, worldUV.xy).rgba;
			}
		}
		ENDCG
	}
	FallBack "Diffuse"
}
