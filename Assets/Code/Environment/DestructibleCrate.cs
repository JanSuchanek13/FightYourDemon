using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DestructibleCrate : MonoBehaviour, IDamageable
{
    public enum demonClass {Demon, MoneyDemon, ArcaneDemon, BulletDemon, OrbitDemon}
    public demonClass _demonClass;

    public float health;
    public float maxhealth = 30f;
    public float playerBoostForce = 7f;
    public int moneyValue = 1;
    public int bulletValue = 0;
    private float stage1health;
    public float stage2healthMulti = 3.3f;
    public float stage3healthMulti = 6.6f;
    public float stage4healthMulti = 6.6f;
    private float stage1BoostForce;
    public float stage2BoostForceMulti = 0.9f;
    public float stage3BoostForceMulti = 0.7f;
    public float stage4BoostForceMulti = 0.7f;
    private int stage1money;
    public int stage2money = 20;
    public int stage3money = 50;
    public int stage4money = 50;
    private float currentstage = 1;
    private bool isbreaking;
    private float normalScale;
    public Material m_damage0;
    public Material m_damage;
    public Material m_damage1;
    public Material m_demon; //unnötig???
    public Material m_stage1;
    public Material m_stage2;
    public Material m_stage3;
    public Material m_stage4;
    public Material[] currentlyAssignedMaterials;
    [SerializeField] private ParticleSystem killPuffPrefab;
    [SerializeField] private ParticleSystem killPerfectPuffPrefab;
    [SerializeField] private ParticleSystem killLootPrefab;

    private MeshRenderer renderer;
    private Collider collider;


    [Header("Horn")]
    private List<Vector3> hornListPos = new List<Vector3>();
    private List<Quaternion> hornListRot = new List<Quaternion>();

    private void Awake()
    {
        renderer = this.transform.GetChild(0).GetComponent<MeshRenderer>();
        collider = this.GetComponent<Collider>();
        normalScale = gameObject.transform.localScale.x;

        stage1health = maxhealth;
        stage1BoostForce = playerBoostForce;
        stage1money = moneyValue;
        health = maxhealth;
        currentlyAssignedMaterials = renderer.materials;
        //currentlyAssignedMaterials[0] = null; //Schaden ausschalten

        for (int i = 1; i < transform.childCount; i++) //get list of all horns
        {
            hornListPos.Add(this.transform.GetChild(i).localPosition);
            hornListRot.Add(this.transform.GetChild(i).transform.rotation);
        }


    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        this.transform.GetChild(0).GetComponent<HitFlash>().Flash();

        if (health < maxhealth*0.9)
        {
            renderer.material = m_damage;
            if (health <= stage1health/3)
            {
                renderer.material = m_damage1;
            }
        }

        if (health <= 0)
            Break();

        //Juice bei Schaden
        float scale = (1 - (Mathf.Clamp(health, 0, maxhealth) / maxhealth)) *0.1f + normalScale;
        this.transform.localScale = new Vector3(scale, scale, scale);

        if(_demonClass != demonClass.OrbitDemon)
        {
            StartCoroutine(ShakeCoroutine(1, 0.1f));
        }
    }

    void Break()
    {
        if (!isbreaking) //checkt ob die Funktion bereits aufgerufen wurde (Shootgun Problem > wenn zu viele Kugeln fliegen, dann ruft es die Break Funktion mehrfach auf)
        {
            isbreaking = true;
            this.transform.GetChild(0).GetComponent<HitFlash>().Flash();
            ComboCounter.Instance.AddKill();
            Invoke("DisableCollider", 0.01f); //Collider aus damit man Gegner dahinter Treffen kann aber mit kleiner Verzögerung um alle Shotgun-Treffer Damage-Zahlen anzuzeigen

            //Check for perfekt Kill
            int activeHorns = 0;
            for (int i = 1; i < transform.childCount; i++)
            {
                if (this.transform.GetChild(i).GetComponent<Rigidbody>().isKinematic == true)
                {
                    activeHorns += 1;
                }
            }

            if (activeHorns == 0)
            {
                Invoke("BreakPerfect", 0.2f);
            }
            else
            {
                Invoke("Break2", 0.2f);
            }
        }
        
    }

    private void DisableCollider()
    {
        collider.enabled = false;
    }

    void Break2()
    {
        if(_demonClass != demonClass.ArcaneDemon)
        {
            ImpactEffectPool.Instance.Play(killLootPrefab, transform.position, transform.rotation);
        }
        ImpactEffectPool.Instance.Play(killPuffPrefab, transform.position, transform.rotation);
        WeaponController.Instance.currentmaxAmmo += bulletValue;
        MoneyManager.Instance.AddMoney(moneyValue);
        RisingEnvironment.Instance.BoostPlayer(playerBoostForce, transform.position);
        //Destroy(gameObject);
        this.gameObject.active = false;
        renderer.material = null;
        health = maxhealth;
        isbreaking = false;
    }

    void BreakPerfect()
    {
        if (_demonClass != demonClass.ArcaneDemon)
        {
            ImpactEffectPool.Instance.Play(killLootPrefab, transform.position, transform.rotation);
        }
        ImpactEffectPool.Instance.Play(killPerfectPuffPrefab, transform.position, transform.rotation);
        WeaponController.Instance.currentmaxAmmo += bulletValue;
        MoneyManager.Instance.AddMoney(moneyValue);
        RisingEnvironment.Instance.BoostPlayer(playerBoostForce, transform.position);
        //Destroy(gameObject);
        this.gameObject.active = false;
        renderer.material = null;
        health = maxhealth;
        isbreaking = false;
    }

    public void ResetCrate(float newstage)
    {
        int randomChange = Random.Range(0, 2);
        if (randomChange == 1 && newstage>currentstage)
        {
            newstage = currentstage;
        }

        this.transform.localScale = new Vector3(normalScale, normalScale, normalScale);
        collider.enabled = true;

        if (_demonClass == demonClass.Demon) //nur bei normalen demonen die Farbe ändern
        {
            if (currentstage != newstage) //Nur wenn stage neu ist soll sich etwas ändern
            {
                switch (newstage)
                {
                    case 1:
                        currentlyAssignedMaterials[1] = m_stage1; //Einzelnes Material wechseln
                        renderer.materials = currentlyAssignedMaterials;
                        break;
                    case 2:
                        currentlyAssignedMaterials[1] = m_stage2; //Einzelnes Material wechseln
                        renderer.materials = currentlyAssignedMaterials;
                        break;
                    case 3:
                        currentlyAssignedMaterials[1] = m_stage3; //Einzelnes Material wechseln
                        renderer.materials = currentlyAssignedMaterials;
                        break;
                    case 4:
                        currentlyAssignedMaterials[1] = m_stage4; //Einzelnes Material wechseln
                        renderer.materials = currentlyAssignedMaterials;
                        break;
                    default:
                        renderer.materials = currentlyAssignedMaterials;
                        break;
                }
            }
            else
            {
                renderer.materials = currentlyAssignedMaterials;//unnötig???
                health = maxhealth;
            }

        }
        else
        {
            renderer.materials = currentlyAssignedMaterials;
            health = maxhealth;
        }

        //Anpassung der Health, Money und PlayerBoostForce
        if (currentstage != newstage) //Nur wenn stage neu ist soll sich etwas ändern
        {
            switch (newstage)
            {
                case 1:
                    currentstage = newstage;
                    health = stage1health;
                    moneyValue = stage1money;
                    playerBoostForce = stage1BoostForce;
                    break;
                case 2:
                    currentstage = newstage;
                    health = stage1health * stage2healthMulti;
                    moneyValue = stage1money * stage2money;
                    playerBoostForce = stage1BoostForce * stage2BoostForceMulti;
                    break;
                case 3:
                    currentstage = newstage;
                    health = stage1health * stage3healthMulti;
                    moneyValue = stage1money * stage3money;
                    playerBoostForce = stage1BoostForce * stage3BoostForceMulti;
                    break;
                case 4:
                    currentstage = newstage;
                    health = stage1health * stage4healthMulti;
                    moneyValue = stage1money * stage4money;
                    playerBoostForce = stage1BoostForce * stage4BoostForceMulti;
                    break;
                default:
                    currentstage = newstage;
                    health = maxhealth;
                    break;
            }
        }
        else
        {
            health = maxhealth;
        }

        maxhealth = health; //set the new MaxHealth

        //Reset Horns
        for (int i = 1; i < transform.childCount; i++)
        {
            this.transform.GetChild(i).localPosition = hornListPos[i-1];
            this.transform.GetChild(i).transform.rotation = hornListRot[i - 1];
            this.transform.GetChild(i).GetComponent<Rigidbody>().isKinematic = true;
        }

        this.transform.GetChild(0).GetComponent<HitFlash>().Flash();
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
