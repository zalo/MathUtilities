using UnityEngine;

[ExecuteInEditMode]
public class Ellipsoid : MonoBehaviour {
    public Transform focus1, focus2;
    public float majorAxis, minorAxis;
    public bool isInside = false;
    public Bounds intersectionBounds;

    public Matrix4x4 localToWorld, worldToLocal;

    // Update is called once per frame
    public void Update() {
        Vector3 worldFocus1 = focus1.position;
        Vector3 worldFocus2 = focus2.position;

        transform.position = (worldFocus1 + worldFocus2) * 0.5f;
        transform.rotation = Quaternion.LookRotation(worldFocus2 - worldFocus1);
        transform.localScale = new Vector3(minorAxis, minorAxis,
                                           Mathf.Sqrt(Mathf.Pow(Vector3.Distance(worldFocus1,
                                                                                 worldFocus2) / 2, 2) +
                                                      Mathf.Pow(minorAxis / 2, 2)) * 2);

        localToWorld = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        worldToLocal = localToWorld.inverse;
        majorAxis = transform.localScale.z;
    }
}
