Shader "Unlit/DistanceFieldTexture"
{
	Properties
	{
		_MainTex ("Texture", 3D) = "white" {}
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		LOD 100

		Pass {
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
			};

			struct v2f {
				float3 camPos : TEXCOORD0;
				float4 lPos : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				float3 lightDir : TEXCOORD3;
				float4 vertex : SV_POSITION;
			};

			sampler3D _MainTex;
			float _Inflation;
			
			v2f vert (appdata v) {
				v2f o;
				o.camPos = mul(unity_WorldToObject, _WorldSpaceCameraPos).xyz;
				o.lPos = v.vertex;
				o.viewDir = ObjSpaceViewDir( v.vertex );
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.lightDir = ObjSpaceLightDir( v.vertex );
				return o;
			}

			float sampleDistanceField(float3 pos) {
				return (tex3D( _MainTex, pos + 0.5).r)-_Inflation;
			}

			float3 calcNormal( in float3 pos ) {
				float2 e = float2(1.0,-1.0)*0.02; //Increasing this number makes the shape look smoother
				return normalize( e.xyy*sampleDistanceField( pos + e.xyy ).x + 
								  e.yyx*sampleDistanceField( pos + e.yyx ).x + 
								  e.yxy*sampleDistanceField( pos + e.yxy ).x + 
								  e.xxx*sampleDistanceField( pos + e.xxx ).x );
			}
			
			//Not my proudest shader; 2am code...
			void frag (v2f i, out fixed4 col:SV_Target, out float depth:SV_DEPTH) {
				//Initialize variables
				float alpha = 0;
				bool valid = true; //A trick to make compilation times faster since "break" breaks compilation speed!
				float4 colorSum = 0.0;
				float3 lightDir = i.lightDir;
				float3 viewDirection = normalize(i.viewDir);

				//Ghetto inside-of-shape support
				float3 startingPos = i.lPos.xyz;
				float3 camPos = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0));
				if(max(max(abs(camPos.x), abs(camPos.y)), abs(camPos.z)) < 0.6) {
					startingPos = camPos;
					alpha += 0.2; //This is like the near clipping plane
				}

				for(int i=1; i<20; i++) {
				  float3 pos = startingPos - (viewDirection*alpha);
				  if(valid && (abs(pos.x)>0.501 || abs(pos.y)>0.501 || abs(pos.z)>0.501)){
						valid = false;
				  }

				  float dist = sampleDistanceField(pos);
				  if(valid && dist < 0.0015) {
						//Once we've hit the surface, calculate the normal and display it
						float brightness = (dot(normalize(lightDir), calcNormal(pos))+0.85)*0.5;
						colorSum = float4(brightness, brightness, brightness, 1.0);

						//Also calculate the depth
						float4 pos_clip = UnityObjectToClipPos(float4(startingPos - (viewDirection*alpha), 1.0));
						depth = pos_clip.z / pos_clip.w;
						valid = false;
				  }
				  alpha += dist;
				}

				col = colorSum;
			}
			ENDCG
		}
	}
}
