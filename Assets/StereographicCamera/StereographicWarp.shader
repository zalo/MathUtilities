Shader "Unlit/StereographicWarp"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
#pragma exclude_renderers gles
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform float3 _CameraPos;
			uniform float3x4 _CameraRot;
			uniform float4x4 _TopProjection;
			uniform float4x4 _BottomProjection;
			uniform float4x4 _LeftProjection;
			uniform float4x4 _RightProjection;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float2 WorldToViewport(float4x4 camVP, float3 worldPoint)
			{
				float2 result;
				result.x = camVP[0][0] * worldPoint.x + camVP[0][1] * worldPoint.y + camVP[0][2] * worldPoint.z + camVP[0][3];
				result.y = camVP[1][0] * worldPoint.x + camVP[1][1] * worldPoint.y + camVP[1][2] * worldPoint.z + camVP[1][3];
				float num = camVP[3][0] * worldPoint.x + camVP[3][1] * worldPoint.y + camVP[3][2] * worldPoint.z + camVP[3][3];
				num = 1.0 / num;
				result.x *= num;
				result.y *= num;

				result.x = (result.x * 0.5 + 0.5);
				result.y = (result.y * 0.5 + 0.5);

				return result;
			}

			float3 UVToStereographicRay(float x, float y) {
				x = (x * 2.0) - 1.0;
				y = (y * 2.0) - 1.0;
				return float3(
					(2.0 * x) / (1.0 + (x * x) + (y * y)),
					(2.0 * y) / (1.0 + (x * x) + (y * y)),
					-((-1.0 + (x * x) + (y * y)) / (1.0 + (x * x) + (y * y))));
			}
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = fixed4(0, 0, 0, 0);

				float3 worldRay = mul(_CameraRot, UVToStereographicRay(i.uv.x, i.uv.y));
				if (dot(worldRay, mul(_CameraRot, float3(0.0, 0.0, 1.0))) > 0.0) {
					worldRay += _CameraPos;

					float2 worldToViewport = WorldToViewport(_TopProjection, worldRay);
					if (worldToViewport.x > 0.0 && worldToViewport.x<1.0 && worldToViewport.y>0.0 && worldToViewport.y < 1.0) {
						worldToViewport /= 2.0;
						worldToViewport.y += 0.5;
						col = tex2D(_MainTex, worldToViewport);
					}

					worldToViewport = WorldToViewport(_BottomProjection, worldRay);
					if (worldToViewport.x > 0.0 && worldToViewport.x<1.0 && worldToViewport.y>0.0 && worldToViewport.y < 1.0) {
						worldToViewport /= 2.0;
						col = tex2D(_MainTex, worldToViewport);
					}

					worldToViewport = WorldToViewport(_LeftProjection, worldRay);
					if (worldToViewport.x > 0.0 && worldToViewport.x<1.0 && worldToViewport.y>0.0 && worldToViewport.y < 1.0 && i.uv.x < 0.5) {
						worldToViewport /= 2.0;
						worldToViewport.x += 0.5;
						worldToViewport.y += 0.5;
						col = tex2D(_MainTex, worldToViewport);
					}
					
					worldToViewport = WorldToViewport(_RightProjection, worldRay);
					if (worldToViewport.x > 0.0 && worldToViewport.x<1.0 && worldToViewport.y>0.0 && worldToViewport.y < 1.0&& i.uv.x > 0.5) {
						worldToViewport /= 2.0;
						worldToViewport.x += 0.5;
						col = tex2D(_MainTex, worldToViewport);
					}
				}
				return col;
			}
			ENDCG
		}
	}
}
