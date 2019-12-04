using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pathfinding : MonoBehaviour {
    public bool relaxConstraints = false;
    NavMeshTriangulation mesh;
    HashSet<int>[] connections;
    float[,] distances;
    Vector4[] accumulatedDisplacements;

    void Update() {
        // Set up the Mesh and n-squared true distance matrix
        if (mesh.vertices == null) setUpMesh();

        if (relaxConstraints) {
            // Do an iteration of dense constraint satisfaction
            for (int i = 0; i < mesh.vertices.Length - 1; i++) {
                for (int j = i; j < mesh.vertices.Length; j++) {
                    float sqrDistance = distances[i, j];
                    sqrDistance *= sqrDistance;
                    if (sqrDistance > 0.0f) {
                        Vector3 offset = mesh.vertices[j] - mesh.vertices[i];
                        offset *= sqrDistance / (Vector3.Dot(offset, offset) + sqrDistance) - 0.5f;
                        accumulatedDisplacements[i] += new Vector4(-offset.x, -offset.y, -offset.z, 1f);
                        accumulatedDisplacements[j] += new Vector4( offset.x,  offset.y,  offset.z, 1f);
                    }
                }
            }
            for (int i = 0; i < mesh.vertices.Length; i++) {
                if (accumulatedDisplacements[i].w > 0f) {
                    mesh.vertices[i] += new Vector3(
                        accumulatedDisplacements[i].x,
                        accumulatedDisplacements[i].y,
                        accumulatedDisplacements[i].z) /
                        accumulatedDisplacements[i].w;
                }
                accumulatedDisplacements[i] = new Vector4(0, 0, 0, 1.0f);
            }
        }
    }

    public void OnDrawGizmos() {
        // Set up the Mesh and n-squared true distance matrix
        if (mesh.vertices == null) setUpMesh();

        Gizmos.color = Color.yellow;
        for (int i = 0; i < mesh.vertices.Length; i++) {
            Gizmos.DrawSphere(mesh.vertices[i], 0.1f);
        }

        Gizmos.color = Color.green;
        for (int i = 0; i < mesh.vertices.Length; i++) {
            //Gizmos.color = Color.HSVToRGB((i / 5.12354536f) % 1.0f, 1.0f, 1.0f); // Useful for debugging the duplicate nodes issue
            foreach (int connection in connections[i]) {
                Gizmos.DrawLine(mesh.vertices[i], ((mesh.vertices[connection] - mesh.vertices[i]) * 0.5f) + mesh.vertices[i]);
            }
        }
    }

    public void OnValidate() { setUpMesh(); }

    void setUpMesh() {
        // Allocate space for all our datastructures
        mesh = NavMesh.CalculateTriangulation();

        // Collapse Vertices!  wtf Unity giving me broken nodegraphs
        List<Vector3> collapsedVertices = new List<Vector3>(mesh.vertices);
        for (int i = 0; i < mesh.vertices.Length - 1; i++) {
            for (int j = collapsedVertices.Count - 1; j > i ; j--) {
                float distance = Vector3.Distance(collapsedVertices[i], collapsedVertices[j]);
                if(distance < 0.001f) {
                    collapsedVertices.RemoveAt(j);
                    for (int k = 0; k < mesh.indices.Length; k++) {
                        if (mesh.indices[k] == j) mesh.indices[k] = i;
                        if (mesh.indices[k] > j)  mesh.indices[k]--;
                    }
                }
            }
        }
        mesh.vertices = collapsedVertices.ToArray();

        connections = new HashSet<int>[mesh.vertices.Length];
        distances = new float[mesh.vertices.Length, mesh.vertices.Length];
        accumulatedDisplacements = new Vector4[mesh.vertices.Length];

        for (int i = 0; i < mesh.vertices.Length; i++) {
            connections[i] = new HashSet<int>();
            for (int j = 0; j < mesh.vertices.Length; j++) { distances[i,j] = 10000.0f; }
        }

        // Reprocess the mesh into a graph with a set of connections at each vertex
        for (int i = 0; i < mesh.indices.Length; i += 3) {
            connections[mesh.indices[i    ]].Add(mesh.indices[i + 1]);
            connections[mesh.indices[i    ]].Add(mesh.indices[i + 2]);
            connections[mesh.indices[i + 1]].Add(mesh.indices[i    ]);
            connections[mesh.indices[i + 1]].Add(mesh.indices[i + 2]);
            connections[mesh.indices[i + 2]].Add(mesh.indices[i    ]);
            connections[mesh.indices[i + 2]].Add(mesh.indices[i + 1]);
        }

        // Traverse the graph, calculate the n-squared distance matrix
        for (int i = 0; i < mesh.vertices.Length; i++) { depthFirstTraversal(i, i, 0f); } //breadthFirstTraversal(i); }//
    }

    /// <summary>Depth-First Traversal of Node-Graph NOT WHAT WE WANT</summary>
    void depthFirstTraversal(int startingNode, int currentNode, float currentDistance) {
        if (currentDistance < distances[startingNode, currentNode]) {
            distances[startingNode, currentNode] = currentDistance;
            foreach (int connection in connections[currentNode]) {
                //Debug.Log(currentNode + " visiting " + connection);
                if (connection != startingNode && connection != currentNode) {
                    depthFirstTraversal(startingNode, connection, currentDistance + Vector3.Distance(mesh.vertices[currentNode], mesh.vertices[connection]));
                }
            }
        }
    }
}
