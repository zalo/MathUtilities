Shader "Unlit/blitSphere"
{
    Properties
    {
        _Center ("Center", Vector) = (0.5,0.5,0.5,0.0)
        _Radius ("Radius", Range (0.0, 1.0)) = 0.25
    }

    SubShader
    {
        Lighting Off
        Blend One Zero

        Pass
        {
            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"

            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag

            float3 _Center;
            float _Radius;
            float4 frag(v2f_customrendertexture IN) : COLOR
            {
				float4 col = tex3D(_SelfTexture3D, IN.globalTexcoord + float3(0, 0, 0.5/_CustomRenderTextureDepth));
				float newDistance = length(IN.globalTexcoord - _Center.xyz) - _Radius;

				if(newDistance<col.a){
					col.rgb = normalize(IN.globalTexcoord-_Center.xyz);
					col.a = newDistance;
				}
				return col;
            }
            ENDCG
        }
    }
}
