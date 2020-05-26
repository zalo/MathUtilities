using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class BSPTest : MonoBehaviour {
  public bool drawNaiveClosestPoint = false;

  public MeshFilter model;
  public Transform probe;

  Mesh mesh;
  Vector3[] meshVertices;
  int    [] meshTriangles;

  BSP binarySpacePartition;

  public Mesh diskMesh;
  public Material diskMaterial;
  public Material visitedDiskMaterial;
  Matrix4x4[] diskMatrices;
  public static List<Matrix4x4> visitedDisks;

#if G3_USING_UNITY
  DMeshAABBTree3 g3MeshTree;
#endif

  void Start() {
    if (model != null) {
      mesh          = model.sharedMesh;
      meshVertices  = mesh.vertices;
      meshTriangles = mesh.triangles;
      for (int i = 0; i < meshVertices.Length; i++) meshVertices[i] = model.transform.TransformPoint(meshVertices[i]);
      binarySpacePartition = new BSP(mesh, 14);

      if (diskMaterial) {
        int numDisksToDraw = Mathf.Min(binarySpacePartition.splittingDisks.Length, 15);
        diskMatrices       = new Matrix4x4[numDisksToDraw];
        for (int i = 0; i < numDisksToDraw; i++) {
          diskMatrices[i]  = Matrix4x4.TRS(
            binarySpacePartition.splittingDisks[i].average, 
            Quaternion.LookRotation(binarySpacePartition.splittingDisks[i].plane), 
            new Vector3(1f, 1f, 0.0001f) * Mathf.Sqrt(binarySpacePartition.splittingDisks[i].sqRadius));
        }
      }

#if G3_USING_UNITY
      DMesh3Builder dMeshBuilder = new DMesh3Builder();
      dMeshBuilder.AppendNewMesh(false, false, false, false);
      foreach (Vector3 vertex in meshVertices)       dMeshBuilder.AppendVertex(vertex.x, vertex.y, vertex.z);
      for(int i = 0; i < meshTriangles.Length; i+=3) dMeshBuilder.AppendTriangle(meshTriangles[i], meshTriangles[i+1], meshTriangles[i+2]);
      g3MeshTree = new DMeshAABBTree3(dMeshBuilder.Meshes[0]);
      g3MeshTree.Build();
#endif
    }
  }

  public void Update() {
    if (diskMaterial != null && visitedDisks != null) {
      Matrix4x4[] diskArray = visitedDisks.ToArray();
      Graphics.DrawMeshInstanced(diskMesh, 0, diskMaterial, diskArray, diskArray.Length, null, UnityEngine.Rendering.ShadowCastingMode.Off, false);
    }
  }

  private void OnDrawGizmos() {
    if (binarySpacePartition == null) Start();

    // Draw a line between the probe and the closest triangle vertex via the BSP...
    Vector3 queryPoint = probe.position;

    if (drawNaiveClosestPoint) {
      Profiler.BeginSample("Naive Closest Point on Mesh");
      float naiveMinDist = 1000f; Vector3 naiveMinPos = Vector3.one;
      for (int i = 0; i < meshTriangles.Length; i+=3) {
        Vector3 trianglePoint = Constraints.ConstrainToTriangle(queryPoint,
            meshVertices[meshTriangles[i+0]],
            meshVertices[meshTriangles[i+1]],
            meshVertices[meshTriangles[i+2]]);
        float dist1 = Vector3.Distance(queryPoint, trianglePoint);
        if (dist1 < naiveMinDist) { naiveMinDist = dist1; naiveMinPos = trianglePoint; }
      }
      Gizmos.color = Color.red;
      Gizmos.DrawLine(queryPoint, naiveMinPos);
      Gizmos.DrawSphere(naiveMinPos, 0.005f);

      Profiler.EndSample();
    }

    if (visitedDisks == null) visitedDisks = new List<Matrix4x4>();
    visitedDisks.Clear();
    Profiler.BeginSample("BSP Closest Point on Mesh", this);
    Gizmos.color = Color.white;
    Vector3 bspMinPos = Vector3.zero; float bspMinSqDist = 100000f;
    binarySpacePartition.QueryClosestPointRecursive(queryPoint, ref bspMinPos, ref bspMinSqDist);
    Gizmos.DrawLine(queryPoint, bspMinPos);
    Gizmos.DrawSphere(bspMinPos, 0.005f);
    Profiler.EndSample();

#if G3_USING_UNITY
    if (g3MeshTree != null && g3MeshTree.IsValid) {
      Profiler.BeginSample("D3 Closest Point on Mesh", this);
      Gizmos.color = Color.magenta;
      int nearestTri = g3MeshTree.FindNearestTriangle(new g3.Vector3d(queryPoint.x, queryPoint.y, queryPoint.z));
      Vector3 trianglePointt = Constraints.ConstrainToTriangle(queryPoint,
          meshVertices[meshTriangles[(nearestTri*3)  ]],
          meshVertices[meshTriangles[(nearestTri*3)+1]],
          meshVertices[meshTriangles[(nearestTri*3)+2]]);
      Gizmos.DrawLine(queryPoint, trianglePointt);
      Gizmos.DrawSphere(trianglePointt, 0.005f);
      Profiler.EndSample();
    }
#endif
  }
}
