using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;

public interface IAccumulator<T> {
  void Accumulate(ref T existing, T value);
}

[NativeContainer]
public unsafe struct NativeAccumulator<T, K>
  where T : struct
  where K : struct, IAccumulator<T> {

  [NativeDisableUnsafePtrRestriction]
  private void* _buffer;
  private Allocator _allocator;

  private K _accumulator;
  private T _defaultT;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
  AtomicSafetyHandle _safety;

  [NativeSetClassTypeToNullOnSchedule]
  DisposeSentinel _disposeSentinel;
#endif

  public NativeAccumulator(Allocator allocator, T defaultValue = default(T)) {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
    if (!UnsafeUtility.IsBlittable<int>()) {
      throw new ArgumentException();
    }
#endif

    _allocator = allocator;

    _buffer = UnsafeUtility.Malloc(JobsUtility.CacheLineSize * JobsUtility.MaxJobThreadCount,
                                   JobsUtility.CacheLineSize,
                                   _allocator);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
    DisposeSentinel.Create(out _safety, out _disposeSentinel, 0);
#endif

    _accumulator = default(K);
    _defaultT = defaultValue;

    Value = defaultValue;
  }

  public T Value {
    get {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      AtomicSafetyHandle.CheckReadAndThrow(_safety);
#endif

      T value = UnsafeUtility.ReadArrayElement<T>(_buffer, 0);
      for (int i = 1; i < JobsUtility.MaxJobThreadCount; i++) {
        _accumulator.Accumulate(ref value, UnsafeUtility.ReadArrayElementWithStride<T>(_buffer, i, JobsUtility.CacheLineSize));
      }

      return value;
    }
    set {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      AtomicSafetyHandle.CheckWriteAndThrow(_safety);
#endif

      UnsafeUtility.WriteArrayElementWithStride<T>(_buffer, 0, JobsUtility.CacheLineSize, value);
      for (int i = 1; i < JobsUtility.MaxJobThreadCount; i++) {
        UnsafeUtility.WriteArrayElementWithStride<T>(_buffer, i, JobsUtility.CacheLineSize, _defaultT);
      }
    }
  }

  public void Accumulate(T value) {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
    AtomicSafetyHandle.CheckWriteAndThrow(_safety);
#endif

    T curr = UnsafeUtility.ReadArrayElementWithStride<T>(_buffer, 0, JobsUtility.CacheLineSize);
    _accumulator.Accumulate(ref curr, value);
    UnsafeUtility.WriteArrayElementWithStride(_buffer, 0, JobsUtility.CacheLineSize, curr);
  }

  public bool IsCreated {
    get {
      return _buffer != null;
    }
  }

  public void Dispose() {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
    DisposeSentinel.Dispose(_safety, ref _disposeSentinel);
#endif

    UnsafeUtility.Free(_buffer, _allocator);
    _buffer = null;
  }

  [NativeContainer]
  [NativeContainerIsAtomicWriteOnly]
  public struct Concurrent {

    [NativeDisableUnsafePtrRestriction]
    private void* _buffer;

    [NativeSetThreadIndex]
    private int _threadIndex;

    private K _accumulator;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
    AtomicSafetyHandle m_Safety;
#endif

    public static implicit operator Concurrent(NativeAccumulator<T, K> accumulator) {
      Concurrent concurrent;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      AtomicSafetyHandle.CheckWriteAndThrow(accumulator._safety);
      concurrent.m_Safety = accumulator._safety;
      AtomicSafetyHandle.UseSecondaryVersion(ref concurrent.m_Safety);
#endif

      concurrent._buffer = accumulator._buffer;
      concurrent._threadIndex = 0;
      concurrent._accumulator = accumulator._accumulator;
      return concurrent;
    }

    public void Accumulate(T t) {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif

      T curr = UnsafeUtility.ReadArrayElementWithStride<T>(_buffer, _threadIndex, JobsUtility.CacheLineSize);
      _accumulator.Accumulate(ref curr, t);
      UnsafeUtility.WriteArrayElementWithStride(_buffer, _threadIndex, JobsUtility.CacheLineSize, curr);
    }
  }
}
