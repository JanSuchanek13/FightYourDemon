using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class KickStarter : MonoBehaviour, IDamageable
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private int playerBoostForce;

    private void Awake()
    {
    }

    public void TakeDamage(float damage)
    {
        rb.AddForce(Vector3.up * playerBoostForce, ForceMode.VelocityChange);
        gameObject.SetActive(false);
        Invoke("Activate", 5);

    }

    private void Activate()
    {
        gameObject.SetActive(true);
    }
}
