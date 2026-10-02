using UnityEngine;
using TMPro;

public class WeaponController : MonoBehaviour
{

    public static WeaponController Instance;

    public WeaponData weaponData;
    public Transform firePoint;
    public Camera playerCamera;
    [SerializeField] private DamageNumberManager damageNumbers;
    [SerializeField] private WeaponRecoil recoil;
    [SerializeField] private WeaponRecoilAnimator weaponAni;

    private float nextFireTime;
    public int currentAmmo;
    public int maxAmmo = 20;
    public int currentmaxAmmo;

    

    public AudioSource sound_shot;
    public ParticleSystem muzzlefire;
    [SerializeField] private ParticleSystem hitSparkPrefab;
    [SerializeField] private BulletHolePool holePool;

    public TMPro.TextMeshProUGUI ui_Ammo;
    public TMPro.TextMeshProUGUI ui_MagSize;
    public TMPro.TextMeshProUGUI ui_MaxAmmo;
    public GameObject ui_Weapon_Selection;

    [Header("Addition")]
    public float damageAdd = 0f;

    [Header("Multiplier")]
    public float damageMulti = 1f;
    public float reloadMulti = 1f;
    public int magazineMulti = 1;

    private void Awake()
    {
        currentmaxAmmo = maxAmmo;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    


    private void Start()
    {
        //ui_Weapon_Selection.SetActive(true);  //Weapon Auswahl am Anfang
        //CursorManager.ShowCursor();
    }

    public void SetUp()
    {
        currentmaxAmmo = maxAmmo;
        currentAmmo = 0;
        /*
        currentAmmo = weaponData.magazineSize;
        currentmaxAmmo = currentmaxAmmo - weaponData.magazineSize;
        ui_Ammo.GetComponent<TMPro.TextMeshProUGUI>().text = currentAmmo.ToString();
        ui_MagSize.GetComponent<TMPro.TextMeshProUGUI>().text = "/" + weaponData.magazineSize;
        ui_MaxAmmo.GetComponent<TMPro.TextMeshProUGUI>().text = currentmaxAmmo.ToString();*/
    }

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            ui_Ammo.text = currentAmmo.ToString();
            ui_MagSize.text = "/" + (weaponData.magazineSize * magazineMulti);
            ui_MaxAmmo.text = currentmaxAmmo.ToString();
        }

        if (weaponData.fireMode == FireMode.FullAuto || CanUseFullAuto())
        {
            if (Input.GetButton("Fire1"))
                TryShoot();
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
                TryShoot();
        }

