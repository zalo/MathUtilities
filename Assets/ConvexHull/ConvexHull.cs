using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

[ExecuteInEditMode]
public class ConvexHull : MonoBehaviour {

    public struct VertexData {
        public int             isFacing;
        public int             isNotFacing;
        public int             index;
        public float3          position;

        public bool isHorizon() { return isFacing > 0 && isNotFacing > 0; }
        public VertexData(VertexData data, int isCurrentlyFacing = 0, int newIndex = -1) {
            position = data.position;

            // Crazy way of saying this is a vert on triangles that are
            // both facing the incoming point AND facing away from it
            if (isCurrentlyFacing == 0) {
                if (newIndex == -1) { isFacing = 0; isNotFacing = 0;
                } else { isFacing = data.isFacing; isNotFacing = data.isNotFacing; }
            } else {
                isFacing    = data.isFacing    + (isCurrentlyFacing ==  1 ? 1 : 0);
                isNotFacing = data.isNotFacing + (isCurrentlyFacing == -1 ? 1 : 0);
            }
            index = newIndex < 0 ? data.index : newIndex;
        }
    }

    public struct TriangleData {
        public int isFacing;
        public int3 vertices;
        public float3 normal;
        public TriangleData(TriangleData data, int isFacing = 0) {
            vertices = data.vertices;
            normal = data.normal;
            this.isFacing = isFacing;
        }
        public float3 getCentroid(ref NativeList<VertexData> inVertices) {
            return (inVertices[vertices.x].position +
                    inVertices[vertices.y].position +
                    inVertices[vertices.z].position) * 0.333f;
        }
        public bool isFacingPoint(Vector3 point, ref NativeList<VertexData> vertices) {
            return Vector3.Dot((float3)point - getCentroid(ref vertices), normal) > 0f;
        }
    }

    public struct ConvexHullData : IDisposable {
        public NativeList<VertexData  > vertices;
        public NativeList<TriangleData> triangles;

        public ConvexHullData(int initSize = 4) {
            vertices  = new NativeList<VertexData  >(initSize, Allocator.Persistent);
            triangles = new NativeList<TriangleData>(initSize, Allocator.Persistent);
        }

        public void Dispose() {
            if (vertices .IsCreated) { vertices.Dispose();  }
            if (triangles.IsCreated) { triangles.Dispose(); }
        }
    }

    [Range(4, 30)]
    public int pointsInHull = 1;
    List<float3> points;
    ConvexHullData convexHull;
    Vector3 lastPos;
    public Transform interactivePoint;
    Mesh hullMesh;
    MeshFilter filter;

