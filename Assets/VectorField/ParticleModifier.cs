using UnityEngine;
public abstract class ParticleModifier : MonoBehaviour {
  public abstract void ModifierOffset(int index, ref Vector3 particlePosition, ref Vector3 previousParticlePosition, bool reset = false);
}
