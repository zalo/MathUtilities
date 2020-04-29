Shader "Unlit/MirrorSystem"
{
    Properties { }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
        LOD 100

        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            //Lighting Off 
            ZWrite Off
            ZTest Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata {
              float4 vertex : POSITION;
              float3 normal : NORMAL;
            };

            uniform int       _Reflectors;
            uniform float4x4  _worldToSpheres[16];
            uniform float4x4  _sphereToWorlds[16];
            uniform float     _MajorAxes     [16];
            uniform float     _MinorAxes     [16];
            uniform float     _IsInsides     [16];
            uniform sampler2D _CameraDepthTexture;

            struct v2f {
              float4 vertex   : SV_POSITION;
              float4 lPos     : TEXCOORD0;
              float3 viewDir  : TEXCOORD1;
              float3 normal   : TEXCOORD2;
              float3 camPos   : TEXCOORD4;
              float4 projPos  : TEXCOORD3;
            };

            v2f vert(appdata v) {
                v2f o;
                o.vertex  = UnityObjectToClipPos(v.vertex);
                o.lPos    = v.vertex;
                o.viewDir = ObjSpaceViewDir(v.vertex);
                o.normal  = mul(unity_ObjectToWorld, float4(v.normal, 0.0)).xyz;
                o.camPos  = mul(unity_WorldToObject, _WorldSpaceCameraPos).xyz;

                o.projPos = ComputeScreenPos(o.vertex);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }

            // Tired: A bog standard line/sphere intersection
            float intersectRaySphere(float3 ro, float3 rd, float4 sph, float isInside) {
                float3 oc = ro - sph.xyz;
                float b = dot(oc, rd);
                float c = dot(oc, oc) - sph.w * sph.w;
                float h = b * b - c;
                if (h < 0.0) return 1000.0;
                return -b + (sqrt(h) * sign(isInside));
            }

            // Wired: A Horrible Horrible ray ellipsoid intersection
            // Again, late-night just-want-to-get-it-to-work-code
            // Speed can be vastly improved... later...
            float intersectRayEllipsoid(float3 ro, float3 rd, float4x4 worldToSphere, 
                float4x4 sphereToWorld, float IsInside, float MinorAxis, float MajorAxis, 
                out float3 worldHit, out float3 worldNormal) {
                // Transform ray origin and dir to sphere local space
                float3 localStart   =           mul(worldToSphere, float4(ro, 1));
                float3 localDirec   = normalize(mul(worldToSphere, float4(rd, 0)));

                // Intersect the ray with the sphere in local space
                float             t = intersectRaySphere(localStart, localDirec, float4(0, 0, 0, 0.5), IsInside);

                // Find the position and normal, transform back to world space
                float3 localHit     = localStart + (localDirec * t);
                float3 localNormal  = normalize(localHit);
                float3 scaledNormal = normalize(float3(
                    localNormal.x / ((MinorAxis / 2) * (MinorAxis / 2)),
                    localNormal.y / ((MinorAxis / 2) * (MinorAxis / 2)),
                    localNormal.z / ((MajorAxis / 2) * (MajorAxis / 2))));

                // Output the World Normal
                worldHit     =           mul(sphereToWorld, float4(localHit,     1));
                worldNormal  = normalize(mul(sphereToWorld, float4(scaledNormal, 0)));

                // Output 1000.0 if it didn't hit, but the worldspace ray length if it did
                // Also this is idiotic, but I'm tired.
                float3 offset = worldHit - ro;
                return t == 1000 ? 1000 : sign(dot(offset, rd)) * length(offset);
            }

            fixed4 frag (v2f i) : SV_Target {
                //Initialize the ray for this fragment
                int hit = 0;
                float3 rayOrigin    = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0)); // The ray's starting point
                float3 rayDirection = normalize(-i.viewDir);                                       // The ray's direction
                float3 firstHit     = rayOrigin;
                
                // Use as many bounces as reflectors for the bare minimum backpropagation
                for (int _bounce = 0; _bounce < _Reflectors; _bounce++) {
                    // Intersect the pixel ray against the array of ellipsoidal reflectors
                    float  leastT        = 1000; 
                    float3 bestOrigin    = rayOrigin; 
                    float3 bestDirection = rayDirection; 
                    int    hitThisBounce = 0;
                    for(int i = 0; i < _Reflectors; i++){

                        // Intersect Ray against Ellipsoid
                        float3 tempOrigin, worldNormal;
                        float t = intersectRayEllipsoid(rayOrigin, rayDirection, _worldToSpheres[i],
                            _sphereToWorlds[i], _IsInsides[i], _MinorAxes[i], _MajorAxes[i],
                            tempOrigin, worldNormal);

                        // Check if this is the closest intersection
                        if (t < leastT && t > 0.0) {
                            hitThisBounce = 1;
                            leastT        = t;
                            bestOrigin    = tempOrigin;
                            bestDirection = reflect(rayDirection, worldNormal);
                        }
                    }
                    
                    // Save First Hit for Depth Testing
                    if (!hit && hitThisBounce) { hit = 1; firstHit = bestOrigin; } 
                    // Take the min-distance ray and use it for the next bounce
                    if (hitThisBounce) { 
                        rayOrigin    = bestOrigin; 
                        rayDirection = bestDirection; 
                    } else{ break; } // Don't bother bouncing this ray if it didn't hit anything
                }

                // Use the ray to Sample the Skybox
                // We'll eventually add more interesting things here I hope...
                half3 skyColor = DecodeHDR(UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, rayDirection, 0), unity_SpecCube0_HDR);

                // Calculate whether this fragment is occluded by the depth buffer
                float  sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
                float  partZ  = ComputeScreenPos(UnityObjectToClipPos(firstHit)).w;

                // Return the color
                if (hit && sceneZ > partZ){
                    return fixed4(skyColor, 1.0);
                } else {
                    return fixed4(1.0, 1.0, 1.0, 0.0);
                }
            }
            ENDCG
        }
    }
}
