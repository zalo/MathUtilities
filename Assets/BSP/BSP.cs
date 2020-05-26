//#define G3_USING_UNITY
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
#if G3_USING_UNITY
using g3;
#endif

public class BSP {
  public SplittingDisk[] splittingDisks;
  public Vector2Int   [] leafTriangleIndices;
  public Vector3Int   [] sortedTriangles;
  public List<Vector3>   vertices;
  public struct SplittingDisk {
    public Vector4 plane;
    public Vector3 average;
    public float sqRadius;
  }

  public BSP(Mesh mesh, int levels) {
    // Stage 1: Construct the BSP

    // There will be 2^(levels)-1 splitting planes
    splittingDisks = new SplittingDisk[(int)Math.Pow(2, levels) - 1];
    vertices = new List<Vector3>();  mesh.GetVertices(vertices);
    //for (int i = 0; i < vertices.Count; i++) vertices[i] = model.transform.TransformPoint(vertices[i]);
    ConstructPartition(splittingDisks, vertices);


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
      int vertex1Leaf = QueryBSPLeaf(splittingDisks, vertices[triangle.x]);
      int vertex2Leaf = QueryBSPLeaf(splittingDisks, vertices[triangle.y]);
      int vertex3Leaf = QueryBSPLeaf(splittingDisks, vertices[triangle.z]);

      sortedTrianglesList[vertex1Leaf].Add(triangle);
      if (vertex2Leaf != vertex1Leaf) sortedTrianglesList[vertex2Leaf].Add(triangle);
      if (vertex3Leaf != vertex2Leaf && vertex3Leaf != vertex1Leaf) sortedTrianglesList[vertex3Leaf].Add(triangle);
    }

    // Stage 3: Flatten the elements for easy access

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
  public Vector3 QueryClosestPointRecursive(Vector3 point, ref Vector3 minPos, ref float minSqDist, int index = 0) {
    if (index < splittingDisks.Length) {
      Profiler.BeginSample("Explore Node");

      // We're in a branch node, so try both paths if it makes sense
      float planeDist = Vector3.Dot(splittingDisks[index].plane, point) - splittingDisks[index].plane.w;
      bool upSide = Mathf.Sign(planeDist) > 0f;

      // Calculate approximate squared distance to disk 
      // (closest point would be more accurate if this were more accurate...)
      Vector3 pointToPlane = point + (Vector3)(splittingDisks[index].plane * planeDist);
      float curDistance = Vector3.SqrMagnitude(splittingDisks[index].average - pointToPlane);
      Vector3 pointToDisk = pointToPlane;
      if (curDistance >= splittingDisks[index].sqRadius) pointToDisk = Constraints.FastConstrainDistance(pointToDisk, splittingDisks[index].average, splittingDisks[index].sqRadius);
      float distanceToSplittingDiskSqrd = Vector3.SqrMagnitude(point - pointToDisk);

      int moreLikelyBranch = (index * 2) + (upSide ? 1 : 2);
      int lessLikelyBranch = (index * 2) + (upSide ? 2 : 1);

      Profiler.EndSample();

#if UNITY_EDITOR
      // Draw the splitting planes we've visited
      if ((Vector3)splittingDisks[index].plane != Vector3.zero) {
        BSPTest.visitedDisks.Add(Matrix4x4.TRS(
              splittingDisks[index].average,
              Quaternion.LookRotation(splittingDisks[index].plane),
              new Vector3(1f, 1f, 0.0001f) * Mathf.Sqrt(splittingDisks[index].sqRadius)));
      }
#endif
                                                   QueryClosestPointRecursive(point, ref minPos, ref minSqDist, moreLikelyBranch);
      if (minSqDist > distanceToSplittingDiskSqrd) QueryClosestPointRecursive(point, ref minPos, ref minSqDist, lessLikelyBranch);
    } else {
      Profiler.BeginSample("Calculate Leaf Distance");
      // We're in a leaf, so iterate like chumps
      Vector2Int triangles = leafTriangleIndices[index - splittingDisks.Length];
      for (int i = triangles.x; i < triangles.x + triangles.y; i++) {
#if G3_USING_UNITY
        // This is a custom fork of the original function, made to take in
        // floating-point points and vertices
        float dist1 = MeshQueries.TriDistanceSqr(
            point,
            vertices[sortedTriangles[i].x],
            vertices[sortedTriangles[i].y],
            vertices[sortedTriangles[i].z]);
#else
        Profiler.BeginSample("Calculate Squared Triangle Distance");
        Vector3 trianglePoint = Constraints.ConstrainToTriangle(
            point,
            vertices[sortedTriangles[i].x],
            vertices[sortedTriangles[i].y],
            vertices[sortedTriangles[i].z]);
        float dist1 = Vector3.SqrMagnitude(point - trianglePoint);
        Profiler.EndSample();
#endif

        if (dist1 < minSqDist) {
          minSqDist = dist1;
          Profiler.BeginSample("Calculate Final Triangle Distance");
          minPos = Constraints.ConstrainToTriangle(
            point,
            vertices[sortedTriangles[i].x],
            vertices[sortedTriangles[i].y],
            vertices[sortedTriangles[i].z]);
          Profiler.EndSample();
        }
      }
      Profiler.EndSample();
    }
    return minPos;
  }