    void Update() {
        if (points == null) {
            points = new List<float3>();
            for (int i = 0; i < 30; i++) {
                points.Add(UnityEngine.Random.insideUnitSphere);
            }
        }

        // Step 1: Construct a Tetrahedron via some random heuristic
        constructInitialSimplex(points, ref convexHull);

        // Steps 2-7: Expand the Convex Hull
        for (int p = 4; p < pointsInHull; p++) {
            GrowHull(points[p]);
        }
        GrowHull(interactivePoint.position);

        // Construct vertex and triangle mesh representations
        if (hullMesh == null) { hullMesh = new Mesh(); }
        if (filter == null) { filter = GetComponent<MeshFilter>(); filter.sharedMesh = hullMesh; }
        Vector3[] vertices = new Vector3[convexHull.vertices.Length];
        int[] triangles = new int[convexHull.triangles.Length * 3];
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] = convexHull.vertices[i].position;
        }
        for (int i = 0; i < triangles.Length; i += 3) {
            triangles[i + 0] = convexHull.triangles[i / 3].vertices.x;
            triangles[i + 1] = convexHull.triangles[i / 3].vertices.y;
            triangles[i + 2] = convexHull.triangles[i / 3].vertices.z;
        }
        hullMesh.SetVertices(vertices); hullMesh.SetTriangles(triangles, 0);
        hullMesh.RecalculateNormals();
    }

    void OnDisable() { convexHull.Dispose(); }
    void OnDestroy() { convexHull.Dispose(); }

    bool GrowHull(float3 newPoint) {
        bool isOutsideOfHull = false;

        // Step 7: Clean up after ourselves (Resets the "isFacing" sentinels on the verts and tris).
        for (int i = 0; i < convexHull.vertices.Length; i++) { convexHull.vertices[i] = new VertexData(convexHull.vertices[i]); }
        for (int i = 0; i < convexHull.triangles.Length; i++) { convexHull.triangles[i] = new TriangleData(convexHull.triangles[i]); }

        // Step 2: Highlight all the vertices on the horizon, mark all the triangles
        for (int i = 0; i < convexHull.triangles.Length; i++) {
            // Check if this triangle is facing toward our next point
            bool triangleIsFacing = convexHull.triangles[i].isFacingPoint(newPoint, ref convexHull.vertices);

            // Mark each vertex with whether it's on a triangle facing the incoming point or away from it
            // A vertex must be on both kinds of triangles to be on the horizon!!
            convexHull.vertices[convexHull.triangles[i].vertices.x] = new VertexData(convexHull.vertices[convexHull.triangles[i].vertices.x], triangleIsFacing ? 1 : -1);
            convexHull.vertices[convexHull.triangles[i].vertices.y] = new VertexData(convexHull.vertices[convexHull.triangles[i].vertices.y], triangleIsFacing ? 1 : -1);
            convexHull.vertices[convexHull.triangles[i].vertices.z] = new VertexData(convexHull.vertices[convexHull.triangles[i].vertices.z], triangleIsFacing ? 1 : -1);

            if (triangleIsFacing) {
                isOutsideOfHull = true;

                // Mark the current triangle as facing the new point
                if (convexHull.triangles[i].isFacing != 1) {
                    convexHull.triangles[i] = new TriangleData(convexHull.triangles[i], 1);
                }
            }
        }

        if (isOutsideOfHull) {
            // Step 3: Delete the facing triangles
            for (int i = convexHull.triangles.Length - 1; i >= 0; i--) {
                if (convexHull.triangles[i].isFacing > 0) {
                    convexHull.triangles.RemoveAtSwapBack(i);
                }
            }

            // Step 4: Remind each vertex what its original index was (foreshadowing!)
            for (int i = 0; i < convexHull.vertices.Length; i++) { convexHull.vertices[i] = new VertexData(convexHull.vertices[i], newIndex: i); }
            convexHull.vertices.Add(new VertexData() { position = newPoint, index = convexHull.vertices.Length - 1 });

            // Step 5: Sort the horizon vertices clockwise about the point-average-average axis
            List<VertexData> horizonVerts = new List<VertexData>();
            for (int i = 0; i < convexHull.vertices.Length; i++) {
                if (convexHull.vertices[i].isHorizon()) {
                    horizonVerts.Add(convexHull.vertices[i]);
                }
            }
            float3 avg = float3.zero; foreach (VertexData p in horizonVerts) { avg += p.position; } 
            avg /= horizonVerts.Count; float3 incomingAxis = newPoint - avg;
            horizonVerts.Sort(delegate (VertexData t1, VertexData t2) {
                float3 planart1 = Vector3.ProjectOnPlane(t1.position - avg, incomingAxis);
                float3 planart2 = Vector3.ProjectOnPlane(t2.position - avg, incomingAxis);
                float3 planarUp = Vector3.ProjectOnPlane(Vector3.up, incomingAxis).normalized;
                return 
                    Vector3.SignedAngle(planart2, planarUp, incomingAxis).CompareTo(
                    Vector3.SignedAngle(planart1, planarUp, incomingAxis));
            });

            // Step 6: Create new triangles around this horizon and call it good...
            for (int i = 0; i < horizonVerts.Count; i++) {
                convexHull.triangles.Add(new TriangleData() {
                    vertices = new int3(horizonVerts[i].index,    horizonVerts[(i + 1) % horizonVerts.Count].index, convexHull.vertices.Length - 1),
                    normal   = Normal  (horizonVerts[i].position, horizonVerts[(i + 1) % horizonVerts.Count].position, newPoint)
                });
            }
        }

        return isOutsideOfHull;
    }

    ///<summary>Create a simplex from a list of points.</summary>
    void constructInitialSimplex(List<float3> points, ref ConvexHullData convexHull) {
        convexHull.Dispose(); convexHull = new ConvexHullData(4);

        // Pick some points far from the centroid to use as initial simplex
        float3 avg = float3.zero; foreach (float3 p in points) { avg += p; } avg /= points.Count;
        points.Sort((a, b) => ((Vector3)(b - avg)).sqrMagnitude.CompareTo(((Vector3)(a - avg)).sqrMagnitude));

        convexHull.vertices.Add(new VertexData() { position = points[0]});
        convexHull.vertices.Add(new VertexData() { position = points[1]});
        convexHull.vertices.Add(new VertexData() { position = points[2]});
        convexHull.vertices.Add(new VertexData() { position = points[3]});

        // Create simplex depending on the handedness
        // https://github.com/OskarSigvardsson/unity-quickhull/blob/master/Scripts/ConvexHullCalculator.cs#L373-L383
        if (Vector3.Dot(points[3] - points[1], Vector3.Cross(points[1] - points[0], points[2] - points[0])) > 0.0f) {
            convexHull.triangles.Add(new TriangleData() { vertices = new int3(0, 2, 1), normal = Normal(points[0], points[2], points[1]) });
            convexHull.triangles.Add(new TriangleData() { vertices = new int3(0, 1, 3), normal = Normal(points[0], points[1], points[3]) });
            convexHull.triangles.Add(new TriangleData() { vertices = new int3(0, 3, 2), normal = Normal(points[0], points[3], points[2]) });
            convexHull.triangles.Add(new TriangleData() { vertices = new int3(1, 2, 3), normal = Normal(points[1], points[2], points[3]) });
        } else {
            convexHull.triangles.Add(new TriangleData() { vertices = new int3(0, 1, 2), normal = Normal(points[0], points[1], points[2]) });
            convexHull.triangles.Add(new TriangleData() { vertices = new int3(0, 3, 1), normal = Normal(points[0], points[3], points[1]) });
            convexHull.triangles.Add(new TriangleData() { vertices = new int3(0, 2, 3), normal = Normal(points[0], points[2], points[3]) });
            convexHull.triangles.Add(new TriangleData() { vertices = new int3(1, 3, 2), normal = Normal(points[1], points[3], points[2]) });
        }
    }


    bool isInside(float3 point, ConvexHullData hull) {
        bool isInsideOfHull = true;
        for (int i = 0; i < convexHull.triangles.Length; i++) {
            // Check if this triangle is facing toward our next point
            if (convexHull.triangles[i].isFacingPoint(point, ref convexHull.vertices)) { isInsideOfHull = false; }
        }
        return isInsideOfHull;
    }

    Vector3 Normal(Vector3 v0, Vector3 v1, Vector3 v2) {
        return Vector3.Cross(v1 - v0, v2 - v0).normalized;
    }

    void OnDrawGizmosSelected() {
        if (points != null) {
            for (int i = 0; i < points.Count; i++) {
                Gizmos.color = (i < convexHull.vertices.Length) ? (convexHull.vertices[i].isHorizon() ? Color.cyan : Color.yellow) : Color.green;
                Gizmos.color = (i > 6 && isInside(points[i], convexHull)) ? Color.magenta : Gizmos.color;

                Gizmos.DrawSphere(points[i], 0.05f);
            }

            if (convexHull.triangles.IsCreated) {
                for (int i = 0; i < convexHull.triangles.Length; i++) {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(
                        convexHull.vertices[convexHull.triangles[i].vertices.x].position,
                        convexHull.vertices[convexHull.triangles[i].vertices.y].position);
                    Gizmos.DrawLine(
                        convexHull.vertices[convexHull.triangles[i].vertices.y].position,
                        convexHull.vertices[convexHull.triangles[i].vertices.z].position);
                    Gizmos.DrawLine(
                        convexHull.vertices[convexHull.triangles[i].vertices.z].position,
                        convexHull.vertices[convexHull.triangles[i].vertices.x].position);

                    float3 centroid = convexHull.triangles[i].getCentroid(ref convexHull.vertices);
                    Gizmos.color = (Vector3.Dot(centroid, convexHull.triangles[i].normal)) > 0f ? Color.red : Color.white;
                    Gizmos.DrawRay(centroid, convexHull.triangles[i].normal * 0.2f);
                }
            }
        }
    }
}
