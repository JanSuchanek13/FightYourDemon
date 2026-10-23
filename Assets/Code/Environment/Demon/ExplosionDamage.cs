using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    [Header("Explosion")]
    [Tooltip("Radius, in dem Gegner getroffen werden")]
    [SerializeField] private float radius = 5f;
    [Tooltip("Schaden im Zentrum")]
    [SerializeField] private float maxDamage = 50f;
    [Tooltip("Schaden faellt zum Rand hin ab")]
    [SerializeField] private bool useFalloff = false;
    [Tooltip("Nur diese Layer werden getroffen (Gegner-Layer)")]
    [SerializeField] private LayerMask targetLayers = ~0;

    // an der Explosionsstelle aufrufen
    public void Explode(Vector3 center)
    {
        Collider[] hits = Physics.OverlapSphere(center, radius, targetLayers);

        foreach (var col in hits)
        {
            if (col.TryGetComponent(out IDamageable damageable))
            {
                float damage = maxDamage;

                if (useFalloff)
                {
                    float dist = Vector3.Distance(center, col.transform.position);
                    float t = Mathf.Clamp01(1f - dist / radius); // 1 im Zentrum, 0 am Rand
                    damage = maxDamage * t;
                }

                damageable.TakeDamage(damage);
            }
        }
    }

    // zum Sehen des Radius im Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}