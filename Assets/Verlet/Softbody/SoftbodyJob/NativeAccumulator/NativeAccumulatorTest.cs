using Unity.Collections;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

public class NativeAccumulatorTest : MonoBehaviour {

  private void Start() {
    JobsUtility.JobDebuggerEnabled = false;
  }

  [ContextMenu("try it")]
  unsafe void Update() {
    var floatCounter = new NativeAccumulator<float, Addition>(Allocator.Temp);

    new SumFloatsJob() {
      counter = floatCounter
    }.Schedule(100000, 1024).Complete();

    Debug.Log("Sum: " + floatCounter.Value);

    floatCounter.Dispose();
  }
}

public struct SumFloatsJob : IJobParallelFor {
  public NativeAccumulator<float, Addition>.Concurrent counter;

  public void Execute(int index) {
    counter.Accumulate(0.1f);
  }
}

public struct Addition : IAccumulator<float> {
  public void Accumulate(ref float existing, float value) {
    existing += value;
  }
}
