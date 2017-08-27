using System.Collections.Generic;
using UnityEngine;

public static class Verlet {
  //Particle Verlet Integration
  public static void Integrate(Vector3[] curPoints, Vector4[] prevPoints, Vector3 gravity, float deltaTime, float prevDeltaTime) {
    for (int i = 0; i < curPoints.Length; i++) {
      //Grab State from Previous Frame
      Vector3 tempPos = curPoints[i];

      //Integrate Position
      curPoints[i] += (curPoints[i] - (Vector3)prevPoints[i]) * (deltaTime / prevDeltaTime) + (gravity * deltaTime * deltaTime);

      //Store State from Previous Frame
      prevPoints[i] = tempPos;
    }
  }

  //Mesh Volume: Sum the (Signed) Volumes of the tetrahedra encompassing the origin and the three triangle vertices
  //http://stackoverflow.com/a/13927691
  public static float VolumeOfMesh(Vector3[] vertices, int[] triangles) {
    float sum = 0f;
    for (int i = 0; i < triangles.Length; i += 3) {
      sum += Vector3.Dot(vertices[triangles[i]], Vector3.Cross(vertices[triangles[i + 1]], vertices[triangles[i + 2]])) * 0.166666f;
    }
    return Mathf.Abs(sum);
  }

  //Mesh Volume: Sum the areas of the triangles formed by the three triangle vertices
  public static float SurfaceAreaOfMesh(Vector3[] vertices, int[] triangles) {
    float sum = 0f;
    for (int i = 0; i < triangles.Length; i += 3) {
      sum += Vector3.Cross(vertices[triangles[i + 1]] - vertices[triangles[i]], vertices[triangles[i + 2]] - vertices[triangles[i]]).magnitude * 0.5f;
    }
    return Mathf.Abs(sum);
  }

  //Recalculate Normals without allocating garbage
  public static void RecalculateNormalsNonAlloc(Vector3[] vertices, int[] triangles, ref Vector3[] normals) {
    for (int i = 0; i < triangles.Length; i += 3) {
      Vector3 normal = Vector3.Cross(vertices[triangles[i]] - vertices[triangles[i + 1]], vertices[triangles[i]] - vertices[triangles[i + 2]]);
      normals[triangles[i]] += normal;
      normals[triangles[i + 1]] += normal;
      normals[triangles[i + 2]] += normal;
    }

    for(int i = 0; i < normals.Length; i++) {
      normals[i] = normals[i].normalized;
    }
  }

  //Recalculate Normals while allocating garbage, but more quickly
  public static void RecalculateNormalsAlloc(Vector3[] vertices, int[] triangles, ref Vector3[] normals) {
    Mesh tempMesh = new Mesh();
    tempMesh.vertices = vertices;
    tempMesh.triangles = triangles;
    tempMesh.RecalculateNormals();
    Vector3[] tempNorm = tempMesh.normals;
    for (int i = 0; i < normals.Length; i++) { normals[i] = tempNorm[i]; }
  }

  public static void setVolume(float desiredVolume, Vector3[] verts, Vector3[] normals, int[] triangles, float surfaceArea = 0f, bool fastButGarbage = true) {
    //Calculate the normals of each vertex...
    if (fastButGarbage) {
      RecalculateNormalsAlloc(verts, triangles, ref normals);
    } else {
      RecalculateNormalsNonAlloc(verts, triangles, ref normals);
    }

    //And the distance we have to dilate each vert to acheive the desired volume...
    float deltaVolume = desiredVolume - VolumeOfMesh(verts, triangles);
    float dilationDistance = deltaVolume / (surfaceArea==0f?SurfaceAreaOfMesh(verts, triangles):surfaceArea);

    //And we translate the verts to acheive that volume
    for (int j = 0; j < verts.Length; j++) {
      verts[j] += normals[j] * dilationDistance;
    }
  }

  //Distance Constraints and Constraint Utility Functions
  public class DistConstraint {
    public int index1;
    public int index2;
    public float Distance = 0.5f;
    public bool equality = false;
    private float sqrDistance = 0.25f;

    public DistConstraint(int i1, int i2, Vector3[] verts, bool EqualityConstraint = false) {
      index1 = i1;
      index2 = i2;
      Distance = Vector3.Distance(verts[index1], verts[index2]);
      sqrDistance = Distance * Distance;
      equality = EqualityConstraint;
    }

    public void ResolveConstraint(Vector3[] vertices, ref Vector4[] accumulatedDisplacements) {
      if (equality || (vertices[index1] - vertices[index2]).sqrMagnitude > sqrDistance) {
        Vector3 offset = (vertices[index2] - vertices[index1]);
        offset *= sqrDistance / (Vector3.Dot(offset, offset) + sqrDistance) - 0.5f;
        accumulatedDisplacements[index1] += new Vector4(-offset.x, -offset.y, -offset.z, 1f);
        accumulatedDisplacements[index2] += new Vector4(offset.x, offset.y, offset.z, 1f);
      }
    }
  }

  public static void resolveDistanceConstraints(List<DistConstraint> constraints, ref Vector3[] verts, int iterations = 3) {
    Vector4[] accumulatedDisplacements = new Vector4[verts.Length];
    for (int j = 0; j < verts.Length; j++) {
      accumulatedDisplacements[j] = Vector4.zero;
    }

    for (int i = 0; i < iterations; i++) {
      for (int j = 0; j < constraints.Count; j++) {
        constraints[j].ResolveConstraint(verts, ref accumulatedDisplacements);
      }

      for (int j = 0; j < verts.Length; j++) {
        if (accumulatedDisplacements[j] != Vector4.zero) { accumulatedDisplacements[j] /= accumulatedDisplacements[j][3]; }
        verts[j] += new Vector3(accumulatedDisplacements[j][0], accumulatedDisplacements[j][1], accumulatedDisplacements[j][2]);
      }
    }
  }

  public static void setUpConstraints(Mesh constrainedMesh, List<DistConstraint> distanceConstraints, bool Equality = false) {
    Vector3[] constrainedVerts = constrainedMesh.vertices;
    int[] constrainedTriangles = constrainedMesh.triangles;
    List<int> edges = new List<int>(constrainedMesh.vertices.Length * 3);

    for (int i = 0; i < constrainedTriangles.Length; i += 3) {
      trySetUpConstraint(constrainedTriangles[i], constrainedTriangles[i + 1], Equality, ref constrainedVerts, ref distanceConstraints, ref edges);
      trySetUpConstraint(constrainedTriangles[i + 1], constrainedTriangles[i + 2], Equality, ref constrainedVerts, ref distanceConstraints, ref edges);
      trySetUpConstraint(constrainedTriangles[i], constrainedTriangles[i + 2], Equality, ref constrainedVerts, ref distanceConstraints, ref edges);
    }
  }

  public static void trySetUpConstraint(int index1, int index2, bool Equality, ref Vector3[] constrainedVerts, ref List<DistConstraint> distanceConstraints, ref List<int> edges) {
    int edgeHash = computeEdgeHash(index1, index2);
    if (!edges.Contains(edgeHash)) {
      distanceConstraints.Add(new DistConstraint(index1, index2, constrainedVerts, Equality));
      edges.Add(edgeHash);
    }
  }

  private static int computeEdgeHash(int i, int j) {
    return (i + j) * (i * j) + (i + j);
  }
}
