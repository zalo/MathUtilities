using UnityEngine;

public class Particles : MonoBehaviour {
  public ParticleModifier modifier;
  public int numParticles = 100;
  public float startingRadius = 0.5f;
  public float startingRandomVelocityMultiplier = 0.01f;
  [Range(0f, 1f)]
  public float drag = 0f;

  Vector3[] particles;
  Vector3[] prevParticles;

  void Start() {
    particles = new Vector3[numParticles];
    prevParticles = new Vector3[particles.Length];

    for (int i = 0; i < particles.Length; i++) {
      particles[i] = Random.insideUnitSphere * startingRadius;
      prevParticles[i] = particles[i] + (Random.insideUnitSphere * startingRandomVelocityMultiplier);
    }
  }

  void Update() {
    for (int i = 0; i < particles.Length; i++) {
      bool reset = false;
      if (!new Bounds(Vector3.zero, Vector3.one * startingRadius*2f).Contains(particles[i])) {
        particles[i] = new Vector3(-startingRadius, Random.Range(-startingRadius, startingRadius), Random.Range(-startingRadius, startingRadius));//Random.insideUnitSphere * startingRadius;
        prevParticles[i] = particles[i] + (Random.insideUnitSphere * startingRandomVelocityMultiplier);
        reset = true;
      }
      if (modifier != null && modifier.enabled) modifier.ModifierOffset(i, ref particles[i], ref prevParticles[i], reset);

      //Verlet
      Vector3 temp = particles[i];
      particles[i] += (particles[i] - prevParticles[i]) * (1f - drag);
      //particles[i] = (Mathf.Clamp((particles[i] - prevParticles[i]).magnitude, 0f, 100f) * (particles[i] - prevParticles[i])) + temp;
      prevParticles[i] = temp;
    }
  }

  public void OnDrawGizmos() {
    if (Application.isPlaying && particles != null) {
      for (int i = 0; i < particles.Length; i++) {
        Gizmos.DrawSphere(particles[i], 0.01f);
      }
    }
  }
}