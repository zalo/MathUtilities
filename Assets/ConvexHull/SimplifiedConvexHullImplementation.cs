using UnityEngine;
using System.Collections.Generic;

/// <summary> Simplified Convex Hull Implementation; 4-5x slower but useful for learning and porting to other languages</summary>
/// Read preamble here for more intuition: https://github.com/OskarSigvardsson/unity-quickhull/blob/master/Scripts/ConvexHullCalculator.cs
public class SimpleConvexHull {
    /// <summary> Convex Hull State </summary>
    public List<VertexData> vertices;
    public List<Vector3Int> triangles;

    ///<summary>Create a convex hull from a list of points.  
    ///This will fail with fewer than 4 points or fully coplanar points.</summary>
    public SimpleConvexHull(List<Vector3> points) {
        // Step 1: Select four points arbitrarily to form the initial simplex (tetrahedron)
        ConstructInitialSimplex(points);

        // Steps 2-6: Expand the Convex Hull to encompass all the points
        for (int p = 4; p < points.Count; p++) { GrowHull(points[p]); }
    }

    ///<summary>Create a simplex from a list of points.</summary>
    public void ConstructInitialSimplex(List<Vector3> points) {
        if (vertices  == null) { vertices  = new List<VertexData>(); }
        if (triangles == null) { triangles = new List<Vector3Int>(); }
        vertices.Clear(); triangles.Clear();

        // Pick some points far from the centroid to use as initial simplex
        Vector3 avg = Vector3.zero; foreach (Vector3 p in points) { avg += p; } avg /= points.Count;
        points.Sort((a, b) => (b - avg).sqrMagnitude.CompareTo((a - avg).sqrMagnitude));

        vertices.Add(new VertexData() { position = points[0] });
        vertices.Add(new VertexData() { position = points[1] });
        vertices.Add(new VertexData() { position = points[2] });
        vertices.Add(new VertexData() { position = points[3] });

        // Create simplex depending on the handedness
        if (Vector3.Dot(points[3] - points[1], Vector3.Cross(points[1] - points[0], points[2] - points[0])) > 0.0f) {
            triangles.Add(new Vector3Int(0, 2, 1));
            triangles.Add(new Vector3Int(0, 1, 3));
            triangles.Add(new Vector3Int(0, 3, 2));
            triangles.Add(new Vector3Int(1, 2, 3));
        } else {
            triangles.Add(new Vector3Int(0, 1, 2));
            triangles.Add(new Vector3Int(0, 3, 1));
            triangles.Add(new Vector3Int(0, 2, 3));
            triangles.Add(new Vector3Int(1, 3, 2));
        }
    }

    /// <summary> Expand the convex hull if the new point is outside of it.</summary>
    public bool GrowHull(Vector3 newPoint) {
        bool isOutsideOfHull = false;

        // Step 2: Clean the way for ourselves (Resets the "isFacing" sentinels on the Vertices)
        for (int i = 0; i < vertices.Count; i++) { vertices[i] = new VertexData(vertices[i]); }

        // Step 3: Mark all the vertices on the horizon, remove soon-to-be internal triangles
        for (int i = triangles.Count - 1; i >= 0; i--) {
            // Check if this triangle is facing toward our next point
            bool triangleIsFacing = isFacingPoint(triangles[i], newPoint);

            // Mark each vertex with whether it's on a triangle facing the incoming point or away from it
            // A vertex must be on both kinds of triangles to be on the horizon!!
            vertices[triangles[i].x] = new VertexData(vertices[triangles[i].x], triangleIsFacing);
            vertices[triangles[i].y] = new VertexData(vertices[triangles[i].y], triangleIsFacing);
            vertices[triangles[i].z] = new VertexData(vertices[triangles[i].z], triangleIsFacing);

            if (triangleIsFacing) {
                isOutsideOfHull = true;
                triangles.RemoveAt(i);
            }
        }

        if (isOutsideOfHull) {
            // Step 4: Remind each vertex what its original index was (needed for the next step)
            for (int i = 0; i < vertices.Count; i++) { vertices[i] = new VertexData(vertices[i], i); }
            vertices.Add(new VertexData() { position = newPoint, index = vertices.Count - 1 });

            // Step 5: Sort the horizon vertices clockwise about the average->point axis
            List<VertexData> horizonVerts = new List<VertexData>();
            for (int i = 0; i < vertices.Count; i++) {
                if (vertices[i].isHorizon()) {
                    horizonVerts.Add(vertices[i]);
                }
            }
            Vector3 avg = Vector3.zero; foreach (VertexData p in horizonVerts) { avg += p.position; }
            avg /= horizonVerts.Count; Vector3 incomingAxis = newPoint - avg;
            horizonVerts.Sort(delegate (VertexData t1, VertexData t2) {
                Vector3 planart1 = Vector3.ProjectOnPlane(t1.position - avg, incomingAxis);
                Vector3 planart2 = Vector3.ProjectOnPlane(t2.position - avg, incomingAxis);
                Vector3 planarUp = Vector3.ProjectOnPlane(Vector3.up       , incomingAxis).normalized;
                return
                    Vector3.SignedAngle(planart2, planarUp, incomingAxis).CompareTo(
                    Vector3.SignedAngle(planart1, planarUp, incomingAxis));
            });

            // Step 6: Create new triangles around this "horizon"
            for (int i = 0; i < horizonVerts.Count; i++) {
                triangles.Add(new Vector3Int(horizonVerts[i].index, horizonVerts[(i + 1) % horizonVerts.Count].index, vertices.Count - 1));
            }
        }

        return isOutsideOfHull;
    }

