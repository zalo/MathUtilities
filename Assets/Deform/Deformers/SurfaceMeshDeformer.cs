using System.Collections.Generic;
using UnityEngine;

public class SurfaceMeshDeformer : MeshDeformer {
  protected List<int>[] vertexConnections;
  List<float[]> vertexDistances;
  public SurfaceMeshDeformer(MeshFilter inputMeshFilter, Vector3[] controlPoints, float weight = 1f) : base(inputMeshFilter, controlPoints, weight) {}

  protected override void InitializeMesh(MeshFilter inputMeshFilter, Vector3[] controlPoints, float weight = 1f) {
    originalMeshVertices = inputMeshFilter.mesh.vertices;
    vertexConnections = new List<int>[originalMeshVertices.Length];
    calculateVertexConnections(inputMeshFilter.mesh.triangles, ref vertexConnections);
    base.InitializeMesh(inputMeshFilter, controlPoints, weight);
  }

  //Calculate the influence of each control point on each vertex
  protected override void calculateVertexWeights(Vector3[] originalPlaneVerts, Vector3[] originalControlPoints, 
                                                 float weightFalloff, ref float[,] vertexControlWeights, bool justWeightFalloff = false) {
    if (vertexControlWeights == null || vertexControlWeights.GetLength(0) != originalPlaneVerts.Length ||
                                        vertexControlWeights.GetLength(1) != originalControlPoints.Length) {
      vertexControlWeights = new float[originalPlaneVerts.Length, originalControlPoints.Length];
    }

    //Calculate the surface distance away from the control point
    if (!justWeightFalloff) {
      vertexDistances = new List<float[]>(originalControlPoints.Length);
      for (int i = 0; i < originalControlPoints.Length; i++) {
        float[] tempVertexDistances = new float[originalPlaneVerts.Length];
        calculateVertexDistances(ref originalMeshVertices, ref originalControlPoints[i], ref vertexConnections, ref tempVertexDistances);
        vertexDistances.Add(tempVertexDistances);
      }
    }

    //Weight the vertices by their distance from the control point
    for (int i = 0; i < originalPlaneVerts.Length; i++) {
      float totalWeight = 0f;
      float[] tempControlPointWeights = new float[originalControlPoints.Length];
      for (int j = 0; j < originalControlPoints.Length; j++) {
        if(vertexDistances[j][i] == 0f) { vertexDistances[j][i] = float.PositiveInfinity; }
        tempControlPointWeights[j] = 1f / Mathf.Pow(vertexDistances[j][i] + 0.001f, weightFalloff);
        totalWeight += tempControlPointWeights[j];
      }

      //Normalize the weights
      for (int j = 0; j < originalControlPoints.Length; j++) {
        tempControlPointWeights[j] /= totalWeight;
        vertexControlWeights[i, j] = tempControlPointWeights[j];
      }
    }
  }

  //Calculate the connections from each vertex to each other vertex
  protected static void calculateVertexDistances(ref Vector3[] originalMeshVertices, ref Vector3 controlPoint, ref List<int>[] vertexConnections, ref float[] vertexDistances) {
    if (vertexDistances == null || vertexDistances.Length != originalMeshVertices.Length) { vertexDistances = new float[originalMeshVertices.Length]; }
    //Find closest vertex
    float minimumDistance = 100000f; int closestVertex = -1;
    for (int i = 0; i < originalMeshVertices.Length; i++) {
      float currentDistance = Vector3.Distance(controlPoint, originalMeshVertices[i]);
      if (currentDistance < minimumDistance) {
        minimumDistance = currentDistance;
        closestVertex = i;
      }
    }
    vertexDistances[closestVertex] = minimumDistance + 0.0001f;

    //Do a breadth first traversal of the mesh graph to calculate surface 
    //distances from the controlPoint to the "closestVertex" to each vertex
    Queue<int> vertexQueue = new Queue<int>();
    vertexQueue.Enqueue(closestVertex);
    while(vertexQueue.Count > 0) {
      int vertex = vertexQueue.Dequeue();
      for (int i = 0; i < vertexConnections[vertex].Count; i++) {
        int curVertex = vertexConnections[vertex][i];
        float curDistAtVertex = vertexDistances[curVertex];
        float distToVertex = vertexDistances[vertex] + Vector3.Distance(originalMeshVertices[vertex], originalMeshVertices[curVertex]);
        if (curDistAtVertex == 0f || curDistAtVertex > distToVertex) {
          vertexDistances[curVertex] = distToVertex;
          vertexQueue.Enqueue(curVertex);
        }
      }
    }
  }

  //Calculate the connections from each vertex to each other vertex
  protected static void calculateVertexConnections(int[] triangles, ref List<int>[] vertexConnections) {
    for (int i = 0; i < vertexConnections.Length; i++) {
      vertexConnections[i] = new List<int>();
    }
    for(int i = 0; i < triangles.Length; i+=3) {
      if (!vertexConnections[triangles[i]].Contains(triangles[i + 1])) {
        vertexConnections[triangles[i]].Add(triangles[i + 1]);
      }
      if (!vertexConnections[triangles[i]].Contains(triangles[i + 2])) {
        vertexConnections[triangles[i]].Add(triangles[i + 2]);
      }
      if (!vertexConnections[triangles[i + 1]].Contains(triangles[i])) {
        vertexConnections[triangles[i + 1]].Add(triangles[i ]);
      }
      if (!vertexConnections[triangles[i + 1]].Contains(triangles[i + 2])) {
        vertexConnections[triangles[i + 1]].Add(triangles[i + 2]);
      }
      if (!vertexConnections[triangles[i + 2]].Contains(triangles[i + 1])) {
        vertexConnections[triangles[i + 2]].Add(triangles[i + 1]);
      }
      if (!vertexConnections[triangles[i + 2]].Contains(triangles[i])) {
        vertexConnections[triangles[i + 2]].Add(triangles[i]);
      }
    }
  }
}
