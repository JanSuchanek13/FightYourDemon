using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BossShooting : MonoBehaviour, IDamageable
{
    public enum demonClass { Ass }
    public demonClass _demonClass;

    public float health;
    public float maxhealth = 30f;
    public float playerBoostForce = 15f;
    public int moneyValue = 1;
    public int bulletValue = 0;
    private bool isbreaking;
    public Material m_damage0;
    public Material m_damage;
    public Material m_damage1;
    public Material m_demon; //unnötig???
    public Material m_red;
    [SerializeField] private ParticleSystem killPuffPrefab;
    [SerializeField] private ParticleSystem killLootPrefab;


    [Header("WeakPoints")]
    private List<GameObject> weakpointsList = new List<GameObject>();

    private void Awake()
    {

        health = maxhealth;
        //currentlyAssignedMaterials[0] = null; //Schaden ausschalten

        for (int i = 1; i < transform.childCount; i++) //get list of all horns
        {
            weakpointsList.Add(this.transform.GetChild(i).gameObject);
        }


    }

    public void TakeDamage(float damage)
    {
        
        health -= damage;
        foreach (Transform child in transform.GetChild(0))
        {
            child.GetComponent<HitFlash>().Flash();
        }
        

        if (health < maxhealth * 0.9)
        {
            foreach (Transform child in transform.GetChild(0))
            {
                child.GetComponent<MeshRenderer>().material = m_damage;
                if (health <= maxhealth / 3)
                {
                    child.GetComponent<MeshRenderer>().material = m_damage1;
                }
            }
            
        }

        if (health <= 0)
            Break();

        //Juice bei Schaden
        float scale = (1 - (Mathf.Clamp(health, 0, maxhealth) / maxhealth)) * 0.1f + 20;
        this.transform.localScale = new Vector3(scale, scale, scale);
        StartCoroutine(ShakeCoroutine(1, 0.1f));
    }

    void Break()
    {
        if (!isbreaking) //checkt ob die Funktion bereits aufgerufen wurde (Shootgun Problem > wenn zu viele Kugeln fliegen, dann ruft es die Break Funktion mehrfach auf)
        {
            isbreaking = true;
            foreach (Transform child in transform.GetChild(0))
            {
                child.GetComponent<HitFlash>().Flash();
            }
            Invoke("Break2", 0.1f);
        }

    }

    void Break2()
    {
        if (_demonClass == demonClass.Ass)
        {
            ImpactEffectPool.Instance.Play(killLootPrefab, transform.position, transform.rotation);
        }
        ImpactEffectPool.Instance.Play(killPuffPrefab, transform.position, transform.rotation);
        WeaponController.Instance.currentmaxAmmo += bulletValue;
        MoneyManager.Instance.AddMoney(moneyValue);
        RisingEnvironment.Instance.BoostPlayer(playerBoostForce, transform.position);
        //Destroy(gameObject);
        this.gameObject.active = false;
        RisingEnvironment.Instance.bossCount += 1;
        isbreaking = false;
    }


    private IEnumerator ShakeCoroutine(float duration, float intensity)
    {
        Vector3 originalPosition = transform.localPosition;
        Quaternion originalRotation = transform.localRotation;

        float elapsed = 0f;

        // Zufälliger Startpunkt für den Perlin Noise
        float seedX = Random.Range(0f, 1000f);
        float seedY = Random.Range(0f, 1000f);
        float seedZ = Random.Range(0f, 1000f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Perlin Noise erzeugt weiche, zufällige Bewegungen
            float x = (Mathf.PerlinNoise(seedX, elapsed * 10f) - 0.5f) * 2f;
            float y = (Mathf.PerlinNoise(seedY, elapsed * 10f) - 0.5f) * 2f;
            float z = (Mathf.PerlinNoise(seedZ, elapsed * 10f) - 0.5f) * 2f;

            // Position wackeln lassen
            transform.localPosition = originalPosition +
                                      new Vector3(x, y, z) * intensity;

            // Rotation wackeln lassen
            transform.localRotation = originalRotation *
                                      Quaternion.Euler(
                                          x * intensity * 10f,
                                          y * intensity * 10f,
                                          z * intensity * 10f
                                      );

            yield return null;
        }

        // Am Ende exakt auf Ausgangszustand zurücksetzen
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
    }
}
