using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;

public static class Verlet {
  //Particle Verlet Integration
  public static void Integrate(NativeArray<Vector3> curPoints, NativeArray<Vector3> prevPoints, Vector3 gravity, float deltaTime = 0.01f, float prevDeltaTime = 0.01f) {
    for (int i = 0; i < curPoints.Length; i++) {
      //Grab State from Previous Frame
      Vector3 tempPos = curPoints[i];

      //Integrate Position
      curPoints[i] += ((curPoints[i] - prevPoints[i]) /** (deltaTime / prevDeltaTime))*/ + (gravity * /*deltaTime * deltaTime*/(1f/3600f)));

      //Store State from Previous Frame
      prevPoints[i] = tempPos;
    }
  }

  //Mesh Volume: Sum the (Signed) Volumes of the tetrahedra encompassing the origin and the three triangle vertices
  //http://stackoverflow.com/a/13927691
  public static float VolumeOfMesh(NativeArray<Vector3> vertices, NativeArray<Vector3Int> triangles) {
    float sum = 0f;
    for (int i = 0; i < triangles.Length; i++) {
      sum += Vector3.Dot(vertices[triangles[i].x], Vector3.Cross(vertices[triangles[i].y], vertices[triangles[i].z])) * 0.166666f;
    }
    return Mathf.Abs(sum);
  }

  //Mesh Volume: Sum the areas of the triangles formed by the three triangle vertices
  public static float SurfaceAreaOfMesh(NativeArray<Vector3> vertices, NativeArray<Vector3Int> triangles) {
    float sum = 0f;
    for (int i = 0; i < triangles.Length; i++) {
      sum += Vector3.Cross(vertices[triangles[i].y] - vertices[triangles[i].x], vertices[triangles[i].z] - vertices[triangles[i].x]).magnitude * 0.5f;
    }
    return Mathf.Abs(sum);
  }

  //Recalculate Normals without allocating garbage
  public static void RecalculateNormalsNonAlloc(NativeArray<Vector3> vertices, NativeArray<Vector3Int> triangles, ref NativeArray<Vector3> normals) {
    for (int i = 0; i < triangles.Length; i++) {
      Vector3 normal = Vector3.Cross(vertices[triangles[i].x] - vertices[triangles[i].y], vertices[triangles[i].x] - vertices[triangles[i].z]);
      normals[triangles[i].x] += normal;
      normals[triangles[i].y] += normal;
      normals[triangles[i].z] += normal;
    }

    for(int i = 0; i < normals.Length; i++) {
      normals[i] = normals[i].normalized;
    }
  }

  //Recalculate Normals while allocating garbage, but more quickly
  public static void RecalculateNormalsAlloc(NativeArray<Vector3> vertices, NativeArray<Vector3Int> triangles, ref NativeArray<Vector3> normals) {
    Mesh tempMesh = new Mesh();
    tempMesh.vertices = vertices.ToArray();
    //tempMesh.triangles = triangles.ToArray(); //BROKEN TODO FIX LATER
    tempMesh.RecalculateNormals();
    Vector3[] tempNorm = tempMesh.normals;
    for (int i = 0; i < normals.Length; i++) { normals[i] = tempNorm[i]; }
  }

  public static void setVolume(float desiredVolume, NativeArray<Vector3> verts, NativeArray<Vector3> normals, NativeArray<Vector3Int> triangles, float surfaceArea = 0f, bool equality = true, bool fastButGarbage = true, bool explosionResistance = true) {
    //Calculate the normals of each vertex...
    if (fastButGarbage) {
      RecalculateNormalsAlloc(verts, triangles, ref normals);
    } else {
      RecalculateNormalsNonAlloc(verts, triangles, ref normals);
    }

    //And the distance we have to dilate each vert to acheive the desired volume...
    float volumeOfMesh = VolumeOfMesh(verts, triangles);
    float deltaVolume = desiredVolume - volumeOfMesh;
    deltaVolume = (explosionResistance && volumeOfMesh > desiredVolume * 2f) ? 0f : deltaVolume; //Explosion resistance
    if (deltaVolume > 0 || equality) {
      float dilationDistance = deltaVolume / (surfaceArea == 0f ? SurfaceAreaOfMesh(verts, triangles) : surfaceArea);

      //And we translate the verts to achieve that volume
      for (int j = 0; j < verts.Length; j++) {
        verts[j] += normals[j] * dilationDistance;
      }
    }
  }

  //Distance Constraints and Constraint Utility Functions
  [StructLayout(LayoutKind.Sequential)]
  public struct DistConstraint : IEquatable<DistConstraint> {
    public int index1;
    public int index2;
    public float Distance;
    private float sqrDistance;
    public int equality;