    public struct VertexData {
        public Vector3 position;
        public int index;
        public bool isFacing, isNotFacing;

        /// <summary> This VertexData constructor resets the isFacing flags. </summary>
        public VertexData(VertexData data)               { position = data.position; index = data.index; isFacing = false;         isNotFacing = false; }
        /// <summary> This VertexData constructor lets the vertex know which index it is. </summary>
        public VertexData(VertexData data, int  inIndex) { position = data.position; index = inIndex;    isFacing = data.isFacing; isNotFacing = data.isNotFacing; }
        /// <summary> This VertexData constructor toggles the isFacing flags. </summary>
        public VertexData(VertexData data, bool inIsFacing) {
            position = data.position; index = data.index;
            isFacing    = data.isFacing    ||  inIsFacing;
            isNotFacing = data.isNotFacing || !inIsFacing;
        }

        /// <summary> Gets whether this vertex is on the edge-loop of the latest point being added. </summary>
        public bool isHorizon() { return isFacing && isNotFacing; }
    }

    /// <summary> Check if a point is inside this convex hull. </summary>
    public bool isInside(Vector3 point) {
        for (int i = 0; i < triangles.Count; i++) {
            if (isFacingPoint(triangles[i], point)) { return false; }
        }
        return true;
    }

    /// <summary> Get whether this triangle's front-face is showing towards a point.</summary>
    public bool isFacingPoint(Vector3Int triangle, Vector3 point) {
        return Vector3.Dot(point - (vertices[triangle.x].position + vertices[triangle.y].position + vertices[triangle.z].position) / 3f,
            Vector3.Cross(vertices[triangle.y].position - vertices[triangle.x].position,
                          vertices[triangle.z].position - vertices[triangle.x].position)) > 0f;
    }
}

[ExecuteInEditMode]
/// <summary> Test Rig Implementation </summary>
public class SimplifiedConvexHullImplementation : MonoBehaviour {

    [Tooltip("This transform represents an interactive point for testing the convex hull.")]
    public Transform interactivePoint;
    protected List<Vector3> points;
    protected SimpleConvexHull convexHull;

    void Update() {
        // Contrive some points for the test
        if (points == null) {
            points = new List<Vector3>();
            for (int i = 0; i < 300; i++) {
                points.Add(Random.insideUnitSphere);
            }
        }

        // Construct the hull and grow it to encompass the interactive test point
        convexHull = new SimpleConvexHull(points);
        if (interactivePoint != null) { convexHull.GrowHull(interactivePoint.position); }
    }

    void OnDrawGizmos() {
        if (convexHull != null && convexHull.vertices != null && convexHull.triangles != null) {
            // Draw all of the triangles in Wireframe
            Gizmos.color = Color.green;
            for (int i = 0; i < convexHull.triangles.Count; i++) {
                Gizmos.DrawLine(convexHull.vertices[convexHull.triangles[i].x].position, convexHull.vertices[convexHull.triangles[i].y].position);
                Gizmos.DrawLine(convexHull.vertices[convexHull.triangles[i].y].position, convexHull.vertices[convexHull.triangles[i].z].position);
                Gizmos.DrawLine(convexHull.vertices[convexHull.triangles[i].z].position, convexHull.vertices[convexHull.triangles[i].x].position);
            }
        }
    }
}
