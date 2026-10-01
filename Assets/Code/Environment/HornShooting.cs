using UnityEngine;

public class HornShooting : MonoBehaviour, IDamageable
{
    public float health;
    public float maxhealth = 1f;
    public float playerBoostForce = 0f;
    public int hornValue = 1;
    public int stage = 1;
    public float stage1health = 30f;
    public float stage2health = 100f;

    private GameObject player;

    private void Awake()
    {
        health = maxhealth;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health < maxhealth)
        {

        }

        if (health <= 0)
            Break();
    }

    void Break()
    {
        MoneyManager.Instance.AddHorns(hornValue);
        BoostPlayer();
        health = maxhealth;
        GetComponent<Rigidbody>().isKinematic = false;
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
