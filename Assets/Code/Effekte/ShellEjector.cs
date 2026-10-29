using UnityEngine;

public class ShellEjector : MonoBehaviour
{
    [SerializeField] private ParticleSystem shellParticles;
    [Tooltip("Wie viele Huelsen pro Schuss (normal 1)")]
    [SerializeField] private int shellsPerShot = 1;

    public void Eject()
    {
        if (shellParticles != null)
            shellParticles.Emit(shellsPerShot);
    }
}