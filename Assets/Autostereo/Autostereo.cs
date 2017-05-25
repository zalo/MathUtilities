using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autostereo : MonoBehaviour {
    Material autosteroMat;
    Vector4[] eyes = new Vector4[2];
    Vector4[] parallaxBarrierDimensions = new Vector4[3];
    Transform parallaxBarrier;

    void Start () {
        autosteroMat = GetComponent<Renderer>().sharedMaterial;
        parallaxBarrier = transform.GetChild(0);
    }

	void LateUpdate () {
        eyes[0] = Camera.main.transform.position - (Camera.main.transform.right * 0.032f);
        eyes[1] = Camera.main.transform.position + (Camera.main.transform.right * 0.032f);

        parallaxBarrierDimensions[0] = parallaxBarrier.position;
        parallaxBarrierDimensions[1] = parallaxBarrier.rotation * Vector3.right;
        parallaxBarrierDimensions[2] = parallaxBarrier.rotation * Vector3.down;

        autosteroMat.SetVectorArray("_eyes", eyes);
        autosteroMat.SetVectorArray("_parallaxBarrier", parallaxBarrierDimensions);
    }
}
