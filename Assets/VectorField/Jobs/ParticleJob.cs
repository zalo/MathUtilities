using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class ParticleJob : MonoBehaviour {
  public VectorField vectorField;
  public Mesh sphereMesh;
  public Material sphereMaterial;
  int numParticles = 1023;
  ParticleData particleData;
  Matrix4x4[] particles;

  public struct ParticleData : IJobParallelFor, System.IDisposable {
    [ReadOnly]
    public VectorFieldStruct field;
    public NativeArray<Vector3> particlesCurrent;
    public NativeArray<Vector3> particlesPrevious;
    [WriteOnly]
    public NativeArray<Matrix4x4> particleMatrices;

    public ParticleData(int numParticles, VectorFieldStruct field) {
      particlesCurrent = new NativeArray<Vector3>(numParticles, Allocator.Persistent);
      particlesPrevious = new NativeArray<Vector3>(numParticles, Allocator.Persistent);
      particleMatrices = new NativeArray<Matrix4x4>(numParticles, Allocator.Persistent);
      for (int i = 0; i < numParticles; i++) {
        particlesCurrent[i] = Random.insideUnitSphere + (Vector3.one*0.5f);
        particlesPrevious[i] = particlesCurrent[i] + (Random.insideUnitSphere * 0.01f);
      }
      this.field = field;
    }

    public void Execute(int i) {
      //Sample the Vector Field
      particlesCurrent[i] += field.trilinearSample(particlesCurrent[i]) * 0.1f;

      //Integrate the Particle via Verlet Integration
      Vector3 temp = particlesCurrent[i];
      particlesCurrent[i] += (particlesCurrent[i] - particlesPrevious[i]);
      particlesPrevious[i] = temp;

      //Write the particle positions out to transformation matrices
      particleMatrices[i] = Matrix4x4.TRS(particlesCurrent[i], Quaternion.identity, Vector3.one * 0.01f);
    }

    public void Dispose() {
      particlesCurrent.Dispose();
      particlesPrevious.Dispose();
      particleMatrices.Dispose();
    }
  }

  void Start() {
    if (!vectorField.hasStarted) { vectorField.Start(); }
    particles = new Matrix4x4[numParticles];
    particleData = new ParticleData(numParticles, vectorField.field);
  }

  void Update() {
    particleData.Schedule(particles.Length, 128).Complete();
    particleData.particleMatrices.CopyTo(particles);
    Graphics.DrawMeshInstanced(sphereMesh, 0, sphereMaterial, particles);
  }

  void OnDestroy() {
    particleData.Dispose();
  }
}
