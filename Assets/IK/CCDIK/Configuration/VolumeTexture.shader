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
				float3 lightDir : TEXCOORD3;
				float4 vertex : SV_POSITION;
			};

			sampler3D _MainTex;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.lPos = v.vertex;
				o.viewDir = ObjSpaceViewDir( v.vertex );
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.lightDir = ObjSpaceLightDir( v.vertex );
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

			float sampleDistanceField(float3 pos){
				return DecodeFloatRGBA(tex3D( _MainTex, pos - 0.5))-0.5;
			}

			float3 calcNormal( in float3 pos ) {
				float2 e = float2(1.0,-1.0)*0.5773*0.005;
				return normalize( e.xyy*sampleDistanceField( pos + e.xyy ).x + 
								  e.yyx*sampleDistanceField( pos + e.yyx ).x + 
								  e.yxy*sampleDistanceField( pos + e.yxy ).x + 
								  e.xxx*sampleDistanceField( pos + e.xxx ).x );
			}

			 float rand(float3 co) {
				 return frac(sin( dot(co.xyz ,float3(12.9898,78.233,45.5432) )) * 43758.5453);
			 }
			
			void frag (v2f i, out fixed4 col:SV_Target)
			{
				float3 startingPos = i.lPos.xyz;
				float3 lightDir = i.lightDir;
				//startingPos = planeAlignment(i.screenPos);

				float3 viewDirection = normalize(i.viewDir);
				float alpha = (rand(startingPos)*0.006)+0.02;
				float4 colorSum = 0.0;
				bool valid = true; //A trick to make compilation times faster since "break" breaks compilation speed

				for(int i=1; i<40; i++){
				  float3 pos = startingPos - (viewDirection*(i*alpha));
				  if(valid && (abs(pos.x)>0.499 || abs(pos.y)>0.499 ||abs(pos.z)>0.499)){
						valid = false;
				  }

				  float dist = sampleDistanceField(pos);
				  alpha += dist*0.006;
				  if(valid && dist < 0.001){
						float brightness = (dot(normalize(lightDir), calcNormal(pos))+0.85)*0.5;
						colorSum = float4(brightness, brightness, brightness, 1.0);
						valid = false;
				  }

				  //colorSum += clamp(sampleDistanceField(pos), 0.0, 1.0)*0.1;//max(0,(0.5-length(pos))*0.1);//
				}

				col = colorSum;
			}
			ENDCG
		}
	}
}
