using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ThickPlaneGenerator : MonoBehaviour {
  Mesh mesh;
  public int xSize = 180, ySize = 180;
  Vector3[] vertices;

  private void Awake() {
    Generate();
  }

  private void Generate() {
    GetComponent<MeshFilter>().mesh = mesh = new Mesh();
    mesh.name = "Procedural Grid";

    //Generate the Vertices
    vertices = new Vector3[(xSize + 1) * (ySize + 1) * 2];
    for (int i = 0, y = 0; y <= ySize; y++) {
      for (int x = 0; x <= xSize; x++, i++) {
        vertices[i] = new Vector3(x/10f, y/10f);
        vertices[i + (vertices.Length/2)] = new Vector3(x/10f, y/10f, 1f);
      }
    }
    mesh.vertices = vertices;


    //Generate the Triangles
    int[] triangles = new int[(xSize * ySize * 6 * 2) + ((xSize + ySize) * 6 * 2)];
    //Front Face
    for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
      for (int x = 0; x < xSize; x++, ti += 6, vi++) {
        triangles[ti] = vi;
        triangles[ti + 3] = triangles[ti + 2] = vi + 1;
        triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
        triangles[ti + 5] = vi + xSize + 2;
      }
    }
    
    //Back Face
    for (int ti = ((xSize) * (ySize))*6, vi = ((xSize) * (ySize + 1)) + ySize + 1, y = 0; y < ySize; y++, vi++) {
      for (int x = 0; x < xSize; x++, ti += 6, vi++) {
        triangles[ti] = vi;
        triangles[ti + 4] = triangles[ti + 1] = vi + 1;
        triangles[ti + 3] = triangles[ti + 2] = vi + xSize + 1;
        triangles[ti + 5] = vi + xSize + 2;
      }
    }

    //Bottom Side
    for (int ti = (xSize * ySize * 6 * 2), x = 0; x < xSize; x++, ti += 6) {
      triangles[ti] = triangles[ti + 5] = x;
      triangles[ti + 1] = triangles[ti + 4] = x + ((xSize + 1) * (ySize + 1)) + 1;
      triangles[ti + 2] = x + ((xSize + 1) * (ySize + 1));
      triangles[ti + 3] = x + 1;
    }

    //Top Side
    for (int ti = ((xSize * ySize * 2) + xSize) * 6, vi = ((xSize + 1) * ySize), x = 0; x < xSize; x++, ti += 6, vi++) {
      triangles[ti + 1] = triangles[ti + 4] = vi;
      triangles[ti] = triangles[ti + 5] = vi + ((xSize + 1) * (ySize + 1)) + 1;
      triangles[ti + 2] = vi + ((xSize + 1) * (ySize + 1));
      triangles[ti + 3] = vi + 1;
    }

    //Left Side
    for (int ti = ((xSize * ySize * 2) + (xSize*2)) * 6, y = 0; y < (ySize) * (xSize+1); y+=xSize+1, ti += 6) {
      triangles[ti] = triangles[ti + 5] = y;
      triangles[ti + 1] = triangles[ti + 4] = y + ((xSize + 1) * (ySize + 1)) + (xSize + 1);
      triangles[ti + 2] = y + xSize + 1;
      triangles[ti + 3] = y + ((xSize + 1) * (ySize + 1));
    }

    //Right Side
    for (int ti = ((xSize * ySize * 2) + (xSize * 2) + ySize) * 6, vi = xSize + 1, y = xSize; y < ((ySize) * (xSize + 1)); y += xSize + 1, ti += 6, vi++) {
      triangles[ti+1] = triangles[ti + 4] = y;
      triangles[ti] = triangles[ti + 5] = y + ((xSize + 1) * (ySize + 1)) + (xSize + 1);
      triangles[ti + 2] = y + xSize + 1;
      triangles[ti + 3] = y + ((xSize + 1) * (ySize + 1));
    }
    mesh.triangles = triangles;
    mesh.RecalculateNormals();
  }
}
