using UnityEngine;

[ExecuteInEditMode]
public class MirrorSystem : MonoBehaviour {

    int         reflectors = 1;
    Matrix4x4[] ellipseWorldToLocal = new Matrix4x4[16];
    Matrix4x4[] ellipseLocalToWorld = new Matrix4x4[16];
    float    [] minorAxes           = new float    [16];
    float    [] majorAxes           = new float    [16];
    float    [] isInside            = new float[16];

    Material  reflectorShader;

    void Start() {
        // Depth is necessary to intersect with objects...
        FindObjectOfType<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }

    void Update() {
        if(reflectorShader == null) reflectorShader = GetComponent<Renderer>().sharedMaterial;

        reflectors = 0;
        foreach(Transform ellipsoid in transform) {
            if (!ellipsoid.gameObject.activeInHierarchy) continue;

            Transform focus1 = ellipsoid.GetChild(0), 
                      focus2 = ellipsoid.GetChild(1);

            focus1.parent = null;
            focus2.parent = null;

            ellipsoid.position = (focus1.position + focus2.position) * 0.5f;
            ellipsoid.rotation = Quaternion.LookRotation(focus2.position - focus1.position);
            ellipsoid.localScale = new Vector3(ellipsoid.localScale.x, ellipsoid.localScale.x, 
                                               Mathf.Sqrt(Mathf.Pow(Vector3.Distance(focus1.position, 
                                                                                     focus2.position) / 2, 2) + 
                                                          Mathf.Pow(minorAxes[reflectors]             / 2, 2)) * 2);

            ellipseLocalToWorld[reflectors] = Matrix4x4.TRS(ellipsoid.position, ellipsoid.rotation, ellipsoid.localScale);
            ellipseWorldToLocal[reflectors] = ellipseLocalToWorld[reflectors].inverse;
            minorAxes[reflectors]           = ellipsoid.localScale.x;
            majorAxes[reflectors]           = ellipsoid.localScale.z;
            isInside [reflectors]           = -1f; // -1 is exterior reflection, 1 is interior reflection

            focus1.parent = ellipsoid;
            focus2.parent = ellipsoid;
            Vector3 invScale = new Vector3(
                1f / ellipsoid.localScale.x, 
                1f / ellipsoid.localScale.y, 
                1f / ellipsoid.localScale.z);
            focus1.localScale = Vector3.Scale(Vector3.one * 0.02f, invScale);
            focus2.localScale = Vector3.Scale(Vector3.one * 0.02f, invScale);

            reflectors++;
        }

        reflectorShader.SetFloat      ("_Reflectors",     reflectors);
        reflectorShader.SetMatrixArray("_worldToSpheres", ellipseWorldToLocal);
        reflectorShader.SetMatrixArray("_sphereToWorlds", ellipseLocalToWorld);
        reflectorShader.SetFloatArray ("_MajorAxes",      majorAxes);
        reflectorShader.SetFloatArray ("_MinorAxes",      minorAxes);
        reflectorShader.SetFloatArray ("_IsInsides",      isInside);
    }
}
