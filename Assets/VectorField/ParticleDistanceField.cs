using UnityEngine;

public class ParticleDistanceField : ParticleModifier {
  public float falloffDistance = 0.5f;
  float[] previousDistances = new float[2000];
  public void Start() {
    for(int i = 0; i < previousDistances.Length; i++) previousDistances[i] = 0f;
  }

  public override void ModifierOffset(int index, ref Vector3 position, ref Vector3 previousParticlePosition, bool reset = false) {
    if (index >= previousDistances.Length) { previousDistances = new float[previousDistances.Length * 2]; }

    float distance = signedDistance(position);
    float scaling = Mathf.Clamp01(Mathf.Pow(Mathf.InverseLerp(falloffDistance, 0f, distance), 4f));
    Vector3 offset = (previousDistances[index] - distance) * (position - transform.position).normalized * scaling;

    position += offset + Vector3.right * 0.001f;
    previousDistances[index] = Mathf.Max(0f, signedDistance(position));
    previousParticlePosition = distance < 0f || reset ? position : previousParticlePosition;
  }

  float signedDistance(Vector3 position) {
    return Vector3.Distance(position, transform.position) - (transform.localScale.x * 0.5f);
  }
}
