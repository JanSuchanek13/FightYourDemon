using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 20f;
    public float lifeTime = 5f;

    void Start()
    {
        //Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
