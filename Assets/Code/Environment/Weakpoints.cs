using UnityEngine;

public class Weakpoints : MonoBehaviour, IDamageable
{
    public float health;
    public float maxhealth = 1f;
    public float playerBoostForce = 0f;
    public float weakPointDamage;
    public bool isPoping;

    private GameObject player;

    private void Awake()
    {
        health = maxhealth;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void TakeDamage(float damage)
    {
        health -= damage;


        if (health <= 0)
            Break();
    }

    void Break()
    {
        BoostPlayer();
        this.transform.parent.GetComponent<BossShooting>().TakeDamage(weakPointDamage);
        if (isPoping)
        {
            this.gameObject.active = false;
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }


    public void BoostPlayer()
    {
        if (player.TryGetComponent(out Rigidbody rb))
        {
            // nur ein Runterfallen abfangen, vorhandenen Aufwaertsschwung behalten
            if (rb.linearVelocity.y < 0f)
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            rb.AddForce(Vector3.up * playerBoostForce, ForceMode.Impulse);
        }
    }
}
