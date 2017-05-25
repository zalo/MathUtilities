Shader "Unlit/Autostereo"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 wPos : TEXCOORD1;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 wPos : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

            uniform float4 _eyes[3];
            uniform float4 _parallaxBarrier[3];
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

            float3 intersectPlane(float3 planeNormal, float3 planePos, float3 rayOrigin, float3 rayDirection) 
            { 
                float t = dot(planePos - rayOrigin, planeNormal) / dot(rayDirection, planeNormal);
                return rayOrigin + (rayDirection * t);
            }
			
			fixed4 frag (v2f i) : SV_Target
			{
                float3 leftHitPosition = intersectPlane(normalize(cross(_parallaxBarrier[2], _parallaxBarrier[1])), _parallaxBarrier[0], _eyes[0], normalize(i.wPos - _eyes[0]));
                float2 leftEyeBarrierUV = float2(dot(_parallaxBarrier[1], leftHitPosition - _parallaxBarrier[0]) / dot(_parallaxBarrier[1], _parallaxBarrier[1]),
                                                 dot(_parallaxBarrier[2], leftHitPosition - _parallaxBarrier[0]) / dot(_parallaxBarrier[2], _parallaxBarrier[2]));

                float3 rightHitPosition = intersectPlane(normalize(cross(_parallaxBarrier[2], _parallaxBarrier[1])), _parallaxBarrier[0], _eyes[1], normalize(i.wPos - _eyes[1]));
                float2 rightEyeBarrierUV = float2(dot(_parallaxBarrier[1], rightHitPosition - _parallaxBarrier[0]) / dot(_parallaxBarrier[1], _parallaxBarrier[1]),
                                                  dot(_parallaxBarrier[2], rightHitPosition - _parallaxBarrier[0]) / dot(_parallaxBarrier[2], _parallaxBarrier[2]));

                //256 Vertical Black Bars
                float increment = 1.0/255.5;
                float leftBarrierValue = (fmod(leftEyeBarrierUV.x + (increment*2001), increment) - (increment*0.5)) * 100000.0;
                float rightBarrierValue = (fmod(rightEyeBarrierUV.x + (increment*2001), increment) - (increment*0.5)) * 100000.0;

                //Normalize the left/right image balance
                float LeftRightBlendAlpha = rightBarrierValue / (leftBarrierValue + rightBarrierValue);

				//Just display the left and right values instead
				fixed4 col = fixed4(leftBarrierValue, rightBarrierValue, 0.0, 0.0);
				return col;
			}
			ENDCG
		}
	}
}
