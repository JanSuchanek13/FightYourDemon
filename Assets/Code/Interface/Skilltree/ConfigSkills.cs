using UnityEngine;

public class ConfigSkills : MonoBehaviour
{
    public static ConfigSkills Instance;

    public int DamageAdd_1_Price = 8;
    public string DamageAdd_1_Name = "Add Damage";
    public string DamageAdd_1_Desc = "Increasing the Weapon Damage";
    public int DamageAdd_1_Value = 20;

    public int DamageAdd_2_Price = 50;
    public string DamageAdd_2_Name = "Add Damage";
    public string DamageAdd_2_Desc = "Increasing the Weapon Damage";
    public int DamageAdd_2_Value = 100;

    public int MagazineMulti_1_Price = 100;
    public string MagazineMulti_1_Name = "Magazine";
    public string MagazineMulti_1_Desc = "Increasing the Weapon Magazine";
    public int MagazineMulti_1_Value = 1;

    public int AmmoAdd_1_Price = 8;
    public string AmmoAdd_1_Name = "Add Ammo";
    public string AmmoAdd_1_Desc = "Increasing the Ammo";
    public int AmmoAdd_1_Value = 20;

    public int AmmoAdd_2_Price = 50;
    public string AmmoAdd_2_Name = "Add Ammo";
    public string AmmoAdd_2_Desc = "Increasing the Ammo";
    public int AmmoAdd_2_Value = 20;

    public int Reload_1_Price = 3;
    public string Reload_1_Name = "Reload";
    public string Reload_1_Desc = "Decreasing the Reload Time";
    public float Reload_1_Value = 1f;

    public int FullAuto_Price = 10;
    public string FullAuto_Name = "Full Auto";
    public string FullAuto_Desc = "Changing the Weapon Firetyp to FullAuto";
    public int FullAuto_Value = 99;

    public int PiercingShot_Price = 200; 
    public string PiercingShot_Name = "Piercing Shot";
    public string PiercingShot_Desc = "Bullets pass through Demons and strike the targets behind it";
    public int PiercingShot_Value = 99;

    public int Demon_1_Price = 5;
    public string Demon_1_Name = "Add Demon";
    public string Demon_1_Desc = "Increasing the Demons";
    public int Demon_1_Value = 10;

    public int Demon_2_Price = 300;
    public string Demon_2_Name = "Add Demon";
    public string Demon_2_Desc = "Increasing the Demons";
    public int Demon_2_Value = 20;

    public int BulletDemon_1_Price = 500;
    public string BulletDemon_1_Name = "Add Green Demon";
    public string BulletDemon_1_Desc = "Adding new green Demon";
    public int BulletDemon_1_Value = 1;

    public int BulletDemon_2_Price = 1000;
    public string BulletDemon_2_Name = "Add Green Demon";
    public string BulletDemon_2_Desc = "Increasing the green Demons";
    public int BulletDemon_2_Value = 2;

    public int OrbitDemon_1_Price = 30;
    public string OrbitDemon_1_Name = "Add Orange Demon";
    public string OrbitDemon_1_Desc = "Adding new orange Demon";
    public int OrbitDemon_1_Value = 1;

    public int MoneyDemon_1_Price = 80;
    public string MoneyDemon_1_Name = "Add Blue Demon";
    public string MoneyDemon_1_Desc = "Adding new blue Demon";
    public int MoneyDemon_1_Value = 1;

    public int MoneyDemon_2_Price = 100;
    public string MoneyDemon_2_Name = "Add BlueDemon";
    public string MoneyDemon_2_Desc = "Increasing the blue Demons";
    public int MoneyDemon_2_Value = 5;

    public int DistanceBoost_1_Price = 2000;
    public string DistanceBoost_1_Name = "Boost by Distance";
    public string DistanceBoost_1_Desc = "Additional boost if the target is farther away";
    public float DistanceBoost_1_Value = 0.1f;

