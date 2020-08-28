using System.Collections.Generic;
using UnityEngine;

/// <summary> A Class for quickly baking inverted distortion lookup tables to textures. </summary>
public class DistortionInverter {
  public Mesh distortionMesh;
  public RenderTexture distortedTexture;
  public Material uvMaterial;
  public List<Vector3> vertices;
  public List<Vector2> uvs;

  public delegate Vector2 DistortUV(Vector2 uv);

  // Start is called before the first frame update
  public DistortionInverter(Material uvMaterial, int distortionMeshRes = 16, int lookupTableRes = 32) {
    // Construct the distortion mesh
    distortionMesh = new Mesh();
    distortionMesh.name = "Tesellated Grid";

    //Generate the Vertices
    int xSize = distortionMeshRes, ySize = distortionMeshRes;
    vertices = new List<Vector3>((xSize + 1) * (ySize + 1) * 2);
    uvs = new List<Vector2>((xSize + 1) * (ySize + 1) * 2);
    for (int i = 0, y = 0; y <= ySize; y++) {
      for (int x = 0; x <= xSize; x++, i++) {
        vertices.Add(new Vector2((float)x / xSize, (float)y / ySize) - (Vector2.one * 0.5f));
        uvs.Add(new Vector2((float)x / xSize, (float)y / ySize));
      }
    }
    distortionMesh.SetVertices(vertices);
    distortionMesh.SetUVs(0, uvs);

    //Generate the Triangles
    int[] triangles = new int[(xSize * ySize * 6 * 2) +
                             ((xSize + ySize) * 6 * 2)];
    for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
      for (int x = 0; x < xSize; x++, ti += 6, vi++) {
        triangles[ti] = vi;
        triangles[ti + 3] = triangles[ti + 2] = vi + 1;
        triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
        triangles[ti + 5] = vi + xSize + 2;
      }
    }
    distortionMesh.triangles = triangles;
    distortionMesh.RecalculateNormals();
    distortionMesh.UploadMeshData(false);

    distortedTexture = new RenderTexture(lookupTableRes, lookupTableRes, 1, RenderTextureFormat.RGFloat);

    this.uvMaterial = uvMaterial;
  }

  public void updateDistortion(DistortUV distort) {
    for (int i = 0; i < uvs.Count; i++) {
      vertices[i] = distort(uvs[i]) - (Vector2.one * 0.5f);
    }
    distortionMesh.SetVertices(vertices);
    distortionMesh.UploadMeshData(false);

    Blit();
  }

  // Based on https://gist.github.com/nothke/e5214489f5690bffa86e2db1563e6fc9
  protected void Blit() {
    // Create an orthographic matrix (for 2D rendering)
    Matrix4x4 projectionMatrix = Matrix4x4.Ortho(-0.5f, 0.5f, -0.5f, 0.5f, -1f, 1f);
    if (Camera.current != null) { projectionMatrix *= Camera.current.worldToCameraMatrix.inverse; }
    RenderTexture prevRT = RenderTexture.active; // Remember the current RenderTexture
    RenderTexture.active = distortedTexture;     // Set our own RenderTexture as "active".
    uvMaterial.SetPass(0);                       // Set material as "active". Without this, Unity editor will freeze.
    GL.PushMatrix();
    GL.LoadProjectionMatrix(projectionMatrix);   // Push the projection matrix
    GL.Clear(true, true, Color.black);           // Clear the texture
    Graphics.DrawMeshNow(distortionMesh, Matrix4x4.identity); // Draw the mesh!
    GL.PopMatrix();                              // Pop the projection matrix to set it back to the previous one
    RenderTexture.active = prevRT;               // Re-set the RenderTexture to the last used one
  }
}
