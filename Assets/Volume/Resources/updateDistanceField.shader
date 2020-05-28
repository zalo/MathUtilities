Shader "Unlit/blitSphere"
{
    Properties
    {
		[Toggle] _Subtraction("Subtraction", Float) = 0
        _Center ("Center", Vector) = (0.5,0.5,0.5,0.0)
        _Radius ("Radius", Range (0.0, 1.0)) = 0.25
        _Extent ("Extent", Vector) = (0.15,0.15,0.15,0.0)
    }

    SubShader
    {
        Lighting Off
        Blend One Zero

        Pass
        {
			Name "BlitSphere"
            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"

            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag

            float3 _Center;
            float _Radius;
			int _Subtraction;
            float4 frag(v2f_customrendertexture IN) : COLOR
            {
				float3 texCoord = IN.globalTexcoord + float3(0, 0, 0.5/_CustomRenderTextureDepth);
				float4 curCol = tex3D(_SelfTexture3D, texCoord);
				float3 offset = _Center.xyz - texCoord;
				float newDistance = length(offset) - _Radius;
				
				float4 col = curCol;
				if (_Subtraction == 0 && newDistance < curCol.a) {
					col.a = newDistance;
					col.rgb = normalize(-offset);
				} else if (_Subtraction == 1 && -newDistance > curCol.a) {
					col.a = -newDistance;
					col.rgb = normalize(offset);
				}
				return col;
            }
            ENDCG
        }

        Pass
        {
			Name "BlitCube"
            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"

            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag

            float3 _Center;
            float3 _Extent;

			//WHY IS NEAREST POINT ON THE SURFACE OF A CUBE SO HARD
            float4 frag(v2f_customrendertexture IN) : COLOR
            {
				float3 texCoord = IN.globalTexcoord + float3(0, 0, 0.5/_CustomRenderTextureDepth);
				float4 col = tex3D(_SelfTexture3D, texCoord);
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
				float newDistance = length(texCoord-posOnSurface) * distMultiplier;
				if(newDistance<col.a){
					col.rgb = (texCoord-posOnSurface)/newDistance;
					col.a = newDistance;
				}
				return col;
            }
            ENDCG
        }

        Pass
        {
			Name "RepairField"
            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"

            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag

            float4 frag(v2f_customrendertexture IN) : COLOR
            {
				float3 texCoord = IN.globalTexcoord + float3(0, 0, 0.5/_CustomRenderTextureDepth);
				float4 col = tex3D(_SelfTexture3D, texCoord);
				
				//If not already inside of a surface...
				if(col.a > 0.001 && col.a != 1000.0){
					float3 newCoord = texCoord + (col.a * col.rgb);
					float4 newCol = tex3D(_SelfTexture3D, newCoord);
					//If not already pointing to a surface...
					if (newCol.a > 0.0025) {
						float3 newSurface = newCoord + (newCol.a * newCol.rgb);
						float3 newOffsetToSurface = newSurface - texCoord;
						float distToNewSurface = length(newOffsetToSurface);
						//Suppress corruption; we already checked that we're not inside a surface
						if(distToNewSurface > 0.001){
							col.a = distToNewSurface;
							col.rgb = newOffsetToSurface/col.a;
						}
					}
				}
				return col;
            }
            ENDCG
        }

        Pass
        {
			Name "RepairFieldWithPlaneConstraint"
            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"

            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag

            float4 frag(v2f_customrendertexture IN) : COLOR
            {
				float3 texCoord = IN.globalTexcoord + float3(0, 0, 0.5/_CustomRenderTextureDepth);
				float4 col = tex3D(_SelfTexture3D, texCoord);
				
				//If not already inside of a surface...
				if(col.a > 0.001 && col.a != 1000.0){
					float3 newCoord = texCoord + (col.a * col.rgb);
					float4 newCol = tex3D(_SelfTexture3D, newCoord);
					//If not already pointing to a surface...
					if (newCol.a > 0.0025) {
						float3 texCoordOnPlane = texCoord - ((dot(texCoord - newCoord, newCol.rgb) / dot(newCol.rgb, newCol.rgb)) * newCol.rgb);
						float4 planeValue = tex3D(_SelfTexture3D, texCoordOnPlane);
						float3 SurfaceFromPlane = texCoordOnPlane + (planeValue.a * planeValue.rgb);
						float3 newOffsetToSurface = SurfaceFromPlane - texCoord;
						float distToNewSurface = length(newOffsetToSurface);
						//Suppress corruption; we already checked that we're not inside a surface
						if(distToNewSurface > 0.001){
							col.a = distToNewSurface;
							col.rgb = newOffsetToSurface/col.a;
						}
					}
				}
				return col;
            }
            ENDCG
        }
    }
}
