using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class ParticleJob : MonoBehaviour {
  public VectorField vectorField;
  public int numParticles = 500;
  ParticleData particleData;
  Vector3[] particles;

  public struct ParticleData : IJobParallelFor, System.IDisposable {
    [ReadOnly]
    public VectorFieldStruct field;
    public NativeArray<Vector3> particlesCurrent;
    public NativeArray<Vector3> particlesPrevious;

    public ParticleData(int numParticles, VectorFieldStruct field) {
      particlesCurrent = new NativeArray<Vector3>(numParticles, Allocator.Persistent);
      particlesPrevious = new NativeArray<Vector3>(numParticles, Allocator.Persistent);
      for (int i = 0; i < numParticles; i++) {
        particlesCurrent[i] = Random.insideUnitSphere + (Vector3.one*0.5f);
        particlesPrevious[i] = particlesCurrent[i] + (Random.insideUnitSphere * 0.01f);
      }
      this.field = field;
    }

    public void Execute(int i) {
      particlesCurrent[i] += field.trilinearSample(particlesCurrent[i]) * 0.1f;

      Vector3 temp = particlesCurrent[i];
      particlesCurrent[i] += (particlesCurrent[i] - particlesPrevious[i]);
      particlesPrevious[i] = temp;
    }

    public void Dispose() {
      particlesCurrent.Dispose();
      particlesPrevious.Dispose();
    }
  }

  void Start() {
    if (!vectorField.hasStarted) { vectorField.Start(); }
    particles = new Vector3[numParticles];
    particleData = new ParticleData(numParticles, vectorField.field);
  }

  void Update() {
    particleData.Schedule(particles.Length, 256).Complete();
  }

  public void OnDrawGizmos() {
    if (Application.isPlaying && particles != null) {
      particleData.particlesCurrent.CopyTo(particles);

      for (int i = 0; i < particles.Length; i++) {
        Gizmos.DrawSphere(particles[i], 0.01f);
      }
    }
  }

  void OnDestroy() {
    particleData.Dispose();
  }
}
