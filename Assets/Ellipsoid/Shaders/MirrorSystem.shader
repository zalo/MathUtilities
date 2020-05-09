Shader "Unlit/MirrorSystem"
{
    Properties { 
        _MainTex("Texture", 2D) = "white" {}
    }
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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            uniform int       _Reflectors;
            uniform float     _FocalDistance = 0.2;
            uniform float     _ApertureSize  = 0.01;
            uniform float4x4  _worldToSpheres[16];
            uniform float4x4  _sphereToWorlds[16];
            uniform float     _MajorAxes     [16];
            uniform float     _MinorAxes     [16];
            uniform float     _IsInsides     [16];
            uniform float4    _BoundsMin     [16];
            uniform float4    _BoundsMax     [16];

            uniform int       _Planes;
            uniform float4x4  _planeToWorlds [16];
            uniform float4x4  _worldToPlanes [16];

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

            int castRayAgainstSystem(inout float3 rayOrigin, inout float3 rayDirection, out float3 firstHit) {
                int hit = 0;
                // Use as many bounces as reflectors for the bare minimum backpropagation
                for (int _bounce = 0; _bounce < 10; _bounce++) {
                    // Intersect the pixel ray against the array of ellipsoidal reflectors
                    float  leastT = 1000;
                    float3 bestOrigin = rayOrigin;
                    float3 bestDirection = rayDirection;
                    int    hitThisBounce = 0;
                    for (int i = 0; i < _Reflectors; i++) {

                        // Intersect Ray against the outside of the Ellipsoid
                        float3 tempOrigin, worldNormal;
                        float t = intersectRayEllipsoid(rayOrigin, rayDirection, _worldToSpheres[i],
                            _sphereToWorlds[i], -1.0, _MinorAxes[i], _MajorAxes[i], //_IsInsides[i]
                            tempOrigin, worldNormal);

                        int isInsideBounds = tempOrigin.x > _BoundsMin[i].x && tempOrigin.y > _BoundsMin[i].y && tempOrigin.z > _BoundsMin[i].z &&
                                             tempOrigin.x < _BoundsMax[i].x && tempOrigin.y < _BoundsMax[i].y && tempOrigin.z < _BoundsMax[i].z;

                        if (!isInsideBounds) {
                            t = intersectRayEllipsoid(rayOrigin, rayDirection, _worldToSpheres[i],
                                _sphereToWorlds[i], 1.0, _MinorAxes[i], _MajorAxes[i], //_IsInsides[i]
                                tempOrigin, worldNormal);

                            isInsideBounds = tempOrigin.x > _BoundsMin[i].x && tempOrigin.y > _BoundsMin[i].y && tempOrigin.z > _BoundsMin[i].z &&
                                             tempOrigin.x < _BoundsMax[i].x && tempOrigin.y < _BoundsMax[i].y && tempOrigin.z < _BoundsMax[i].z;
                        }

                        // Check if this is the closest intersection
                        if (t < leastT && t > 0.0 && isInsideBounds) {
                            hitThisBounce = 1;
                            leastT = (t - 0.0000001);//t;
                            bestOrigin = rayOrigin + (rayDirection * (t - 0.000001));//tempOrigin;
                            bestDirection = reflect(rayDirection, worldNormal);
                        }
                    }

                    // Save First Hit for Depth Testing
                    if (!hit && hitThisBounce) { hit = 1; firstHit = bestOrigin; }
                    // Take the min-distance ray and use it for the next bounce
                    if (hitThisBounce) {
                        rayOrigin    = bestOrigin;
                        rayDirection = bestDirection;
                    } else { break; } // Don't bother bouncing this ray if it didn't hit anything
                }
                return hit;
            }

            float4 castRayAgainstQuads(float3 rayOrigin, float3 rayDirection) {
                // Transform ray origin and dir to sphere local space
                float  leastT = 1000;
                float3 bestHit = rayOrigin;
                for(int q = 0; q < _Planes; q++) {
                    float3 localStart =           mul(_worldToPlanes[q], float4(rayOrigin,    1));
                    float3 localDirec = normalize(mul(_worldToPlanes[q], float4(rayDirection, 0)));

                    float  t          = (localStart.z / -localDirec.z);
                    float3 planeHit   =  localStart +  (localDirec * t);


                    if (localStart.z < 0 && localDirec.z > 0 && t < leastT &&
                        planeHit.x < 0.5 && planeHit.x > -0.5 && 
                        planeHit.y < 0.5 && planeHit.y > -0.5) {
                        leastT = t; bestHit = planeHit;
                    }
                }

                if (leastT < 1000) {
                    float2 Pos = floor(bestHit.xy * 25.0);
                    float PatternMask = fmod(Pos.x + 200 + fmod(Pos.y, 2), 2);
                    return float4(PatternMask, PatternMask, PatternMask, 1);//tex2D(_MainTex, TRANSFORM_TEX(float2(bestHit.x + 0.5, bestHit.y + 0.5), _MainTex));
                } else {
                    return float4(DecodeHDR(UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, rayDirection, 0), unity_SpecCube0_HDR), 1.0);
                }
            }

            fixed4 frag (v2f i) : SV_Target {
                //Initialize the ray for this fragment
                float3 rayOrigin    = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0)); // The ray's starting point
                float3 rayDirection = normalize(-i.viewDir);                                       // The ray's direction
                float3 firstHit     = rayOrigin;

                float3 tempDir = rayDirection; float3 tempOrigin = rayOrigin;
                float4 accumulatedColor = float4(0, 0, 0, 0);
                float  sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))), partZ = 0;

                // Only do extra work if the primary ray hits an ellipsoid
                if((castRayAgainstSystem(tempOrigin, tempDir, firstHit)) && (sceneZ > ComputeScreenPos(UnityObjectToClipPos(firstHit)).w)) {
                    // Find the position of the ray on the focal plane
                    float3 cameraForward    = dot(rayDirection, mul((float3x3)unity_CameraToWorld, float3(0, 0, 1)));
                    float3 focalPlane       = rayOrigin + ((rayDirection / cameraForward) * _FocalDistance);

                    // Offset the rayOrigin across the aperture, accumulating rays across the aperture
                    float3 ogOrigin         = rayOrigin; float apertureStep = _ApertureSize / 4;
                    float3 cameraRight      = mul((float3x3)unity_CameraToWorld, float3(1, 0, 0)) * apertureStep;
                    float3 cameraUp         = mul((float3x3)unity_CameraToWorld, float3(0, 1, 0)) * apertureStep;
                    for (int lr = -2; lr <= 2; lr++) {  //int lr = 0, ud = 0;
                        for (int ud = -2; ud <= 2; ud++) {
                            float3 curOrig = ogOrigin + (lr * cameraRight) + (ud * cameraUp);
                            float3 outDir = normalize(focalPlane - curOrig);
                            int hit = castRayAgainstSystem(curOrig, outDir, firstHit);

                            // Use the ray to Sample the Skybox; We'll eventually add more interesting things here I hope...
                            if (hit) {
                                accumulatedColor += castRayAgainstQuads(curOrig, outDir);
                            }
                        }
                    }
                }

                // Return the color
                if (accumulatedColor.a > 0){
                    return accumulatedColor / accumulatedColor.a;
                } else {
                    return fixed4(1.0, 1.0, 1.0, 0.0);
                }
            }
            ENDCG
        }
    }
}