    public int FlyingBoost_1_Price = 1;
    public string FlyingBoost_1_Name = "Boost per Kill";
    public string FlyingBoost_1_Desc = "Additional boost when an enemy ist killed";
    public float FlyingBoost_1_Value = 0.5f;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool CanBuy(PlayerSkills.SkillType skillType)
    {
        switch (skillType)  //checkt nach Skill und seinen Kosten
        {
            case PlayerSkills.SkillType.DamageAdd_1: 
                if (MoneyManager.Instance.GetMoney() >= DamageAdd_1_Price) 
                {
                    Debug.Log("gekauft");
                    MoneyManager.Instance.SpendMoney(DamageAdd_1_Price);
                    return true;
                }
                else
                {
                    return false;
                }

            case PlayerSkills.SkillType.DamageAdd_2:
                if (MoneyManager.Instance.GetMoney() >= DamageAdd_2_Price)
                {
                    Debug.Log("gekauft");
                    MoneyManager.Instance.SpendMoney(DamageAdd_2_Price);
                    return true;
                }
                else
                {
                    return false;
                }

            case PlayerSkills.SkillType.MagazineMulti_1:
                if (MoneyManager.Instance.GetMoney() >= MagazineMulti_1_Price)
                {
                    Debug.Log("gekauft");
                    MoneyManager.Instance.SpendMoney(MagazineMulti_1_Price);
                    return true;
                }
                else
                {
                    return false;
                }

            case PlayerSkills.SkillType.AmmoAdd_1:
                if (MoneyManager.Instance.GetMoney() >= AmmoAdd_1_Price) 
                {
                    Debug.Log("gekauft");
                    MoneyManager.Instance.SpendMoney(AmmoAdd_1_Price);
                    return true;
                }
                else
                {
                    return false;
                }

            case PlayerSkills.SkillType.AmmoAdd_2:
                if (MoneyManager.Instance.GetMoney() >= AmmoAdd_2_Price)
                {
                    Debug.Log("gekauft");
                    MoneyManager.Instance.SpendMoney(AmmoAdd_2_Price);
                    return true;
                }
                else
                {
                    return false;
                }

            case PlayerSkills.SkillType.Reload_1:
                if (MoneyManager.Instance.GetMoney() >= Reload_1_Price)
                {
                    Debug.Log("gekauft");
                    MoneyManager.Instance.SpendMoney(Reload_1_Price);
                    return true;
                }
                else
                {
                    return false;
                }

            case PlayerSkills.SkillType.FullAuto:
                if (MoneyManager.Instance.GetMoney() >= FullAuto_Price)
                {
                    Debug.Log("gekauft");
                    MoneyManager.Instance.SpendMoney(FullAuto_Price);
                    return true;
                }
                else
                {
                    return false;
                }

            case PlayerSkills.SkillType.PiercingShot:
                if (MoneyManager.Instance.GetMoney() >= PiercingShot_Price)
                {
                    Debug.Log("gekauft");
                    MoneyManager.Instance.SpendMoney(PiercingShot_Price);
                    return true;
                }
                else
                {
                    return false;
                }

            case PlayerSkills.SkillType.Demon_1:
                if (MoneyManager.Instance.GetMoney() >= Demon_1_Price)
                {
                    Debug.Log("gekauft");
                    MoneyManager.Instance.SpendMoney(Demon_1_Price);
                    return true;
                }
                else
                {
                    return false;
                }

            case PlayerSkills.SkillType.Demon_2:
                if (MoneyManager.Instance.GetMoney() >= Demon_2_Price)
                {
                    Debug.Log("gekauft");
                    MoneyManager.Instance.SpendMoney(Demon_2_Price);
                    return true;
                }
                else
                {
                    return false;
                }

            case PlayerSkills.SkillType.BulletDemon_1:
                if (MoneyManager.Instance.GetMoney() >= BulletDemon_1_Price)
                {
                    Debug.Log("gekauft");
                    MoneyManager.Instance.SpendMoney(BulletDemon_1_Price);
                    return true;
                }
                else
                {
                    return false;
                }

            case PlayerSkills.SkillType.BulletDemon_2:
                if (MoneyManager.Instance.GetMoney() >= BulletDemon_2_Price)
                {
                    Debug.Log("gekauft");
                    MoneyManager.Instance.SpendMoney(BulletDemon_2_Price);
                    return true;
                }
                else
                {
                    return false;
                }

            case PlayerSkills.SkillType.OrbitDemon_1:
                if (MoneyManager.Instance.GetMoney() >= OrbitDemon_1_Price)
                {
                    Debug.Log("gekauft");
                    MoneyManager.Instance.SpendMoney(OrbitDemon_1_Price);
                    return true;
                }
                else
                {
                    return false;
                }

            case PlayerSkills.SkillType.MoneyDemon_1:
                if (MoneyManager.Instance.GetMoney() >= MoneyDemon_1_Price)
                {
                    Debug.Log("gekauft");
                    MoneyManager.Instance.SpendMoney(MoneyDemon_1_Price);
                    return true;
                }
                else
                {
                    return false;
                }

            case PlayerSkills.SkillType.MoneyDemon_2:
                if (MoneyManager.Instance.GetMoney() >= MoneyDemon_2_Price)
                {
                    Debug.Log("gekauft");
                    MoneyManager.Instance.SpendMoney(MoneyDemon_2_Price);
                    return true;
                }
                else
                {
                    return false;
                }

            case PlayerSkills.SkillType.DistanceBoost_1:
                if (MoneyManager.Instance.GetMoney() >= DistanceBoost_1_Price)
                {
                    Debug.Log("gekauft");
                    MoneyManager.Instance.SpendMoney(DistanceBoost_1_Price);
                    return true;
                }
                else
                {
                    return false;
                }

            case PlayerSkills.SkillType.FlyingBoost_1:
                if (MoneyManager.Instance.GetMoney() >= FlyingBoost_1_Price)
                {
                    Debug.Log("gekauft");
                    MoneyManager.Instance.SpendMoney(FlyingBoost_1_Price);
                    return true;
                }
                else
                {
                    return false;
                }



            case PlayerSkills.SkillType.None:
                break;

        }
        return false;
    }
}
