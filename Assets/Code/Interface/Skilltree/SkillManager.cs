using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;

    private PlayerSkills playerSkills;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerSkills = new PlayerSkills(); //Erstellt eine neue, leere Liste der vorhandenen Skills
        playerSkills.OnSkillUnlocked += PlayerSkills_OnSkillUnlocked; //subscribed PlayerSkills_OnSkillUnlocked zu der Event Funktion in PlayerSkills
    }

    private void PlayerSkills_OnSkillUnlocked(object sender, PlayerSkills.OnSkillUnlockedEventArgs e)
    {
        switch (e.skilltype)
        {
            case PlayerSkills.SkillType.DamageAdd_1:
                WeaponController.Instance.UpdateDamage(ConfigSkills.Instance.DamageAdd_1_Value);
                break;
            case PlayerSkills.SkillType.DamageAdd_2:
                WeaponController.Instance.UpdateDamage(ConfigSkills.Instance.DamageAdd_2_Value);
                break;
            case PlayerSkills.SkillType.AmmoAdd_1:
                WeaponController.Instance.UpdateMaxAmmo(ConfigSkills.Instance.AmmoAdd_1_Value);
                break;
            case PlayerSkills.SkillType.AmmoAdd_2:
                WeaponController.Instance.UpdateMaxAmmo(ConfigSkills.Instance.AmmoAdd_2_Value);
                break;
            case PlayerSkills.SkillType.MagazineMulti_1:
                WeaponController.Instance.UpdateMagazine(ConfigSkills.Instance.MagazineMulti_1_Value);
                break;
            case PlayerSkills.SkillType.Reload_1:
                WeaponController.Instance.UpdateReload(ConfigSkills.Instance.Reload_1_Value);
                break;
            case PlayerSkills.SkillType.Demon_1:
                RisingEnvironment.Instance.UpdateDemon(ConfigSkills.Instance.Demon_1_Value);
                break;
            case PlayerSkills.SkillType.Demon_2:
                RisingEnvironment.Instance.UpdateDemon(ConfigSkills.Instance.Demon_2_Value);
                break;
            case PlayerSkills.SkillType.BulletDemon_1:
                RisingEnvironment.Instance.UpdateBulletDemon(ConfigSkills.Instance.BulletDemon_1_Value);
                break;
            case PlayerSkills.SkillType.BulletDemon_2:
                RisingEnvironment.Instance.UpdateBulletDemon(ConfigSkills.Instance.BulletDemon_2_Value);
                break;
            case PlayerSkills.SkillType.OrbitDemon_1:
                RisingEnvironment.Instance.UpdateOrbitDemon(ConfigSkills.Instance.OrbitDemon_1_Value);
                break;
            case PlayerSkills.SkillType.MoneyDemon_1:
                RisingEnvironment.Instance.UpdateMoneyDemon(ConfigSkills.Instance.MoneyDemon_1_Value);
                break;
            case PlayerSkills.SkillType.MoneyDemon_2:
                RisingEnvironment.Instance.UpdateMoneyDemon(ConfigSkills.Instance.MoneyDemon_2_Value);
                break;
            case PlayerSkills.SkillType.DistanceBoost_1:
                RisingEnvironment.Instance.UpdateDistanceBoost(ConfigSkills.Instance.DistanceBoost_1_Value);
                break;
            case PlayerSkills.SkillType.FlyingBoost_1:
                RisingEnvironment.Instance.UpdateFlyingBoost(ConfigSkills.Instance.FlyingBoost_1_Value);
                break;
        }
    }


    public PlayerSkills GetPlayerSkills()
    {
        return playerSkills;
    }
}
