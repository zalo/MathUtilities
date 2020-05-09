using UnityEngine;

[ExecuteInEditMode]
public class MirrorSystem : MonoBehaviour {

    const int MAX_REFLECTORS = 16;

    public Transform ellipsoids;
    [Range(0.0f, 0.02f)]
    public float apertureSize = 0.007f;
    [Range(0.2f, 0.75f)]
    public float focalDistance = 0.05f;

    int         reflectors = 1;
    Matrix4x4[] ellipseWorldToLocal = new Matrix4x4[MAX_REFLECTORS];
    Matrix4x4[] ellipseLocalToWorld = new Matrix4x4[MAX_REFLECTORS];
    float    [] minorAxes           = new float    [MAX_REFLECTORS];
    float    [] majorAxes           = new float    [MAX_REFLECTORS];
    float    [] isInside            = new float    [MAX_REFLECTORS];
    Vector4  [] boundsMin           = new Vector4  [MAX_REFLECTORS];
    Vector4  [] boundsMax           = new Vector4  [MAX_REFLECTORS];

    public Transform quads;
    int planes = 1;
    Matrix4x4[] quadLocalToWorld    = new Matrix4x4[MAX_REFLECTORS];
    Matrix4x4[] quadWorldToLocal    = new Matrix4x4[MAX_REFLECTORS];

    Material  reflectorShader;

    void Start() {
        // Depth is necessary to intersect with objects...
        FindObjectOfType<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }

    void Update() {
        if(reflectorShader == null) reflectorShader = GetComponent<Renderer>().sharedMaterial;

#if UNITY_EDITOR
        UnityEditor.Tools.hidden = false;
#endif

        reflectors = 0;
        foreach(Transform ell in ellipsoids) {
            if (!ell.gameObject.activeInHierarchy) continue;

            Ellipsoid ellipsoid = ell.GetComponent<Ellipsoid>();
            ellipseLocalToWorld[reflectors] = ellipsoid.localToWorld;
            ellipseWorldToLocal[reflectors] = ellipsoid.worldToLocal;
            minorAxes[reflectors]           = ellipsoid.minorAxis;
            majorAxes[reflectors]           = ellipsoid.majorAxis;
            isInside [reflectors]           = ellipsoid.isInside ? 1f : -1f; // -1 is exterior reflection, 1 is interior reflection
            boundsMin[reflectors]           = ellipsoid.intersectionBounds.min;
            boundsMax[reflectors]           = ellipsoid.intersectionBounds.max;

            reflectors++;
        }

        planes = 0;
        foreach (Transform quad in quads) {
            if (!quad.gameObject.activeInHierarchy) continue;

            quadLocalToWorld[planes] = Matrix4x4.TRS(quad.position, quad.rotation, quad.localScale);
            quadWorldToLocal[planes] = quadLocalToWorld[planes].inverse;

            planes++;
        }

        reflectorShader.SetInt        ("_Reflectors",     reflectors);
        reflectorShader.SetFloat      ("_FocalDistance",  focalDistance);
        reflectorShader.SetFloat      ("_ApertureSize",   apertureSize);
        reflectorShader.SetMatrixArray("_worldToSpheres", ellipseWorldToLocal);
        reflectorShader.SetMatrixArray("_sphereToWorlds", ellipseLocalToWorld);
        reflectorShader.SetFloatArray ("_MajorAxes",      majorAxes);
        reflectorShader.SetFloatArray ("_MinorAxes",      minorAxes);
        reflectorShader.SetFloatArray ("_IsInsides",      isInside);
        reflectorShader.SetVectorArray("_BoundsMin",      boundsMin);
        reflectorShader.SetVectorArray("_BoundsMax",      boundsMax);

        reflectorShader.SetInt        ("_Planes",         planes);
        reflectorShader.SetMatrixArray("_planeToWorlds",  quadLocalToWorld);
        reflectorShader.SetMatrixArray("_worldToPlanes",  quadWorldToLocal);
    }
}
