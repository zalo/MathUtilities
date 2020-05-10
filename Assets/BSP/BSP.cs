using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class BSP : MonoBehaviour {
  public bool drawNaiveClosestPoint = false;

  public MeshFilter model;
  public Transform probe;

  Mesh mesh;
  Vector3[] meshVertices;

  // GPU Friendly Representation of the BSP Tree
  Vector4[] splittingPlanes;
  Vector2Int[] leafTriangleIndices;
  Vector3Int[] sortedTriangles;

  void Start() {
    if (model != null) {
      mesh = model.sharedMesh;
      meshVertices = mesh.vertices;
      for (int i = 0; i < meshVertices.Length; i++) meshVertices[i] = model.transform.TransformPoint(meshVertices[i]);
      ConstructBSP(mesh, 11);
    }
  }

  void ConstructBSP(Mesh mesh, int levels) {
    // Stage 1: Construct the BSP

    // There will be 2^(levels)-1 splitting planes
    splittingPlanes = new Vector4[(int)Math.Pow(2, levels) - 1];
    List<Vector3> vertices = new List<Vector3>();
    mesh.GetVertices(vertices);
    for (int i = 0; i < vertices.Count; i++) vertices[i] = model.transform.TransformPoint(vertices[i]);
    ConstructPartition(splittingPlanes, vertices);


    // Stage 2: Place the elements into the BSP

    // There will be 2^levels leaf nodes
    leafTriangleIndices = new Vector2Int[(int)Math.Pow(2, levels)];

    // Put the triangles into a more convenient struct
    int[] triangles = mesh.triangles;
    List<Vector3Int> groupedTriangles = new List<Vector3Int>(triangles.Length / 3);
    for (int i = 0; i < triangles.Length; i += 3) {
      groupedTriangles.Add(new Vector3Int(triangles[i + 0],
                                          triangles[i + 1],
                                          triangles[i + 2]));
    }

    // Sort the triangles into the leaf nodes their vertices touch
    List<Vector3Int>[] sortedTrianglesList = new List<Vector3Int>[leafTriangleIndices.Length];
    for (int i = 0; i < sortedTrianglesList.Length; i++) sortedTrianglesList[i] = new List<Vector3Int>();
    foreach (Vector3Int triangle in groupedTriangles) {
      int vertex1Leaf = QueryBSPLeaf(splittingPlanes, vertices[triangle.x]);
      int vertex2Leaf = QueryBSPLeaf(splittingPlanes, vertices[triangle.y]);
      int vertex3Leaf = QueryBSPLeaf(splittingPlanes, vertices[triangle.z]);

      sortedTrianglesList[vertex1Leaf].Add(triangle);
      if (vertex2Leaf != vertex1Leaf) sortedTrianglesList[vertex2Leaf].Add(triangle);
      if (vertex3Leaf != vertex2Leaf && vertex3Leaf != vertex1Leaf) sortedTrianglesList[vertex3Leaf].Add(triangle);
    }

    // Concatenate these into a single fixed-size array
    List<Vector3Int> flatTrianglesList = new List<Vector3Int>();
    for (int i = 0; i < sortedTrianglesList.Length; i++) {
      leafTriangleIndices[i] = new Vector2Int(flatTrianglesList.Count, sortedTrianglesList[i].Count);
      flatTrianglesList.AddRange(sortedTrianglesList[i]);
    }
    sortedTriangles = flatTrianglesList.ToArray();

    // The outputs: splittingPlanes, leafTriangleIndices, sortedTriangles
  }

  /// <summary> Look Up which leaf index the point falls under </summary> 
  Vector3 QueryClosestTriangleRecursive(Vector3 point, ref Vector3 minPos, ref float minDist, int index = 0) {
    if(index < splittingPlanes.Length) {
      // We're in a branch node, so try both paths if it makes sense
      float distance = Vector3.Dot(splittingPlanes[index], point);
      float distanceToSplittingPlane = Mathf.Abs(distance - splittingPlanes[index].w);

      int moreLikelyBranch = (index * 2) + ((distance > splittingPlanes[index].w) ? 1 : 2);
      int lessLikelyBranch = (index * 2) + ((distance > splittingPlanes[index].w) ? 2 : 1);

                                              QueryClosestTriangleRecursive(point, ref minPos, ref minDist, moreLikelyBranch);
      if (minDist > distanceToSplittingPlane) QueryClosestTriangleRecursive(point, ref minPos, ref minDist, lessLikelyBranch);
    } else {
      // We're in a leaf, so iterate like chumps
      Vector2Int triangles = leafTriangleIndices[index - splittingPlanes.Length];
      for (int i = triangles.x; i < triangles.x + triangles.y; i++) {
        Vector3 trianglePoint = Constraints.ConstrainToTriangle(point,
            meshVertices[sortedTriangles[i].x],
            meshVertices[sortedTriangles[i].y],
            meshVertices[sortedTriangles[i].z]);
        float dist1 = Vector3.Distance(point, trianglePoint);
        if (dist1 < minDist) { minDist = dist1; minPos = trianglePoint; }
      }
    }
    return minPos;
  }

  /// <summary> Look Up which leaf index the point falls under </summary> 
  static int QueryBSPLeaf(Vector4[] splittingPlanes, Vector3 point) {
    int index = 0;
    while (index < splittingPlanes.Length) {
      float distance = Vector3.Dot(splittingPlanes[index], point);
      index = (2 * index) + ((distance > splittingPlanes[index].w) ? 1 : 2);
    }
    return index - splittingPlanes.Length;
  }

  /// <summary> Recursively Split the points by the optimal fitting plane</summary> 
  static void ConstructPartition(Vector4[] splittingPlanes, List<Vector3> points, int index = 0) {
    List<Vector3> upPoints = new List<Vector3>(),
                  downPoints = new List<Vector3>();
    splittingPlanes[index] = GetSplittingPlane(points, upPoints, downPoints);

    // Recurse only until the tree is filled up
    if ((2 * index) + 1 < splittingPlanes.Length) {
      ConstructPartition(splittingPlanes,   upPoints, (2 * index) + 1);
      ConstructPartition(splittingPlanes, downPoints, (2 * index) + 2);
    }
  }

  /// <summary> Split the points by the optimal dividing plane </summary> 
  static Vector4 GetSplittingPlane(List<Vector3> points,
      List<Vector3> upPoints = null, List<Vector3> downPoints = null) {
    Vector3 pointAverage, planeNormal = Vector3.up;

    // Fit a line through the average of the points
    Fit.Line(points, out pointAverage, ref planeNormal);//, drawGizmos: true);

    planeNormal = planeNormal.normalized;
    float planeDepth = Vector3.Dot(planeNormal, pointAverage);

    // Split Points into up and down lists depending 
    // on which side of the plane they're on
    if (upPoints != null && downPoints != null) {
      foreach (Vector3 point in points) {
        if (Vector3.Dot(planeNormal, point) > planeDepth) {
          upPoints  .Add(point);
        } else {
          downPoints.Add(point);
        }
      }
    }

    // Return Splitting Plane
    return new Vector4(planeNormal.x, planeNormal.y, planeNormal.z, planeDepth);
  }

  private void OnDrawGizmos() {
    if (splittingPlanes == null) Start();

    // Draw a line between the probe and the closest triangle vertex via the BSP...
    if (leafTriangleIndices != null && leafTriangleIndices.Length > 0) {
      Vector3 queryPoint = probe.position;
      float minDist = 1000f; Vector3 minPos = Vector3.one; Vector3Int tri = Vector3Int.zero;

      if (drawNaiveClosestPoint) {
        Profiler.BeginSample("Naive Closest Point on Mesh");
        minDist = 1000f;
        for (int i = 0; i < sortedTriangles.Length; i++) {
          Vector3 trianglePoint = Constraints.ConstrainToTriangle(queryPoint,
              meshVertices[sortedTriangles[i].x],
              meshVertices[sortedTriangles[i].y],
              meshVertices[sortedTriangles[i].z]);
          float dist1 = Vector3.Distance(queryPoint, trianglePoint);
          if (dist1 < minDist) { minDist = dist1; minPos = trianglePoint; tri = sortedTriangles[i]; }
        }
        Gizmos.color = Color.red;
        Gizmos.DrawLine(queryPoint, minPos);
        Gizmos.DrawSphere(minPos, 0.005f);
        Gizmos.DrawSphere(meshVertices[tri.x], 0.0025f);
        Gizmos.DrawSphere(meshVertices[tri.y], 0.0025f);
        Gizmos.DrawSphere(meshVertices[tri.z], 0.0025f);

        Profiler.EndSample();
      }

      Profiler.BeginSample("BSP Closest Point on Mesh");
      Gizmos.color = Color.white;
      Vector3 minPoss = Vector3.zero; float minDists = 100000f;
      QueryClosestTriangleRecursive(queryPoint, ref minPoss, ref minDists);
      Gizmos.DrawLine(queryPoint, minPoss);
      Gizmos.DrawSphere(minPoss, 0.005f);
      Profiler.EndSample();
    }
  }
}
