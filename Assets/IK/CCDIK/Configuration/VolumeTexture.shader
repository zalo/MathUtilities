// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Unlit/VolumeTexture"
{
	Properties
	{
		_MainTex ("Texture", 3D) = "white" {}
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
				float4 lPos : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
				float4 vertex : SV_POSITION;
			};

			sampler3D _MainTex;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.lPos = v.vertex;
				o.viewDir = normalize(ObjSpaceViewDir( v.vertex ));
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				return o;
			}

			float4 planeAlignment(float4 screenPos){
				// Plane Alignment
				// get object scale factor
				//NOTE: This assumes the volume will only be UNIFORMLY scaled. Non uniform scale would require tons of little changes.
				float worldstepsize = 0.1;
				float camdist = length( _WorldSpaceCameraPos - mul(unity_ObjectToWorld, float4(float3(0.0, 0.0, 0.0), 1.0)).xyz );
				float planeoffset = screenPos.w / worldstepsize;
				float actoroffset = camdist / worldstepsize;
				planeoffset = frac( planeoffset - actoroffset);

				float3 localcamvec = normalize( mul(unity_WorldToObject, UNITY_MATRIX_IT_MV[2].xyz) );
				float3 offsetvec = localcamvec * 100 * planeoffset;
				return float4(offsetvec, planeoffset * worldstepsize);
			}
			
			void frag (v2f i, out fixed4 col:SV_Target)
			{
				float3 startingPos = i.lPos.xyz;
				//startingPos = planeAlignment(i.screenPos);

				float3 viewDirection = i.viewDir;
				const float alpha = 0.015;
				float colorSum = 0.0;
				for(int i=0; i<100; i++){
				  float3 pos = startingPos - (viewDirection*(i*alpha));
				  if(abs(pos.x)>0.5 || abs(pos.y)>0.5 ||abs(pos.z)>0.5){
						break;
				  }

				  colorSum += clamp(DecodeFloatRGBA(tex3D( _MainTex, pos - 0.5))-0.5, 0.0, 1.0)*0.1;//max(0,(0.5-length(pos))*0.1);//
				}

				col = colorSum;
			}
			ENDCG
		}
	}
}
