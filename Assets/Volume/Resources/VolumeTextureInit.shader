Shader "CustomRenderTexture/CustomVolumeTextureInit"
{
    Properties
    {
    }

    SubShader
    {
        Lighting Off
        Blend One Zero

        Pass
        {
            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"

            #pragma vertex InitCustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0

            float4 frag(v2f_init_customrendertexture IN) : COLOR
            {
                return 1000.0;
            }
            ENDCG
        }
    }
}