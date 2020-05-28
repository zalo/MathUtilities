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
				float4 vertex : SV_POSITION;
				float4 lPos : TEXCOORD0;
				float3 viewDir : TEXCOORD1;
				float3 lightDir : TEXCOORD2;
				float3 camPos : TEXCOORD3;
			};

			sampler3D _MainTex;
			float _Inflation;
			
			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos( v.vertex );
				o.lPos = v.vertex;
				o.viewDir = ObjSpaceViewDir( v.vertex );
				o.lightDir = ObjSpaceLightDir( v.vertex );
				o.camPos = mul(unity_WorldToObject, _WorldSpaceCameraPos).xyz;
				return o;
			}

			float sampleDistanceField(float3 pos) {
				return (tex3D( _MainTex, pos + 0.5).a)-_Inflation;
			}
			
			//Not my proudest shader; 2am code...
			void frag (v2f i, out fixed4 col:SV_Target, out float depth:SV_DEPTH) {
				//Initialize variables
				float alpha = 0;
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
				
				int it = 0;
				for(it=0; it<50; it++) {
				  float3 pos = startingPos - (viewDirection*alpha);
				  if(abs(pos.x)>0.501 || abs(pos.y)>0.501 || abs(pos.z)>0.501){
						break;
				  }
					
				  float4 sampledColor = tex3D( _MainTex, pos + 0.5);
				  float dist = sampledColor.a - _Inflation;
				  if(dist < 0.001) {
						//Once we've hit the surface display it with a simple dot product
						float brightness = (dot(normalize(lightDir), (sampledColor.rgb))+0.85)*0.5;
						colorSum = float4(brightness, brightness, brightness, 1.0);

						//Also calculate the depth
						float4 pos_clip = UnityObjectToClipPos(float4(startingPos - (viewDirection*alpha), 1.0));
						depth = pos_clip.z / pos_clip.w;
						break;
				  }
				  alpha += dist;
				}

				col = colorSum;//float4(0.02,0,0,1)*it; // Draw the number of raymarching iterations
			}
			ENDCG
		}
	}
}
