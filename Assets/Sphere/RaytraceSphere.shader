Shader "Unlit/RaytraceSphere"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_SquareRadius ("Square Radius", Range(0,0.25)) = 0.25
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		LOD 100

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float3 wPos : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			fixed4 _Color;
			half _SquareRadius;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 intersectSphere (float3 position, float3 direction, float sqRadius) 
			{
				float3 sphereCenter = mul(unity_ObjectToWorld, float4(float3(0.0, 0.0, 0.0), 1.0)).xyz;
				float3 oc = position - sphereCenter;
				float b = dot(direction, oc);
				float c = dot(oc, oc) - sqRadius;
				float t = b*b - c;
				//if( t > 0.0) {
					t = -b - sqrt(t);
				//}

				float3 posOnSphere = position + (direction * t);
				float shading = saturate(dot(normalize(posOnSphere - sphereCenter), _WorldSpaceLightPos0.xyz)) + 0.1; //This line is 5 instructions :(
				return fixed4(posOnSphere, shading);
			}
			
			void frag (v2f i, out fixed4 col:SV_Target, out float depth:SV_DEPTH)
			{
				float3 viewDirection = normalize(i.wPos - _WorldSpaceCameraPos);
				float4 spherePos = intersectSphere(_WorldSpaceCameraPos, viewDirection, _SquareRadius);
				float4 pos_clip = mul(UNITY_MATRIX_VP, float4(spherePos.xyz, 1.0));
				depth = pos_clip.z / pos_clip.w;
				col = float4(spherePos.a, spherePos.a, spherePos.a, saturate(depth*1000.0)) * _Color;
			}
			ENDCG
		}
	}
}
