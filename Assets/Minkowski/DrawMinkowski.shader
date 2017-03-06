/// A Minkowski Difference renderer based on Aras's examples
/// https://forum.unity3d.com/threads/compute-shaders.148874/#post-1021130
Shader "DX11/DrawMinkowski" {
	Properties{
		_MainTex("", 2D) = "white" {}
		_Sprite("", 2D) = "white" {}
	}
		SubShader{

		Pass
	{
		ZWrite Off ZTest Always Cull Off Fog{ Mode Off }

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 5.0
#include "UnityCG.cginc"

	struct appdata {
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

	v2f vert(appdata v)
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord;
		return o;
	}

	sampler2D _MainTex;
	AppendStructuredBuffer<float2> pointBufferOutput : register(u1);


	fixed4 frag(v2f i) : COLOR0
	{
		fixed4 c = tex2D(_MainTex, i.uv);
		[branch]
		if (c.r > 0.5 && c.g < 0.5){
			pointBufferOutput.Append(i.uv);
		}
		return c;
	}
		ENDCG
	}


		Pass{

		ZWrite Off ZTest Always Cull Off Fog{ Mode Off }
		Blend SrcAlpha One

		CGPROGRAM
#pragma vertex vert
#pragma geometry geom
#pragma fragment frag

#include "UnityCG.cginc"

		StructuredBuffer<float2> pointBuffer;

		struct vs_out {
			float4 pos : SV_POSITION;
		};

		vs_out vert(uint id : SV_VertexID)
		{
			vs_out o;
			o.pos = float4(pointBuffer[id] * 2.0 - 1.0, 0, 1);
			return o;
		}

		struct gs_out {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
		};

		float _Size;

		float4 _Corner1;
		float4 _Corner2;
		float4 _Corner3;
		float4 _Corner4;

		[maxvertexcount(4)]
		void geom(point vs_out input[1], inout TriangleStream<gs_out> outStream)
		{
			float dx = _Size;
			float dy = _Size * _ScreenParams.x / _ScreenParams.y;
			gs_out output;
			float4 newPos = float4(-input[0].pos.x, -input[0].pos.y, input[0].pos.z, input[0].pos.w);
			output.pos = (newPos + _Corner1); output.uv = float2(0, 0); outStream.Append(output);
			output.pos = (newPos + _Corner4); output.uv = float2(1, 0); outStream.Append(output);
			output.pos = (newPos + _Corner2); output.uv = float2(0, 1); outStream.Append(output);
			output.pos = (newPos + _Corner3); output.uv = float2(1, 1); outStream.Append(output);
			outStream.RestartStrip();
		}

		sampler2D _Sprite;
		fixed4 _Color;

		fixed4 frag(gs_out i) : COLOR0
		{
			fixed4 col = tex2D(_Sprite, i.uv);
			return fixed4(0, col.g, 0, col.a);
		}

	ENDCG

	}

	}

		Fallback Off
}
