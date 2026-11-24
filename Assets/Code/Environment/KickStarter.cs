using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class KickStarter : MonoBehaviour, IDamageable
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private int playerBoostForce;
    [SerializeField] private ParticleSystem kickStart;
    [SerializeField] private Transform transformBase;

    private void Awake()
    {
    }

    public void TakeDamage(float damage)
    {
        ImpactEffectPool.Instance.Play(kickStart, transformBase.position, transformBase.rotation);
        gameObject.SetActive(false);
        Invoke("AddBoost", 0.1f);
        Invoke("Activate", 5);
    }

    private void AddBoost()
    {
        rb.AddForce(Vector3.up * playerBoostForce, ForceMode.VelocityChange);
    }


    private void Activate()
    {
        gameObject.SetActive(true);
    }
}
