using UnityEngine;

public enum FireMode
{
    SemiAuto,
    FullAuto
}

[CreateAssetMenu(menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("General")]
    public string weaponName;
    public FireMode fireMode;
    public float fireRate = 0.1f;
    public int pellets = 1;
    public float spread = 0f;

    [Header("Damage")]
    public float damage = 10f;
    public float range = 100f;

    [Header("Ammo")]
    public int magazineSize = 30;
    public float reloadTime = 1.5f;

    [Header("Recoil")]
    public float recoilForce = 1f;

    [Header("Projectile")]
    public bool useProjectile = false;
    public GameObject projectilePrefab;
    public float projectileSpeed = 30f;

    [Header("Effects")]
    public GameObject muzzleFlash;
    public ParticleSystem hitEffect;

    [Header("Sound")]
    public AudioClip shootSound;
    [Range(0f, 1f)] public float shootVolume = 1f;
}
