using UnityEngine;

//This script emulates the most basic functionality of a skinned mesh renderer
//(by latching onto one, stealing all of its contained data, and manually skinning and rendering a mesh)
[RequireComponent(typeof(SkinnedMeshRenderer))]
public class LinearBlendSkinning : MonoBehaviour {
  SkinnedMeshRenderer skinnedMeshRenderer;
  Vector3[] vertices, normals, origVertices, origNormals;
  Mesh skinnedMesh;
  Transform[] bones;
  Matrix4x4[] boneMatrices, bindPoses;
  Matrix4x4 vertexMatrix = new Matrix4x4();
  BoneWeight[] boneWeights;

  void Start() {
    skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
    skinnedMeshRenderer.enabled = false;
    boneMatrices = new Matrix4x4[skinnedMeshRenderer.bones.Length];
    bindPoses = new Matrix4x4[skinnedMeshRenderer.bones.Length];
    bones = skinnedMeshRenderer.bones;

    //Have to preallocate all of this because mesh accessors generate garbage
    Mesh mesh = skinnedMeshRenderer.sharedMesh;
    boneWeights = new BoneWeight[mesh.vertices.Length];
    for (int i = 0; i < mesh.vertexCount; i++) {
      boneWeights[i] = mesh.boneWeights[i];
    }
    for (int i = 0; i < boneMatrices.Length; i++) {
      bindPoses[i] = mesh.bindposes[i];
    }
    origVertices = mesh.vertices;
    origNormals = mesh.normals;
    //----------------------------------------------------------------------

    int vertexCount = mesh.vertexCount;
    vertices = new Vector3[vertexCount];
    normals = new Vector3[vertexCount];
    skinnedMesh = new Mesh();
    skinnedMesh.vertices = vertices;
    skinnedMesh.triangles = mesh.triangles;
  }

  void LateUpdate() {
    //Find the delta transform between the current bone transform and its bind transform
    for (int i = 0; i < boneMatrices.Length; i++) {
      boneMatrices[i] = bones[i].localToWorldMatrix * bindPoses[i];
    }

    //Loop through the vertices and calculate their deformation
    for (int i = 0; i < skinnedMesh.vertexCount; i++) {
      Matrix4x4 bm0 = boneMatrices[boneWeights[i].boneIndex0];
      Matrix4x4 bm1 = boneMatrices[boneWeights[i].boneIndex1];
      Matrix4x4 bm2 = boneMatrices[boneWeights[i].boneIndex2];
      Matrix4x4 bm3 = boneMatrices[boneWeights[i].boneIndex3];

      //Calculate the delta transform for this vertex by performing a 
      //weighted average of the delta transforms of the nearest 4 bones
      for (int n = 0; n < 16; n++) {
        vertexMatrix[n] =
            bm0[n] * boneWeights[i].weight0 +
            bm1[n] * boneWeights[i].weight1 +
            bm2[n] * boneWeights[i].weight2 +
            bm3[n] * boneWeights[i].weight3;
      }

      //Apply this delta transform to each vertex and normal
      vertices[i] = vertexMatrix.MultiplyPoint3x4(origVertices[i]);
      normals[i] = vertexMatrix.MultiplyVector(origNormals[i]);
    }

    skinnedMesh.vertices = vertices;
    skinnedMesh.normals = normals;

    //Note that the bones are in world space, not local space, so this mesh has to be drawn "at the origin" to appear in the right place
    Graphics.DrawMesh(skinnedMesh, Matrix4x4.identity, skinnedMeshRenderer.material, 0);
  }
}