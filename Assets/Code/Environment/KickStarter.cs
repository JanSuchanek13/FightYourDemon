using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class KickStarter : MonoBehaviour, IDamageable
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private SphereCollider collider;
    [SerializeField] private GameObject vfx;
    [SerializeField] private FallDeathReload fallDeathReload;
    [SerializeField] private int playerBoostForce;
    [SerializeField] private ParticleSystem kickStart;
    [SerializeField] private Transform transformBase;

    private bool beActive;

    private void Awake()
    {
    }

    private void Update()
    {
        if(beActive && fallDeathReload.isGrounded)
        {
            Activate();
        }
    }

    public void TakeDamage(float damage)
    {
        ImpactEffectPool.Instance.Play(kickStart, transformBase.position, transformBase.rotation);
        //gameObject.SetActive(false);
        collider.enabled = false;
        vfx.SetActive(false);
        Invoke("AddBoost", 0.1f);
        Invoke("BeActive", 5);
    }

    private void AddBoost()
    {
        rb.AddForce(Vector3.up * playerBoostForce, ForceMode.VelocityChange);
    }


    private void BeActive()
    {
        beActive = true;
    }

    private void Activate()
    {
        collider.enabled = true;
        vfx.SetActive(true);
        beActive = false;
    }
}
