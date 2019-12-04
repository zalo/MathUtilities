using UnityEngine;

/// <summary>
/// A Minkowski Difference renderer based on Aras's examples
/// https://forum.unity3d.com/threads/compute-shaders.148874/#post-1021130
/// </summary>

[ExecuteInEditMode]
public class DrawMinkowski : MonoBehaviour {
  public RectTransform ShapeOne;
  public Texture2D sprite;
  [Range(0.02f, 0.5f)]
  public float size = 0.1f;
  public Color color = new Color(1.0f, 0.6f, 0.3f, 0.03f);
  public Shader shader;
  private Material mat;
  private ComputeBuffer cbDrawArgs;
  private ComputeBuffer cbPoints;
  private Vector3[] corners = new Vector3[4];

  private void CreateResources() {
    if (cbDrawArgs == null) {
      cbDrawArgs = new ComputeBuffer(1, 16, ComputeBufferType.IndirectArguments);
      var args = new int[4];
      args[0] = 0;
      args[1] = 1;
      args[2] = 0;
      args[3] = 0;
      cbDrawArgs.SetData(args);
    }
    if (cbPoints == null) {
      cbPoints = new ComputeBuffer(10000, 8, ComputeBufferType.Append);
    }
    if (mat == null) {
      mat = new Material(shader);
      mat.hideFlags = HideFlags.HideAndDontSave;
    }
  }

  private void ReleaseResources() {
    if (cbDrawArgs != null) cbDrawArgs.Release(); cbDrawArgs = null;
    if (cbPoints != null) cbPoints.Release(); cbPoints = null;
    DestroyImmediate(mat);
  }

  void OnDisable() {
    ReleaseResources();
  }

  void OnRenderImage(RenderTexture src, RenderTexture dst) {
    if (!shader)
      return;
    if (!SystemInfo.supportsComputeShaders) {
      Graphics.Blit(src, dst);
      Debug.LogWarning("Compute shaders not supported (not using DX11?)");
      return;
    }
    CreateResources();

    mat.SetTexture("_Sprite", ShapeOne.GetComponent<SpriteRenderer>().sprite.texture);
    mat.SetFloat("_Size", size);
    mat.SetColor("_Color", color);

    ShapeOne.GetWorldCorners(corners);
    //HACK: Was getting strange errors with SetVectorArray in 5.5.1f1...
    mat.SetVector("_Corner1", new Vector4(corners[0].x * Screen.height / Screen.width, corners[0].y, 0f, 0f));
    mat.SetVector("_Corner2", new Vector4(corners[1].x * Screen.height / Screen.width, corners[1].y, 0f, 0f));
    mat.SetVector("_Corner3", new Vector4(corners[2].x * Screen.height / Screen.width, corners[2].y, 0f, 0f));
    mat.SetVector("_Corner4", new Vector4(corners[3].x * Screen.height / Screen.width, corners[3].y, 0f, 0f));

    Graphics.SetRandomWriteTarget(1, cbPoints);
    Graphics.Blit(src, dst, mat, 0);

    Graphics.ClearRandomWriteTargets();
    Graphics.SetRenderTarget(dst);

    ComputeBuffer.CopyCount(cbPoints, cbDrawArgs, 0);
    mat.SetBuffer("pointBuffer", cbPoints);
    mat.SetPass(1);
    Graphics.DrawProceduralIndirectNow(MeshTopology.Points, cbDrawArgs, 0);
  }
}