    public DistConstraint(int i1, int i2, NativeArray<Vector3> verts, bool EqualityConstraint = false) {
      index1 = i1;
      index2 = i2;
      Distance = Vector3.Distance(verts[index1], verts[index2]);
      sqrDistance = Distance * Distance;
      equality = EqualityConstraint ? 1 : 0;
    }

    public void ResolveConstraint(NativeArray<Vector3> vertices, ref NativeArray<Vector4> accumulatedDisplacements, bool resolveIndex2 = true) {
      if ((index1 != index2) && (equality == 1 || (vertices[index1] - vertices[index2]).sqrMagnitude > sqrDistance)) {
        Vector3 offset = (vertices[index2] - vertices[index1]);
        offset *= sqrDistance / (Vector3.Dot(offset, offset) + sqrDistance) - 0.5f;
        accumulatedDisplacements[index1] += new Vector4(-offset.x, -offset.y, -offset.z, 1f);
        if (resolveIndex2) { accumulatedDisplacements[index2] += new Vector4(offset.x, offset.y, offset.z, 1f); }
      }
    }

    public static bool operator ==(DistConstraint lhs, DistConstraint rhs) { return (Math.Min(lhs.index1, lhs.index2) == Math.Min(rhs.index1, rhs.index2) && Math.Max(lhs.index1, lhs.index2) == Math.Max(rhs.index1, rhs.index2)); }
    public static bool operator !=(DistConstraint lhs, DistConstraint rhs) { return !(lhs == rhs); }
    public override int GetHashCode() { return (Math.Min(index1, index2) * 32768) + Math.Max(index1, index2); }
    //public bool Equals(DistConstraint other) { return index1.Equals(other.index1) && index2.Equals(other.index2); }
    public bool Equals(DistConstraint other) { return (Math.Min(index1, index2) == Math.Min(other.index1, other.index2) && Math.Max(index1, index2) == Math.Max(other.index1, other.index2)); }
    public override bool Equals(object other) {if (!(other is DistConstraint)) return false; return Equals((DistConstraint)other); }
  }

  public static void resolveDistanceConstraints(NativeArray<DistConstraint> constraints, ref NativeArray<Vector3> verts, ref NativeArray<Vector4> accumulatedDisplacements, int iterations = 3) {
    for (int i = 0; i < iterations; i++) {
      for (int j = 0; j < constraints.Length; j++) {
        constraints[j].ResolveConstraint(verts, ref accumulatedDisplacements);
      }

      for (int j = 0; j < verts.Length; j++) {
        if (accumulatedDisplacements[j] != Vector4.zero) { accumulatedDisplacements[j] /= accumulatedDisplacements[j][3]; }
        verts[j] += new Vector3(accumulatedDisplacements[j][0], accumulatedDisplacements[j][1], accumulatedDisplacements[j][2]);
        accumulatedDisplacements[j] = Vector4.zero;
      }
    }
  }

  public static void setUpConstraints(Mesh constrainedMesh, List<DistConstraint> distanceConstraints, bool Equality = false, float scale = 1f) {
    NativeArray<Vector3> constrainedVerts = new NativeArray<Vector3>(constrainedMesh.vertices, Allocator.Temp);
    for (int i = 0; i < constrainedVerts.Length; i++) constrainedVerts[i] *= scale;
    int[] constrainedTriangles = constrainedMesh.triangles;
    List<int> edges = new List<int>(constrainedMesh.vertices.Length * 3);

    for (int i = 0; i < constrainedTriangles.Length; i += 3) {
      trySetUpConstraint(constrainedTriangles[i], constrainedTriangles[i + 1], Equality, ref constrainedVerts, ref distanceConstraints, ref edges);
      trySetUpConstraint(constrainedTriangles[i + 1], constrainedTriangles[i + 2], Equality, ref constrainedVerts, ref distanceConstraints, ref edges);
      trySetUpConstraint(constrainedTriangles[i], constrainedTriangles[i + 2], Equality, ref constrainedVerts, ref distanceConstraints, ref edges);
    }
    constrainedVerts.Dispose();
  }

  public static int trySetUpConstraint(int index1, int index2, bool Equality, ref NativeArray<Vector3> constrainedVerts, ref List<DistConstraint> distanceConstraints, ref List<int> edges) {
    DistConstraint constraint = new DistConstraint(index1, index2, constrainedVerts, Equality);
    int edgeHash = constraint.GetHashCode();
    if (!edges.Contains(edgeHash)) {
      distanceConstraints.Add(constraint);
      edges.Add(edgeHash);
    }
    return distanceConstraints.Count;
  }
}
