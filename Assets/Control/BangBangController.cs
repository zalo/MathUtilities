using UnityEngine;
public class BangBangController : MonoBehaviour {
  //Math taken from: http://underactuated.csail.mit.edu/underactuated.html?chapter=9

  public float force = 1f;
  public Rigidbody body;
  public Transform target;

  void FixedUpdate() {
    Vector3 acceleration =
          new Vector3(bangBangControllerWithZero(body.position.x, body.velocity.x, target.position.x, force / body.mass) * force,
                      bangBangControllerWithZero(body.position.y, body.velocity.y, target.position.y, force / body.mass) * force,
                      bangBangControllerWithZero(body.position.z, body.velocity.z, target.position.z, force / body.mass) * force);
    //    new Vector3(bangBangController(body.position.x, body.velocity.x, target.position.x, force / body.mass) ? force : -force,
    //                bangBangController(body.position.y, body.velocity.y, target.position.y, force / body.mass) ? force : -force,
    //                bangBangController(body.position.z, body.velocity.z, target.position.z, force / body.mass) ? force : -force);

    body.AddForce(acceleration, ForceMode.Force);
    ParticleSystem.EmissionModule particles = GetComponent<ParticleSystem>().emission;
    if (acceleration.magnitude >= force) {
      transform.rotation = Quaternion.LookRotation(acceleration);
      particles.enabled = true;
    } else {
      particles.enabled = false;
    }
    Debug.DrawRay(transform.position, -acceleration / force);
  }

  bool bangBangController(float currentPosition, float currentVelocity, float target, float maxForceDividedByMass) {
    return (currentVelocity < 0f  && currentPosition - target <= 0.5f * Mathf.Pow(currentVelocity, 2f) / maxForceDividedByMass) ||
           (currentVelocity >= 0f && currentPosition - target < -0.5f * Mathf.Pow(currentVelocity, 2f) / maxForceDividedByMass);
  }

  int bangBangControllerWithZero(float currentPosition, float currentVelocity, float target, float maxForceDividedByMass, float epsilon = 0.01f) {
    float disparity = currentPosition - target;
    float parabola = 0.5f * Mathf.Pow(currentVelocity, 2f) / maxForceDividedByMass;
    if ((currentVelocity <  0f && disparity + epsilon <= parabola) ||
        (currentVelocity >= 0f && disparity + epsilon < -parabola)) {
      return 1;
    } else 
    if ((currentVelocity <  0f && disparity - epsilon >= parabola) ||
        (currentVelocity >= 0f && disparity - epsilon > -parabola)) {
      return -1;
    } else {
      return 0;
    }
  }
}
