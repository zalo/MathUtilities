using UnityEngine;

public class VectorField : MonoBehaviour {
  Vector3[,,] field;
  public Vector3 dimension = Vector3.one;
  public int resolution = 10;

  void Start() {
    field = new Vector3[resolution, resolution, resolution];
    for (int i = 0; i < field.GetLength(0); i++) {
      for (int j = 0; j < field.GetLength(1); j++) {
        for (int k = 0; k < field.GetLength(2); k++) {
          Vector3 coordinate = new Vector3(
            dimension.x * ((float)i / ((float)field.GetLength(0) - 1)),
            dimension.y * ((float)j / ((float)field.GetLength(1) - 1)),
            dimension.z * ((float)k / ((float)field.GetLength(2) - 1)));

          Vector3 orthogonal = Vector3.Cross(Vector3.up, coordinate - (Vector3.one * 0.5f));
          field[i, j, k] = (((orthogonal.normalized) * (1f / orthogonal.magnitude)) * 0.001f) - ((coordinate - (Vector3.one * 0.5f)) * 0.1f);
        }
      }
    }
  }

  public void OnDrawGizmos() {
    if (field == null) { Start(); }
    for (int i = 0; i < field.GetLength(0); i++) {
      for (int j = 0; j < field.GetLength(1); j++) {
        for (int k = 0; k < field.GetLength(2); k++) {
          Vector3 coordinate = new Vector3(
            dimension.x * ((float)i / ((float)field.GetLength(0) - 1)),
            dimension.y * ((float)j / ((float)field.GetLength(1) - 1)),
            dimension.z * ((float)k / ((float)field.GetLength(2) - 1)));

          Gizmos.DrawLine(transform.TransformPoint(coordinate), transform.TransformPoint(coordinate + field[i, j, k]));
        }
      }
    }
  }


  public Vector3 trilinearSample(Vector3 position) {
    position = transform.InverseTransformPoint(position);

    Vector3 indexPosition = Vector3.Scale(new Vector3(
      position.x / dimension.x,
      position.y / dimension.y,
      position.z / dimension.z), new Vector3(
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
    Vector3 x1 = Vector3.LerpUnclamped(field[x, y, z], field[x + 1, y, z], xalpha);
    Vector3 x2 = Vector3.LerpUnclamped(field[x, y + 1, z], field[x + 1, y + 1, z], xalpha);
    Vector3 x3 = Vector3.LerpUnclamped(field[x, y, z + 1], field[x + 1, y, z + 1], xalpha);
    Vector3 x4 = Vector3.LerpUnclamped(field[x, y + 1, z + 1], field[x + 1, y + 1, z + 1], xalpha);

    //2 Lerps
    Vector3 y1 = Vector3.LerpUnclamped(x1, x2, yalpha);
    Vector3 y2 = Vector3.LerpUnclamped(x3, x4, yalpha);

    //1 Lerp
    return transform.TransformDirection(Vector3.LerpUnclamped(y1, y2, zalpha));
  }
}