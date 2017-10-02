Shader "Unlit/blitCube"
{
    Properties
    {
        _Center ("Center", Vector) = (0.5,0.5,0.5,0.0)
        _Extent ("Extent", Vector) = (0.15,0.15,0.15,0.0)
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
            float3 _Extent;

			//WHY IS NEAREST POINT ON THE SURFACE OF A CUBE SO HARD
            float4 frag(v2f_customrendertexture IN) : COLOR
            {
				float4 col = tex3D(_SelfTexture3D, IN.globalTexcoord + float3(0, 0, 0.5/_CustomRenderTextureDepth));
				float3 boxSpacePos = (IN.globalTexcoord - _Center.xyz);
				float distMultiplier = 1.0;

				if(abs(boxSpacePos.x) < _Extent.x && 
				   abs(boxSpacePos.y) < _Extent.y && 
				   abs(boxSpacePos.z) < _Extent.z){
					float maxCoord = max(max(abs(boxSpacePos.x), 
											 abs(boxSpacePos.y)), 
											 abs(boxSpacePos.z));
					if(maxCoord == abs(boxSpacePos.x)){
						boxSpacePos.x = sign(boxSpacePos.x) * _Extent.x;
					}else if(maxCoord == abs(boxSpacePos.y)){
						boxSpacePos.y = sign(boxSpacePos.y) * _Extent.y;
					}else if(maxCoord == abs(boxSpacePos.z)){
						boxSpacePos.z = sign(boxSpacePos.z) * _Extent.z;
					}
					distMultiplier *= -1.0;
				} else {
					boxSpacePos = float3(sign(boxSpacePos.x) * min(abs(boxSpacePos.x), _Extent.x), 
										 sign(boxSpacePos.y) * min(abs(boxSpacePos.y), _Extent.y), 
										 sign(boxSpacePos.z) * min(abs(boxSpacePos.z), _Extent.z));
				}

				float3 posOnSurface = (boxSpacePos+_Center.xyz);
				float newDistance = length(IN.globalTexcoord-posOnSurface) * distMultiplier;
				if(newDistance<col.a){
					col.rgb = (IN.globalTexcoord-posOnSurface)/newDistance;
					col.a = newDistance;
				}
				return col;
            }
            ENDCG
        }
    }
}
