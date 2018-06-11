using UnityEngine;
using Unity.Collections;

public class VectorField : MonoBehaviour {
  public VectorFieldStruct field;
  public Vector3 dimension = Vector3.one;
  public int resolution = 10;
  [System.NonSerialized]
  public bool hasStarted = false;

  Vector3 ringField(Vector3 coordinate) {
    Vector3 orthogonal = Vector3.Cross(Vector3.up, coordinate - (Vector3.one * 0.5f));
    return (((orthogonal.normalized) * (1f / orthogonal.magnitude)) * 0.001f) - ((coordinate - (Vector3.one * 0.5f)) * 0.1f);
  }

  public void Start() {
    if (!hasStarted) {
      field = new VectorFieldStruct(dimension, transform.localToWorldMatrix, transform.worldToLocalMatrix, resolution, ringField);
      hasStarted = true;
    }
  }

  public void Update() {
    if (!hasStarted) { return; }
    field.localToWorld = transform.localToWorldMatrix;
    field.worldToLocal = transform.worldToLocalMatrix;
  }

  public void OnDrawGizmos() {
    if (!hasStarted) { return; }
    for (int i = 0; i < field.field.GetLength(0); i++) {
      for (int j = 0; j < field.field.GetLength(1); j++) {
        for (int k = 0; k < field.field.GetLength(2); k++) {
          Vector3 coordinate = new Vector3(
            dimension.x * (i / ((float)field.field.GetLength(0) - 1)),
            dimension.y * (j / ((float)field.field.GetLength(1) - 1)),
            dimension.z * (k / ((float)field.field.GetLength(2) - 1)));

          Gizmos.DrawLine(transform.TransformPoint(coordinate), transform.TransformPoint(coordinate + field.field.Get(i, j, k)));
        }
      }
    }
  }

  public Vector3 trilinearSample(Vector3 position) {
    return field.trilinearSample(position);
  }

  private void OnDestroy() {
    field.Dispose();
  }
}

public struct VectorFieldStruct : System.IDisposable {
  public Native3DArray<Vector3> field;
  public Vector3 dimension;
  public Matrix4x4 worldToLocal;
  public Matrix4x4 localToWorld;

  public VectorFieldStruct(Vector3 dimension, Matrix4x4 localToWorld, Matrix4x4 worldToLocal, int resolution, System.Func<Vector3, Vector3> fieldFunction) {
    this.dimension = dimension;
    this.localToWorld = localToWorld;
    this.worldToLocal = worldToLocal;
    field = new Native3DArray<Vector3>(resolution, resolution, resolution);
    for (int i = 0; i < field.GetLength(0); i++) {
      for (int j = 0; j < field.GetLength(1); j++) {
        for (int k = 0; k < field.GetLength(2); k++) {
          Vector3 coordinate = new Vector3(
            dimension.x * (i / ((float)field.GetLength(0) - 1)),
            dimension.y * (j / ((float)field.GetLength(1) - 1)),
            dimension.z * (k / ((float)field.GetLength(2) - 1)));

          field.Set(i, j, k, fieldFunction(coordinate));
        }
      }
    }
  }

  public Vector3 trilinearSample(Vector3 position) {
    Vector3 localPosition = worldToLocal.MultiplyPoint3x4(position);
    Vector3 indexPosition = Vector3.Scale(new Vector3(
      localPosition.x / dimension.x,
      localPosition.y / dimension.y,
      localPosition.z / dimension.z), new Vector3(
        field.GetLength(0) - 1,
        field.GetLength(1) - 1,
        field.GetLength(2) - 1));

    int x = Mathf.Clamp(Mathf.FloorToInt(indexPosition.x), 0, field.GetLength(0) - 2);
    int y = Mathf.Clamp(Mathf.FloorToInt(indexPosition.y), 0, field.GetLength(1) - 2);
    int z = Mathf.Clamp(Mathf.FloorToInt(indexPosition.z), 0, field.GetLength(2) - 2);

    float xalpha = indexPosition.x - x;
    float yalpha = indexPosition.y - y;
    float zalpha = indexPosition.z - z;

    //4 Lerps
    Vector3 x1 = Vector3.LerpUnclamped(field.Get(x, y, z), field.Get(x + 1, y, z), xalpha);
    Vector3 x2 = Vector3.LerpUnclamped(field.Get(x, y + 1, z), field.Get(x + 1, y + 1, z), xalpha);
    Vector3 x3 = Vector3.LerpUnclamped(field.Get(x, y, z + 1), field.Get(x + 1, y, z + 1), xalpha);
    Vector3 x4 = Vector3.LerpUnclamped(field.Get(x, y + 1, z + 1), field.Get(x + 1, y + 1, z + 1), xalpha);

    //2 Lerps
    Vector3 y1 = Vector3.LerpUnclamped(x1, x2, yalpha);
    Vector3 y2 = Vector3.LerpUnclamped(x3, x4, yalpha);

    //1 Lerp
    return localToWorld.MultiplyVector(Vector3.LerpUnclamped(y1, y2, zalpha));
  }

  public void Dispose() {
    field.Dispose();
  }
}

public struct Native3DArray<T> : System.IDisposable where T : struct {
  public int height, width, depth;
  public NativeArray<T> nativeArray;
  public Native3DArray(int height, int width, int depth) {
    this.height = height; this.width = width; this.depth = depth;
    nativeArray = new NativeArray<T>(height * width * depth, Allocator.Persistent);
  }
  public T Get(int x, int y, int z) {
    return nativeArray[x + height * (y + width * z)];
  }
  public void Set(int x, int y, int z, T it) {
    nativeArray[x + height * (y + width * z)] = it;
  }
  public int GetLength(int dim) {
    return (dim == 0 ? height : (dim == 1 ? width : depth));
  }
  public void Dispose() {
    nativeArray.Dispose();
  }
}
