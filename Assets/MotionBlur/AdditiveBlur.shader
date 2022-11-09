// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/AdditiveBlur"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _numSamples("Num Samples", Range(1,100)) = 1.0
    }
    SubShader
    {
            Tags {Queue = Transparent}
            Blend One One
            ZWrite Off
            LOD 200

        Pass {
            Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
            //Blend One One
            Blend DstColor Zero // Multiplicative
            //Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                //UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            uniform float _numSamples;

            v2f vert(appdata_full v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                // sample the texture
                fixed4 col = fixed4(1.0, 1.0, 1.0, 1.0) * (1.0 - (1.0/_numSamples));
                //fixed4 col = fixed4(1.0, 1.0, 1.0, 1.0) * (1.0 / -_numSamples);
                return col;
            }
            ENDCG
        }


        Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
        Blend One One
        //ZWrite Off
        LOD 200

        CGPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
            #pragma surface surf Standard finalcolor:dimFromBlur fullforwardshadows

            uniform float _numSamples;

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0

            sampler2D _MainTex;

            struct Input { float2 uv_MainTex; };

            inline void dimFromBlur(Input IN, SurfaceOutputStandard  o, inout fixed4 color) {
                color = clamp(color, 0.0, 1.0) / _numSamples;
            }

            half _Glossiness;
            half _Metallic;
            fixed4 _Color;

            void surf(Input IN, inout SurfaceOutputStandard  o)
            {
                // Albedo comes from a texture tinted by color
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;// / _numSamples;
                // Metallic and smoothness come from slider variables
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
                o.Alpha = c.a;
            }
        ENDCG
    }
    FallBack "Diffuse"
}
