Shader "Unlit/ParallaxBarrier"
{
	Properties
	{
	}
	SubShader
	{
        Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
        LOD 100
        ZWrite Off
        Lighting Off
        Fog { Mode Off }

        Blend SrcAlpha OneMinusSrcAlpha 

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
                //256 Vertical Black Bars
                float increment = 1.0/255.5;
                float barrierValue = (fmod(i.uv.x + (increment*2001), increment) - (increment*0.5)) * 100000.0;

				// sample the texture
				fixed4 col = fixed4(0.0,0.0,0.0,1.0);
                clip(barrierValue-0.5);
				return col;
			}
			ENDCG
		}
	}
}
