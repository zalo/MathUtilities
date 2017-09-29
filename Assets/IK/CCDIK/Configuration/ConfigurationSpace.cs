using UnityEngine;

public class ConfigurationSpace : MonoBehaviour {
  public CCDIKJoint[] joints;
  public SphereCollider sphere;
  public Transform floor;
  public Material volumeMaterial;
  public int evaluationsPerFrame = 1;

  const int resolution = 64;
  Texture3D configurationSpace;
  Color32[] distances = new Color32[resolution * resolution * resolution];

  const float twistHalfRange = 180f;
  const float arm1HalfRange = 100;
  const float arm2HalfRange = 140;

  int curTwist = 0;
  int curArm1 = 0;
  int curArm2 = 0;

  float twistStep = (twistHalfRange * 2f) / resolution;
  float arm1Step = (arm1HalfRange * 2f) / resolution;
  float arm2Step = (arm2HalfRange * 2f) / resolution;

  //Initialize the Arm's Kinematics
  void Start() {
    configurationSpace = new Texture3D(resolution, resolution, resolution, TextureFormat.ARGB32, false);
    joints[2].transform.localRotation = Quaternion.Euler(joints[2].axis * ((curArm2 * arm2Step) - arm2HalfRange));
    joints[3].transform.localRotation = Quaternion.Euler(joints[3].axis * ((curArm1 * arm1Step) - arm1HalfRange));
    joints[4].transform.localRotation = Quaternion.Euler(joints[4].axis * ((curTwist * twistStep) - twistHalfRange));

    volumeMaterial.mainTexture = configurationSpace;
  }

  //Incrementally update the configuration space evaluation
  void Update() {
    for (int i = 0; i < evaluationsPerFrame; i++) {
      //Find the distance to the nearest joint
      float distance = distanceToNearestObstructor();
      byte distanceByte = (byte)((Mathf.Clamp01((distance) * 5f)) * 255f);

      //Write to the configuration space texture array
      distances[(curTwist * resolution * resolution) + (curArm1 * resolution) + curArm2] = new Color32(distanceByte, 0, 0, distanceByte);

      //Increment the configuration space coordinates
      incrementConfigurationSpace();
    }

    //Fill the configuration space texture and apply it
    configurationSpace.SetPixels32(distances);
    configurationSpace.Apply();
  }

  //Increment the joint angles on the arm
  void incrementConfigurationSpace() {
    curArm2++;
    joints[2].transform.localRotation = Quaternion.Euler(joints[2].axis * ((curArm2 * arm2Step) - arm2HalfRange));
    if (curArm2 >= resolution) {
      curArm2 = 0;
      joints[2].transform.localRotation = Quaternion.Euler(joints[2].axis * ((curArm2 * arm2Step) - arm2HalfRange));
      curArm1++;
      joints[3].transform.localRotation = Quaternion.Euler(joints[3].axis * ((curArm1 * arm1Step) - arm1HalfRange));
    }

    if (curArm1 >= resolution) {
      curArm1 = 0;
      joints[3].transform.localRotation = Quaternion.Euler(joints[3].axis * ((curArm1 * arm1Step) - arm1HalfRange));
      curTwist++;
      joints[4].transform.localRotation = Quaternion.Euler(joints[4].axis * ((curTwist * twistStep) - twistHalfRange));
    }

    if (curTwist >= resolution) {
      curTwist = 0;
      curArm1 = 0;
      curArm2 = 0;
      Start();
    }
  }

  //Returns the signed distance between the nearest joint and the nearest obstructor for this configuration (sphere and ground plane in this instance)
  float distanceToNearestObstructor() {
    float distance = 100f;
    for (int j = joints.Length - 1; j >= 0; j--) {
      float jointDistance = 100f;

      //Closest point on Sphere
      Vector3 jointPoint = Constraints.ClosestPointOnCapsule(sphere.transform.position, joints[j].GetComponent<Collider>() as CapsuleCollider);
      Vector3 offset = jointPoint - sphere.transform.position;
      float tempDist = offset.magnitude - (sphere.radius * sphere.transform.lossyScale.x);
      jointDistance = (jointDistance > tempDist) ? tempDist : jointDistance;

      //Closest point on Plane
      tempDist = distanceCapsuleToPlane(joints[j].GetComponent<Collider>() as CapsuleCollider, floor.position, -floor.forward.normalized);
      jointDistance = (jointDistance > tempDist) ? tempDist : jointDistance;

      //Color the colliding joint appropriately
      joints[j].transform.GetChild(1).GetComponent<Renderer>().material.color = Color.Lerp(Color.red, Color.green, jointDistance * 30f);

      //Update the minimum distance
      distance = (distance > jointDistance) ? jointDistance : distance;
    }
    return distance;
  }

  //Simple helper function that returns the distance between an (unscaled) capsule collider and a plane
  float distanceCapsuleToPlane(CapsuleCollider collider, Vector3 planePoint, Vector3 normal) {
    Vector3 offset = ((collider.direction == 0 ? Vector3.right : Vector3.up) * Mathf.Clamp01((collider.height * 0.5f) - collider.radius));
    Vector3 capsuleBeginning = collider.transform.TransformPoint(collider.center - offset);
    Vector3 capsuleEnd = collider.transform.TransformPoint(collider.center + offset);

    return Mathf.Min(Vector3.Dot(normal, capsuleBeginning - planePoint),
                     Vector3.Dot(normal, capsuleEnd -       planePoint)) - collider.radius;
  }
}
