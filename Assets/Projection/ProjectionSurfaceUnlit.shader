Shader "Unlit/UnlitProjectionSurface" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}
		SubShader {
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass {
			CGPROGRAM
			#pragma exclude_renderers gles
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			uniform float4x4 _projectorMatrix;

			struct appdata {
				float4 vertex : POSITION;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float4 position_in_world_space : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

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
			
			v2f vert (appdata input) {
				v2f output;
				output.pos = UnityObjectToClipPos(input.vertex);
				output.position_in_world_space = mul(unity_ObjectToWorld, input.vertex);
				return output;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				fixed4 col = fixed4(0, 0, 0, 0);

				float3 worldUV = WorldToViewport(_projectorMatrix, i.position_in_world_space);
				if (abs(worldUV.x - 0.5f) < 0.5 && abs(worldUV.y - 0.5) < 0.5f && worldUV.z > 0) {
					col.rgb = tex2D(_MainTex, worldUV.xy).rgb;
				}
				return col;
			}
			ENDCG
		}
	}
}
