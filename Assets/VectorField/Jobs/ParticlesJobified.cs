using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

public class ParticlesJobified : MonoBehaviour {
  public int numParticles = 10230;
  public VectorField vectorField;
  public Mesh sphereMesh;
  public Material sphereMaterial;
  ParticleData particleData;
  Matrix4x4[] particles;
  Matrix4x4[] instanceMatrices = new Matrix4x4[1023];

  public struct ParticleData : System.IDisposable {
    public NativeArray<Vector3> particlesCurrent;
    public NativeArray<Vector3> particlesPrevious;
    [WriteOnly]
    public NativeArray<Matrix4x4> particleMatrices;

    public ParticleData(int numParticles) {
      particlesCurrent = new NativeArray<Vector3>(numParticles, Allocator.Persistent);
      particlesPrevious = new NativeArray<Vector3>(numParticles, Allocator.Persistent);
      particleMatrices = new NativeArray<Matrix4x4>(numParticles, Allocator.Persistent);

      for (int i = 0; i < numParticles; i++) {
        particlesCurrent[i] = Random.insideUnitSphere + (Vector3.one * 0.5f);
        particlesPrevious[i] = particlesCurrent[i] + (Random.insideUnitSphere * 0.01f);
      }
    }

    public void Dispose() {
      particlesCurrent.Dispose();
      particlesPrevious.Dispose();
      particleMatrices.Dispose();
    }
  }

  [BurstCompile]
  public struct ParticleJob : IJobParallelFor {
    [ReadOnly]
    public VectorFieldStruct field;
    public ParticleData data;
    public ParticleJob(ParticleData data, VectorFieldStruct field) {
      this.data = data;
      this.field = field;
    }

    public void Execute(int i) {
      //Sample the Vector Field
      data.particlesCurrent[i] += field.trilinearSample(data.particlesCurrent[i]) * 0.1f;

      //Integrate the Particle via Verlet Integration
      Vector3 temp = data.particlesCurrent[i];
      data.particlesCurrent[i] += (data.particlesCurrent[i] - data.particlesPrevious[i]);
      data.particlesPrevious[i] = temp;

      //Write the particle positions out to transformation matrices
      data.particleMatrices[i] = Matrix4x4.TRS(data.particlesCurrent[i], new Quaternion(0f,0f,0f,1f), new Vector3(1f,1f,1f) * 0.01f);
    }
  }

  void Start() {
    if (!vectorField.hasStarted) { vectorField.Start(); }
    particleData = new ParticleData(numParticles);
    particles = new Matrix4x4[numParticles];
  }

  void Update() {
    new ParticleJob(particleData, vectorField.field).Schedule(particles.Length, 2048).Complete();
    particleData.particleMatrices.CopyTo(particles);
    
    //Draw the particles 1023 at a time (since DrawMeshInstanced can't do any more at once)
    int particlesDrawn = 0;
    while (particlesDrawn < numParticles) {
      int particlesToDraw = Mathf.Min(1023, numParticles - particlesDrawn);
      //SLICES SEEM TO BE BROKEN; use slow system copy instead
      //NativeSlice<Matrix4x4> slice = new NativeSlice<Matrix4x4>(particleData.particleMatrices, particlesDrawn, particlesToDraw);
      //slice.CopyTo(instanceMatrices); //or //instanceMatrices = slice.toArray();
      System.Array.Copy(particles, particlesDrawn, instanceMatrices, 0, particlesToDraw);
      Graphics.DrawMeshInstanced(sphereMesh, 0, sphereMaterial, instanceMatrices);
      particlesDrawn += particlesToDraw;
    }
  }

  void OnDestroy() {
    particleData.Dispose();
  }
}
