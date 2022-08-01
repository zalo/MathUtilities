using UnityEngine;
using System.Collections.Generic;

public class Pierce : MonoBehaviour {
    List<ConfigurableJoint> joints = new List<ConfigurableJoint>();
    Dictionary<Joint, Collider> jointToCollider = new Dictionary<Joint, Collider>();
    Rigidbody body;

    private void OnEnable() {
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        //foreach(ConfigurableJoint joint in joints) {
        for (int i = 0; i < joints.Count; i++) {
            Vector3 worldJointAnchor = transform.TransformPoint(joints[i].anchor);
            Vector3 worldConnectedAnchor = joints[i].connectedBody.transform.TransformPoint(joints[i].connectedAnchor);
            if (Vector3.Dot(transform.up, worldConnectedAnchor - worldJointAnchor) < -0.01f) {
                //Physics.IgnoreCollision(GetComponent<BoxCollider>(), jointToCollider[joints[i]], false);
                SetIgnoreCollision(jointToCollider[joints[i]], true);
                jointToCollider.Remove(joints[i]);
                Destroy(joints[i]);
                joints.RemoveAt(i);
                i--;
            }
        }
    }

    void TryToStabIntoObject(Collision collision) {
        if (collision.impulse.magnitude > 0.2f /*&& Vector3.Dot(collision.impulse, Vector3.up) >= -0.0f*/) {
            //Debug.Log(collision.impulse.magnitude, this);

            ConfigurableJoint linearJoint = gameObject.AddComponent<ConfigurableJoint>();
            linearJoint.autoConfigureConnectedAnchor = true;
            linearJoint.enableCollision = false;
            //linearJoint.axis = transform.InverseTransformDirection(Vector3.up);
            linearJoint.linearLimit = new SoftJointLimit { bounciness = 0f, contactDistance = 0.2f, limit = 0.22f };
            linearJoint.linearLimitSpring = new SoftJointLimitSpring { damper = 1000f, spring = 100000f };
            linearJoint.connectedBody = collision.rigidbody;
            linearJoint.anchor = transform.InverseTransformPoint(collision.contacts[0].point);

            linearJoint.xMotion = ConfigurableJointMotion.Locked;
            linearJoint.yMotion = ConfigurableJointMotion.Limited;
            linearJoint.zMotion = ConfigurableJointMotion.Locked;
            linearJoint.angularXMotion = ConfigurableJointMotion.Locked;
            linearJoint.angularYMotion = ConfigurableJointMotion.Locked;
            linearJoint.angularZMotion = ConfigurableJointMotion.Locked;

            linearJoint.yDrive = new JointDrive { positionDamper = 10000f, positionSpring = 0f, maximumForce = 10000000f };

            jointToCollider.Add(linearJoint, collision.collider);
            //Physics.IgnoreCollision(GetComponent<BoxCollider>(), collision.collider, true);
            SetIgnoreCollision(collision.collider, true);

            joints.Add(linearJoint);
        }
    }

    void SetIgnoreCollision(Collider collider, bool enabled) {
        foreach(Collider otherCollider in GetComponentsInChildren<Collider>()) {
            if (!otherCollider.name.Contains("Guard")) {
                Physics.IgnoreCollision(otherCollider, collider, enabled);
            }
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.rigidbody == null) { return; }
        TryToStabIntoObject(collision);
    }

    void OnCollisionStay(Collision collision) {
        if (collision.rigidbody == null) { return; }
        TryToStabIntoObject(collision);
    }

    void OnCollisionExit(Collision collision) {
        //if (collision.rigidbody == null) { return; }
        //TryToStabIntoObject(collision);
    }
}