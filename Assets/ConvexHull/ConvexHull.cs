using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

[ExecuteInEditMode]
public class ConvexHull : MonoBehaviour {

    public struct ConvexHullData : IDisposable {
        public NativeList<VertexData> vertices;
        public NativeList<int3>       triangles;

        /// <summary> Initialize an empty Convex Hull Object.</summary>
        public ConvexHullData(int initSize = 4) {
            vertices  = new NativeList<VertexData>(initSize, Allocator.Persistent);
            triangles = new NativeList<int3      >(initSize, Allocator.Persistent);
        }

        /// <summary> Initialize a full Convex Hull Object.</summary>
        public ConvexHullData(IEnumerable<Vector3> hullPoints) {
            vertices  = new NativeList<VertexData>(4, Allocator.Persistent);
            triangles = new NativeList<int3      >(4, Allocator.Persistent);

            List<Vector3> pointsList = new List<Vector3>(hullPoints);

            // Step 1: Select four points arbitrarily to form the initial simplex (tetrahedron)
            ConstructInitialSimplex(ref pointsList);

            // Steps 2-6: Expand the Convex Hull to encompass all the points
            for (int p = 4; p < pointsList.Count; p++) { GrowHull(pointsList[p]); }
        }

        ///<summary>Create a simplex from a list of points.</summary>
        public void ConstructInitialSimplex(ref List<Vector3> points) {
            Dispose(); this = new ConvexHullData(4);

            // Pick some points far from the centroid to use as initial simplex
            Vector3 avg = Vector3.zero; foreach (Vector3 p in points) { avg += p; }
            avg /= points.Count;
            points.Sort((a, b) => (b - avg).sqrMagnitude.CompareTo((a - avg).sqrMagnitude));

            vertices.Add(new VertexData() { position = points[0] });
            vertices.Add(new VertexData() { position = points[1] });
            vertices.Add(new VertexData() { position = points[2] });
            vertices.Add(new VertexData() { position = points[3] });

            // Create simplex depending on the handedness
            // https://github.com/OskarSigvardsson/unity-quickhull/blob/master/Scripts/ConvexHullCalculator.cs#L373-L383
            if (Vector3.Dot(points[3] - points[1], Vector3.Cross(points[1] - points[0], points[2] - points[0])) > 0.0f) {
                triangles.Add(new int3(0, 2, 1));
                triangles.Add(new int3(0, 1, 3));
                triangles.Add(new int3(0, 3, 2));
                triangles.Add(new int3(1, 2, 3));
            } else {
                triangles.Add(new int3(0, 1, 2));
                triangles.Add(new int3(0, 3, 1));
                triangles.Add(new int3(0, 2, 3));
                triangles.Add(new int3(1, 3, 2));
            }
        }

