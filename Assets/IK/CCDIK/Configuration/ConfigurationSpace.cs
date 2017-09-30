using UnityEngine;

public class ConfigurationSpace : MonoBehaviour {
  public CCDIKJoint[] joints;
  public SphereCollider sphere;
  public Transform floor;
  public Material volumeMaterial;
  public int evaluationsPerFrame = 1;
  public Transform configurationVolume;
  public Transform configurationCursor;

  const int resolution = 64;
  Texture3D configurationSpace;
  Color32[] distances = new Color32[resolution * resolution * resolution];

  const float twistHalfRange = 180f;
  const float arm1HalfRange = 100;
  const float arm2HalfRange = 140;

  float curTwist = 0;
  float curArm1 = 0;
  float curArm2 = 0;

  float twistStep = (twistHalfRange * 2f) / resolution;
  float arm1Step = (arm1HalfRange * 2f) / resolution;
  float arm2Step = (arm2HalfRange * 2f) / resolution;

  bool textureComplete = false;

  //Initialize the Arm's Kinematics
  void Start() {
    joints[2].transform.localRotation = Quaternion.Euler(joints[2].axis * ((curArm2 * arm2Step) - arm2HalfRange));
    joints[3].transform.localRotation = Quaternion.Euler(joints[3].axis * ((curArm1 * arm1Step) - arm1HalfRange));
    joints[4].transform.localRotation = Quaternion.Euler(joints[4].axis * ((curTwist * twistStep) - twistHalfRange));

    configurationSpace = new Texture3D(resolution, resolution, resolution, TextureFormat.ARGB32, false);
    configurationSpace.wrapMode = TextureWrapMode.Clamp;
    if (volumeMaterial.mainTexture == null) { volumeMaterial.mainTexture = configurationSpace; }
  }

  //Incrementally update the configuration space evaluation
  void Update() {
    if (!textureComplete) {
      for (int i = 0; i < evaluationsPerFrame; i++) {
        //Find the distance to the nearest joint
        float distance = distanceToNearestObstructor();

        //Write to the configuration space texture array
        //The 0-1 range here encodes the -50cm to +50cm range of distance values as a float
        distances[(int)((curTwist * resolution * resolution) + (curArm1 * resolution) + curArm2)] = encodeFloatRGBA(Mathf.Clamp01(distance + 0.5f));

        //Increment the configuration space coordinates
        incrementConfigurationSpace();
      }

      //Fill the configuration space texture and apply it
      configurationSpace.SetPixels32(distances);
      configurationSpace.Apply();
    } else {
      //Find the distance to the nearest joint
      Vector3 gradient = sampleConfigurationSpaceGradient(configurationCursor.position);
      float distance = sampleConfigurationSpace(configurationCursor.position);

      //Depenetrate the arm using the configuration space gradient
      //Pretty cool :)
      if(distance < 0) { configurationCursor.position += gradient*0.1f; }

      configurationCursor.GetComponent<Renderer>().material.color = Color.Lerp(Color.red, Color.green, distance * 300f);

      //Clamp the cursor to remain inside of the configuration space
      Vector3 localSphere = configurationVolume.InverseTransformPoint(configurationCursor.position);
      localSphere = new Vector3(Mathf.Clamp(localSphere.x, -0.5f, 0.5f), Mathf.Clamp(localSphere.y, -0.5f, 0.5f), Mathf.Clamp(localSphere.z, -0.5f, 0.5f));
      configurationCursor.position = configurationVolume.TransformPoint(localSphere);
    }
  }

  //Increment the joint angles on the arm to sweep through the configuration space
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
      textureComplete = true;
    }
  }

  Vector3 sampleConfigurationSpaceGradient(Vector3 pos, float epsilon = 0.05f) {
    float distanceAtPos = sampleConfigurationSpace(pos);
    return new Vector3(sampleConfigurationSpace(pos + (Vector3.right   * epsilon)) - distanceAtPos,
                       sampleConfigurationSpace(pos + (Vector3.up      * epsilon)) - distanceAtPos,
                       sampleConfigurationSpace(pos + (Vector3.forward * epsilon)) - distanceAtPos)/epsilon;
  }

  float sampleConfigurationSpace(Vector3 pos) {
    Vector3 configurationPos = configurationVolume.InverseTransformPoint(pos) + (Vector3.one * 0.5f);
    joints[2].transform.localRotation = Quaternion.Euler(joints[2].axis * (((Mathf.Clamp01(configurationPos.x) * resolution) * arm2Step) - arm2HalfRange));
    joints[3].transform.localRotation = Quaternion.Euler(joints[3].axis * (((Mathf.Clamp01(configurationPos.y) * resolution) * arm1Step) - arm1HalfRange));
    joints[4].transform.localRotation = Quaternion.Euler(joints[4].axis * (((Mathf.Clamp01(configurationPos.z) * resolution) * twistStep) - twistHalfRange));
    return distanceToNearestObstructor();
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

  //Encode a float to four bytes (without generating garbage) for writing to the Configuration space volume texture
  Color32 encodeFloatRGBA(float input) {
    float r = input;
    float g = input * 255;
    float b = input * 255 * 255;
    float a = input * 255 * 255 * 255;

    r = r - Mathf.Floor(r);
    g = g - Mathf.Floor(g);
    b = b - Mathf.Floor(b);
    a = a - Mathf.Floor(a);

    return new Color32((byte)(r*255), (byte)(g*255), (byte)(b*255), (byte)(a*255));
  }
}
