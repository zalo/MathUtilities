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
				float4 col = 0;
                col.r = min(tex3D(_SelfTexture3D, IN.globalTexcoord + float3(0, 0, 0.5/_CustomRenderTextureDepth)), //Bilinear/Trilinear filtering introduces a half pixel offset in the Z-Axis
							length(IN.globalTexcoord - _Center.xyz)-_Radius);
				return col;
            }
            ENDCG
        }
    }
}