        public bool GrowHull(float3 newPoint) {
            bool isOutsideOfHull = false;

            // Step 2: Clean the way for ourselves (Resets the "isFacing" sentinels on the Vertices)
            for (int i = 0; i < vertices.Length; i++) { vertices[i] = new VertexData(vertices[i]); }

            // Step 3: Mark all the vertices on the horizon
            for (int i = triangles.Length - 1; i >= 0; i--) {
                // Check if this triangle is facing toward our next point
                bool triangleIsFacing = isFacingPoint(triangles[i], newPoint);

                // Mark each vertex with whether it's on a triangle facing the incoming point or away from it
                // A vertex must be on both kinds of triangles to be on the horizon!!
                vertices[triangles[i].x] = new VertexData(vertices[triangles[i].x], triangleIsFacing ? 1 : -1);
                vertices[triangles[i].y] = new VertexData(vertices[triangles[i].y], triangleIsFacing ? 1 : -1);
                vertices[triangles[i].z] = new VertexData(vertices[triangles[i].z], triangleIsFacing ? 1 : -1);

                if (triangleIsFacing) {
                    isOutsideOfHull = true;
                    triangles.RemoveAtSwapBack(i);
                }
            }

            if (isOutsideOfHull) {
                // Step 4: Remind each vertex what its original index was (foreshadowing!)
                for (int i = 0; i < vertices.Length; i++) { vertices[i] = new VertexData(vertices[i], newIndex: i); }
                vertices.Add(new VertexData() { position = newPoint, index = vertices.Length - 1 });

                // Step 5: Sort the horizon vertices clockwise about the point-average-average axis
                List<VertexData> horizonVerts = new List<VertexData>();
                for (int i = 0; i < vertices.Length; i++) {
                    if (vertices[i].isHorizon()) {
                        horizonVerts.Add(vertices[i]);
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
                    triangles.Add(new int3(horizonVerts[i].index, horizonVerts[(i + 1) % horizonVerts.Count].index, vertices.Length - 1));
                }
            }

            return isOutsideOfHull;
        }

        public struct VertexData {
            public int isFacing;
            public int isNotFacing;
            public int index;
            public float3 position;

            /// <summary> Gets whether this vertex is on the edge-loop of the latest point being added </summary>
            public bool isHorizon() { return isFacing > 0 && isNotFacing > 0; }
            public VertexData(VertexData data, int isCurrentlyFacing = 0, int newIndex = -1) {
                position = data.position;
                if (isCurrentlyFacing == 0) {
                    if (newIndex == -1) {
                             isFacing = 0;             isNotFacing = 0;
                    } else { isFacing = data.isFacing; isNotFacing = data.isNotFacing; }
                } else {
                    isFacing    = data.isFacing    + (isCurrentlyFacing ==  1 ? 1 : 0);
                    isNotFacing = data.isNotFacing + (isCurrentlyFacing == -1 ? 1 : 0);
                }
                index = newIndex < 0 ? data.index : newIndex;
            }
        }

        /// <summary> Check if a point is inside this convex hull.</summary>
        public bool isInside(float3 point) {
            for (int i = 0; i < triangles.Length; i++) {
                if (isFacingPoint(triangles[i], point)) { return false; }
            }
            return true;
        }

        /// <summary> Get the centroid of a triangle's vertices.</summary>
        public static float3 getCentroid(int3 vertices, ref NativeList<VertexData> inVertices) {
            return (inVertices[vertices.x].position +
                    inVertices[vertices.y].position +
                    inVertices[vertices.z].position) * 0.333f;
        }

        /// <summary> Get whether this triangle's front-face is showing towards a point.</summary>
        public bool isFacingPoint(int3 triangle, Vector3 point) {
            return Vector3.Dot((float3)point - getCentroid(triangle, ref vertices),
                Vector3.Cross(vertices[triangle.y].position - vertices[triangle.x].position,
                              vertices[triangle.z].position - vertices[triangle.x].position)) > 0f;
        }

        /// <summary> Copies this Convex Hull to a Unity Mesh object.</summary>
        public void CopyToUnityMesh(ref Mesh unityMesh) {
            if (unityMesh == null) { unityMesh = new Mesh(); }
            Vector3[] uVertices = new Vector3[vertices.Length];
            int[] uTriangles = new int[triangles.Length * 3];
            for (int i = 0; i < vertices.Length; i++) {
                uVertices[i] = vertices[i].position;
            }
            for (int i = 0; i < uTriangles.Length; i += 3) {
                uTriangles[i + 0] = triangles[i / 3].x;
                uTriangles[i + 1] = triangles[i / 3].y;
                uTriangles[i + 2] = triangles[i / 3].z;
            }
            unityMesh.SetVertices(uVertices); unityMesh.SetTriangles(uTriangles, 0);
            unityMesh.RecalculateNormals();
        }

        /// <summary>Properly clean up the Convex Hull's NativeList's.</summary>
        public void Dispose() {
            if (vertices .IsCreated) { vertices .Dispose(); }
            if (triangles.IsCreated) { triangles.Dispose(); }
        }
    }

    [Tooltip("This transform represents an interactive point for testing the convex hull.")]
    public    Transform      interactivePoint;
    protected ConvexHullData convexHull;
    protected List<Vector3>  points;
    protected MeshFilter     filter;
    protected Mesh           hullMesh;

    void Update() {
        // Step 0: Contrive some points
        if (points == null) {
            points = new List<Vector3>();
            for (int i = 0; i < 30; i++) {
                points.Add(UnityEngine.Random.insideUnitSphere);
            }
        }

        // Construct the convex hull
        convexHull = new ConvexHullData(points);
        if (interactivePoint != null) { convexHull.GrowHull(interactivePoint.position); }

        // Copy the data into the Unity Mesh Representation
        convexHull.CopyToUnityMesh(ref hullMesh);
        if (filter == null || filter.sharedMesh == null) { filter = GetComponent<MeshFilter>(); filter.mesh = hullMesh; }
    }

    void OnDrawGizmosSelected() {
        if (points != null) {
            // Draw the vertices as yellow, the interior points as magenta, and the horizon points as cyan...
            for (int i = 0; i < points.Count; i++) {
                Gizmos.color = (i < convexHull.vertices.Length) ? (convexHull.vertices[i].isHorizon() ? Color.cyan : Color.yellow) : Color.green;
                Gizmos.color = (i > 6 && convexHull.isInside(points[i])) ? Color.magenta : Gizmos.color;
                Gizmos.DrawSphere(points[i], 0.05f);
            }

            // Draw all of the triangles in Wireframe
            if (convexHull.triangles.IsCreated) {
                for (int i = 0; i < convexHull.triangles.Length; i++) {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(
                        convexHull.vertices[convexHull.triangles[i].x].position,
                        convexHull.vertices[convexHull.triangles[i].y].position);
                    Gizmos.DrawLine(
                        convexHull.vertices[convexHull.triangles[i].y].position,
                        convexHull.vertices[convexHull.triangles[i].z].position);
                    Gizmos.DrawLine(
                        convexHull.vertices[convexHull.triangles[i].z].position,
                        convexHull.vertices[convexHull.triangles[i].x].position);
                }
            }
        }
    }

    void OnDisable() { convexHull.Dispose(); }
    void OnDestroy() { convexHull.Dispose(); }
}
