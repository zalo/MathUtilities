using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class WeightedAverageSpline : MonoBehaviour {
    [Range(1, 15)]
    public int averagingWindow = 5;

    public Mesh cubeMesh;

    // Jittered Poses State
    List<Pose> jitteredPoses = new List<Pose>();

    void OnDrawGizmos() {
        if(jitteredPoses.Count == 0) {
            jitteredPoses.Clear();
            for (float i = 0; i <= 360f; i+= 360f / 40f) {
                jitteredPoses.Add(new Pose(Quaternion.Euler(0f, 0f, -i) * Vector3.up * 5f + Random.insideUnitSphere, 
                                           Quaternion.Slerp(Quaternion.identity, Random.rotationUniform, 0.3f) * Quaternion.Euler(0f, 0f, -i)));
            }
        }

        Gizmos.color = new Color(0.3f, 0.3f, 0.3f, 0.3f);
        foreach(Pose pose in jitteredPoses) {
            Gizmos.DrawMesh(cubeMesh, pose.position, pose.rotation);
        }

        // Draw Bumpy (Naively Interpolated) Pose
        Gizmos.color = new Color(1.0f, 0.3f, 0.3f, 1f);
        Pose bumpyPose = smoothPoseAtIndex(jitteredPoses, Mathf.Repeat((float)EditorApplication.timeSinceStartup * 5f, jitteredPoses.Count - 1), 1);
        Gizmos.DrawMesh(cubeMesh, bumpyPose.position, bumpyPose.rotation);

        // Draw Smoothed Pose
        Gizmos.color = new Color(0.3f, 1.0f, 0.3f, 1f);
        Pose smoothedPose = smoothPoseAtIndex(jitteredPoses, Mathf.Repeat((float)EditorApplication.timeSinceStartup * 5f, jitteredPoses.Count - 1), averagingWindow);
        Gizmos.DrawMesh(cubeMesh, smoothedPose.position, smoothedPose.rotation);
    }

    Pose smoothPoseAtIndex(List<Pose> poses, float index, float averagingWindow = 5) {
        Vector3 accumulatedPosition = Vector3.zero;
        Vector4 accumulatedRotation = Vector4.zero;
        float   accumulatedWeight   = 0f;

        // Prevent Slowing down near the beginning or end of the sequence
        averagingWindow = Mathf.Max(Mathf.Min(Mathf.Min(averagingWindow, index), (poses.Count - 1) - index), 1f);

        for (int i = 0; i < poses.Count; i++) {
            Pose curPose = poses[Mathf.Clamp(i, 0, poses.Count-1)];
            float poseWeight = Mathf.Max(averagingWindow - Mathf.Abs(i - index), 0f);

            if (poseWeight > 0f) {
                accumulatedPosition += curPose.position * poseWeight;
                // TODO: Enforce a positive quaternion hemisphere for a valid accumulation
                accumulatedRotation += new Vector4(curPose.rotation.x, curPose.rotation.y, curPose.rotation.z, curPose.rotation.w) * poseWeight;
                accumulatedWeight   += poseWeight;
            }
        }

        Vector4 averageRotationVector = accumulatedRotation / accumulatedWeight;
        Quaternion averageRotation = new Quaternion(averageRotationVector.x, averageRotationVector.y, averageRotationVector.z, averageRotationVector.w).normalized;

        return new Pose(accumulatedPosition / accumulatedWeight, averageRotation);
    }
}