        if (currentAmmo == 0 && currentmaxAmmo != 0)   //Nachladen: wenn Magazin leer und insgesammte Munition noch nicht 0
        {
            nextFireTime = Time.time + weaponData.reloadTime / reloadMulti;
            if (currentmaxAmmo >= weaponData.magazineSize * magazineMulti) //Magazin kleiner als Reserve Ammo
            {
                currentAmmo = weaponData.magazineSize * magazineMulti;
                currentmaxAmmo -= weaponData.magazineSize * magazineMulti;
            }
            else
            {
                currentAmmo = currentmaxAmmo;
                currentmaxAmmo = 0;
            }
            
            ui_Ammo.text = "reloading";
            return;
        }
    }

    void TryShoot()
    {
        if (Time.time < nextFireTime || currentAmmo <= 0)
            return;

        
        nextFireTime = Time.time + weaponData.fireRate;
        currentAmmo--;

        Shoot();
    }

    void Shoot()
    {
        muzzlefire.Play();
        recoil.ApplyRecoil(weaponData.recoilForce, weaponData.recoilForce/4);
        weaponAni.Fire();
        for (int i = 0; i < weaponData.pellets; i++)
        {
            Vector3 direction = playerCamera.transform.forward;
            direction += Random.insideUnitSphere * weaponData.spread;
            direction.Normalize();

            sound_shot.Play();
            ui_Ammo.text = currentAmmo.ToString();
            ui_MaxAmmo.text = currentmaxAmmo.ToString();

            if (weaponData.useProjectile)
            {
                SpawnProjectile(direction);
            }
            else
            {
                //Debug.Log("Shot");
                if (CanUsePiercingShot())
                {
                    RaycastPiercingShot(direction);
                }
                else
                {
                    RaycastShoot(direction);
                }
            }
        }
    }

    void RaycastShoot(Vector3 direction)
    {
        if (Physics.Raycast(playerCamera.transform.position, direction, out RaycastHit hit, weaponData.range))
        {
            // statt Instantiate: gepoolten Effekt an der Einschlagstelle abspielen
            Quaternion rot = Quaternion.LookRotation(hit.normal);
            


            if (hit.collider.TryGetComponent(out IDamageable damageable))
            {
                float dmg = (weaponData.damage + damageAdd) * damageMulti;//Brechnugn vom Schaden

                if (hit.transform.gameObject.GetComponent<HornShooting>() != null) //Horn Treffer
                {
                    damageable.TakeDamage(dmg);
                    ImpactEffectPool.Instance.Play(hitSparkPrefab, hit.point, rot);          // Funken spritzen
                }
                else                                                                //Gegner Treffer
                {
                    //Debug.Log("Hit");
                    damageable.TakeDamage(dmg);
                    // Schadenszahl genau am Einschlagpunkt
                    damageNumbers.Show(dmg, hit.point, Color.white);
                }
                
            }
            else                                                                    //Umgebungs Treffer
            {
                holePool.Place(hit.point, hit.normal);   // Einschussloch setzen
                ImpactEffectPool.Instance.Play(hitSparkPrefab, hit.point, rot);           // Funken spritzen
            }

            


            /*
            if (weaponData.hitEffect)
                Instantiate(weaponData.hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            */
        }
    }

    void RaycastPiercingShot(Vector3 direction)
    {
        RaycastHit[] hits = Physics.RaycastAll(
            playerCamera.transform.position,
            direction,
            weaponData.range
        );

        // Wichtig: RaycastAll garantiert nicht unbedingt die gewünschte Reihenfolge
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            Quaternion rot = Quaternion.LookRotation(hit.normal);

            if (hit.collider.TryGetComponent(out IDamageable damageable))
            {
                float dmg = (weaponData.damage + damageAdd) * damageMulti;          //Brechnugn vom Schaden

                if (hit.transform.gameObject.GetComponent<HornShooting>() != null)  //Horn Treffer
                {
                    damageable.TakeDamage(dmg);
                    ImpactEffectPool.Instance.Play(hitSparkPrefab, hit.point, rot); // Funken spritzen
                    break; // Wand/Object stoppt die Kugel
                }
                else                                                                //Gegner Treffer
                {
                    //Debug.Log("Hit");
                    damageable.TakeDamage(dmg);
                    // Schadenszahl genau am Einschlagpunkt
                    damageNumbers.Show(dmg, hit.point, Color.white);
                }

            }
            else                                                                    //Umgebungs Treffer
            {
                holePool.Place(hit.point, hit.normal);   // Einschussloch setzen
                ImpactEffectPool.Instance.Play(hitSparkPrefab, hit.point, rot);           // Funken spritzen
                break; // Wand/Object stoppt die Kugel
            }
        }
    }

    void SpawnProjectile(Vector3 direction)
    {
        GameObject proj = Instantiate(weaponData.projectilePrefab, firePoint.position, Quaternion.identity);
        proj.GetComponent<Rigidbody>().linearVelocity = direction * weaponData.projectileSpeed;
        proj.GetComponent<Projectile>().damage = weaponData.damage;
    }

    public void ChangeWeapon(WeaponData weapon)
    {
        weaponData = weapon;
        ui_Weapon_Selection.SetActive(false);
        CursorManager.HideCursor();
        Cursor.lockState = CursorLockMode.Locked;
        SetUp();
    }

    public void UpdateDamage(float add)
    {
        damageAdd += add;
    }

    public void UpdateDamageMulti(float multi)
    {
        if (MoneyManager.Instance.SpendMoney(1))
        {
            damageMulti += multi;
        }

    }

    public void UpdateReload(float multi)
    {
        reloadMulti += multi;
    }

    public void UpdateMaxAmmo(int add)
    {
        maxAmmo += add;
        currentmaxAmmo = maxAmmo;
        currentAmmo = 0;
    }

    public void UpdateMagazine(int muti)
    {
        magazineMulti += muti;
        currentmaxAmmo = maxAmmo;
        currentAmmo = 0;
    }

    public bool CanUseFullAuto()
    {
        return SkillManager.Instance.GetPlayerSkills().IsSkillUnlocked(PlayerSkills.SkillType.FullAuto);
    }

    public bool CanUsePiercingShot()
    {
        return SkillManager.Instance.GetPlayerSkills().IsSkillUnlocked(PlayerSkills.SkillType.PiercingShot);
    }
}
