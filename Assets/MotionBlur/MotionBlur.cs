using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MotionBlur : MonoBehaviour {

    public Mesh mesh;
    public Material blurredMaterial;
    public Transform start, end;
    [Range(1, 100)]
    public int numIntermediates = 100;
    Matrix4x4[] intermediatePositions = new Matrix4x4[100];

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() {
        if(numIntermediates != intermediatePositions.Length) { intermediatePositions = new Matrix4x4[numIntermediates]; }

        for (int i = 0; i < intermediatePositions.Length; i++) {
            intermediatePositions[i] = Matrix4x4.TRS(Vector3.Lerp(start.position, end.position,     (float) i / intermediatePositions.Length),
                                                     Quaternion.Slerp(start.rotation, end.rotation, (float) i / intermediatePositions.Length),
                                                     Vector3.Lerp(start.lossyScale, end.lossyScale, (float) i / intermediatePositions.Length));
        }

        blurredMaterial.SetInt("_numSamples", numIntermediates);

        Graphics.DrawMeshInstanced(mesh, 0, blurredMaterial, intermediatePositions);

    }
}