  /// <summary> Look Up which leaf index the point falls under </summary> 
  static int QueryBSPLeaf(SplittingDisk[] splittingPlanes, Vector3 point) {
    int index = 0;
    while (index < splittingPlanes.Length) {
      float distance = Vector3.Dot(splittingPlanes[index].plane, point);
      index = (2 * index) + ((distance > splittingPlanes[index].plane.w) ? 1 : 2);
    }
    return index - splittingPlanes.Length;
  }

  /// <summary> Recursively Split the points by the optimal fitting plane</summary> 
  static void ConstructPartition(SplittingDisk[] splittingPlanes, List<Vector3> points, int index = 0) {
    List<Vector3> upPoints   = new List<Vector3>(),
                  downPoints = new List<Vector3>();
    splittingPlanes[index] = GetSplittingPlane(points, upPoints, downPoints);

    // Recurse only until the tree is filled up
    if ((2 * index) + 1 < splittingPlanes.Length) {
      ConstructPartition(splittingPlanes, upPoints, (2 * index) + 1);
      ConstructPartition(splittingPlanes, downPoints, (2 * index) + 2);
    }
  }

  /// <summary> Split the points by the optimal dividing plane </summary> 
  static SplittingDisk GetSplittingPlane(List<Vector3> points,
      List<Vector3> upPoints, List<Vector3> downPoints) {
    Vector3 pointAverage, planeNormal = Vector3.up;

    // Fit a line through the average of the points
    Fit.Line(points, out pointAverage, ref planeNormal);//, drawGizmos: true);

    planeNormal = planeNormal.normalized;
    float planeDepth = Vector3.Dot(planeNormal, pointAverage);

    float maxSqDist = 0f;
    foreach (Vector3 point in points) {
      // Calculate squared distance of farthest point's 
      // projection onto the plane to the average
      Vector3 planePoint = new Plane(planeNormal, pointAverage).ClosestPointOnPlane(point);
      float dist         = Vector3.SqrMagnitude(planePoint - point);
      maxSqDist          = Mathf.Max(maxSqDist, dist);

      // Split Points into up and down lists depending 
      // on which side of the plane they're on
      if (Vector3.Dot(planeNormal, point) > planeDepth) {
        upPoints  .Add(point);
      } else {
        downPoints.Add(point);
      }
    }

    // Return Splitting Disk
    return new SplittingDisk() { plane = new Vector4(planeNormal.x, planeNormal.y, planeNormal.z, planeDepth), average = pointAverage, sqRadius = maxSqDist };
  }
}
