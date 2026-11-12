using UnityEngine;

public class ImpactFrameCaller : MonoBehaviour
{
    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem impactFrame;

    [Header("Player")]
    [SerializeField] private Transform player;

    public void PlayImpactFrame()
    {
        ImpactEffectPool.Instance.Play(impactFrame, player.position, Quaternion.Euler(Vector3.zero));
    }
}
