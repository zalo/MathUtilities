Shader "Custom/MagneticField" {
    Properties
    {
      //_MainTex         ("Texture",                            3D) = "white" {}
      _AlphaExponent   ("Alpha Exponent",     Range(0.5,     10)) = 2
      _RangeScalar     ("Range Scalar",       Range(0.001, 0.1)) = 0.015
      _ColorRangeOffset("Color Range Offset", Range(0.0,  100.0)) = 20.0
      _AlphaRangeOffset("Alpha Range Offset", Range(0.0,  100.0)) = 20.0
    }
        SubShader{
          Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
          LOD 100

          Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
          //Lighting Off 
          ZWrite Off
          ZTest Off

          CGPROGRAM

          // More raysteps reduces noise but increases compute
          #define RAY_STEPS 20

          #pragma vertex vert
          #pragma fragment frag
          // make fog work
          #pragma multi_compile_fog

          #include "UnityCG.cginc"
          sampler2D _CameraDepthTexture;

          uniform float4 _WirePoints [256];
          uniform float  _WireCurrent[256];

          float sdCapsule(float3 p, float3 a, float3 b, float r) {
              float3 pa = p - a, ba = b - a;
              float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
              return length(pa - ba * h) - r;
          }

          float3 fieldAroundWire(float3 p, float3 a, float3 b, float current) {
              float3 IdL   = (b - a) * current;                 // Current * WireLength
              float3 R     = (b + a) * 0.5;                     // Midpoint of the Wire
              float rNorm  = length(p - R);                     // Distance from Midpoint to Sample
              float3 r3    = (p - R) / (rNorm * rNorm * rNorm); // Normalize Offset from Midpoint to Sample by cube of magnitude
              float3 field = cross(IdL, r3);                    // Cross the (Current*Length) with r3

              //float3 wireDir  = normalize(b - a);
              //float3 fieldDir = normalize(cross(wireDir, p - ((b + a) * 0.5)));
              //float dot1 = dot(normalize(p - a),  wireDir);  // This is based on some random website
              //float dot2 = dot(normalize(p - b), -wireDir);  // Not sure which is the right way of doing it
              //float fieldIntensity = dot1 + dot2;            // Am very confused
              //float3 field = fieldDir * current * fieldIntensity;    // * fieldIntensity * fieldIntensity ?

              return field;
          }

          float3 fieldAtPosition(float3 p) {
              float3 sumField = float3(0, 0, 0);
              for (int i = 0; i < 255; i++) {
                  sumField += fieldAroundWire(p, _WirePoints[i].xyz, _WirePoints[i + 1].xyz, _WireCurrent[i]);
              }
              return sumField;
          }

          struct appdata {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
          };

          struct v2f {
            float4 vertex   : SV_POSITION;
            float4 lPos     : TEXCOORD0;
            float3 viewDir  : TEXCOORD1;
            float3 normal   : TEXCOORD2;
            float3 camPos   : TEXCOORD4;
            float4 projPos  : TEXCOORD3;
          };

          // A fast approximation of Google's JET Color Pallette
          float3 turbo(in float t) {
            const float3 a = float3(0.13830712, 0.49827032, 0.47884378);
            const float3 b = float3(0.8581176, -0.50469547, 0.40234273);
            const float3 c = float3(0.67796707, 0.84353134, 1.111561);
            const float3 d = float3(0.50833158, 1.11536091, 0.76036415);
            float3       tt = clamp(float3(t,t,t), float3(0.375, 0.0, 0.0), float3(1.0, 1.0, 0.7));
            return a + b * cos(6.28318 * (c * tt + d));
          }

          float rand(float2 co) {
            return frac(sin(dot(co.xy * (sin(_Time)+10.0), float2(12.9898, 78.233))) * 43758.5453);
          }

          //sampler3D _MainTex;
          int _DrawAbs;
          float _AlphaExponent;
          float _RangeScalar;
          float _ColorRangeOffset;
          float _AlphaRangeOffset;

          v2f vert(appdata v) {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.lPos = v.vertex;
            o.viewDir = ObjSpaceViewDir(v.vertex);
            o.normal = mul(unity_ObjectToWorld, float4(v.normal, 0.0)).xyz;
            o.camPos = mul(unity_WorldToObject, _WorldSpaceCameraPos).xyz;

            o.projPos = ComputeScreenPos(o.vertex);
            COMPUTE_EYEDEPTH(o.projPos.z);
            return o;
          }

          //Not my proudest shader; 2am code...
          fixed4 frag(v2f i) : COLOR{//, out fixed4 col:SV_Target){//, out float depth:SV_DEPTH) {
            //Initialize variables
            float4 colorSum = 0.0;                       // The Accumulated Color for each ray
            float  rayAlpha = 0.001;                     // The Current Ray Progress through the volume
            float  rayStep = 1.7 / RAY_STEPS;            // The Step Size through the volume
            float3 startingPos = i.lPos.xyz;             // The ray's starting point
            float3 viewDirection = normalize(i.viewDir); // The ray's direction
            float3 camPos = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0));

            //float3 camDir             = mul((float3x3)unity_CameraToWorld, float3(0, 0, 1));
            //float3 lightDir           = i.lightDir; // Unused, but could be in the future...

            // Add Inside-of-Shape support
            if (dot(i.normal, viewDirection) < 0) {
              startingPos = camPos;
              rayAlpha += 0.05; //This is like the near clipping plane
            }

            float4 screenPos = ComputeScreenPos(UnityObjectToClipPos(startingPos - (viewDirection * rayAlpha)));
            //screenPos /= screenPos.w;

            // Add ray step noise to hide the stepping artifacts with noise...
            rayAlpha += rand(screenPos.xy) * rayStep; //

            float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
            float rayStepScalar = (500 / RAY_STEPS);

            // Step through the volume, have each ray accumulate acoustic intensity
            int it = 0;
            for (it = 0; it < RAY_STEPS; it++) {
              float3 pos = startingPos - (viewDirection * rayAlpha); // Increment the ray march

              float partZ = ComputeScreenPos(UnityObjectToClipPos(pos)).w;

              if (!(sceneZ < partZ || abs(pos.x) > 0.501 || abs(pos.y) > 0.501 || abs(pos.z) > 0.501)) {
                float3 fieldVec          = fieldAtPosition(pos);
                float sampledField       = length(fieldVec);//tex3D(_MainTex, pos + 0.5).r;
                float  acousticIntensity = max(0.0, sampledField);
                float  squaredAlpha      = pow(max(0.0, acousticIntensity - _AlphaRangeOffset) * _RangeScalar, _AlphaExponent);
                float3 coloredIntensity  = turbo((      acousticIntensity - _ColorRangeOffset) * _RangeScalar); // fieldVec;
                colorSum                += float4(coloredIntensity * squaredAlpha, squaredAlpha) * rayStepScalar;
                rayAlpha                += rayStep;
              }
            }

            // Extract the average color
            float3 normalizedColor = colorSum.rgb / colorSum.a;

            // Map the Pressure Intensity along this pixel to a Color
            return fixed4(normalizedColor, colorSum.a);
          }
          ENDCG
        }
      }
}