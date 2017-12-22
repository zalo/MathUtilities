using UnityEngine;

public class Particles : MonoBehaviour {
  public VectorField field;
  Vector3[] particles;
  Vector3[] prevParticles;

  void Start() {
    particles = new Vector3[100];
    prevParticles = new Vector3[particles.Length];

    for (int i = 0; i < particles.Length; i++) {
      particles[i] = Random.insideUnitSphere;
      prevParticles[i] = particles[i] + (Random.insideUnitSphere * 0.01f);
    }
  }

  public void OnDrawGizmos() {
    if (Application.isPlaying && particles != null) {
      for (int i = 0; i < particles.Length; i++) {
        Gizmos.DrawSphere(particles[i], 0.01f);

        particles[i] += field.trilinearSample(particles[i]) * 0.1f;

        //Verlet
        Vector3 temp = particles[i];
        particles[i] += (particles[i] - prevParticles[i]);
        //particles[i] = (Mathf.Clamp((particles[i] - prevParticles[i]).magnitude, 0f, 100f) * (particles[i] - prevParticles[i])) + temp;
        prevParticles[i] = temp;
      }
    }
  }
}