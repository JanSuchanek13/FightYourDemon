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
    public string Demon_1_Name = "Demon Spawnrate";
    public string Demon_1_Desc = "Increasing the Demon Spawnrate";
    public int Demon_1_Value = 1;

    public int Demon_2_Price = 300;
    public string Demon_2_Name = "Demon Spawnrate";
    public string Demon_2_Desc = "Increasing the Demon Spawnrate";
    public int Demon_2_Value = 1;

    public int BulletDemon_1_Price = 500;
    public string BulletDemon_1_Name = "Military Demon";
    public string BulletDemon_1_Desc = "Increasing the Military Demon Spawnchance";
    public float BulletDemon_1_Value = 0.01f;

    public int BulletDemon_2_Price = 1000;
    public string BulletDemon_2_Name = "Military Demon";
    public string BulletDemon_2_Desc = "Increasing the Military Demon Spawnchance";
    public float BulletDemon_2_Value = 0.01f;

    public int OrbitDemon_1_Price = 30;
    public string OrbitDemon_1_Name = "Planet Demon";
    public string OrbitDemon_1_Desc = "Increasing the Planet Demon Spawnchance";
    public float OrbitDemon_1_Value = 0.05f;

    public int MoneyDemon_1_Price = 80;
    public string MoneyDemon_1_Name = "Business Demon";
    public string MoneyDemon_1_Desc = "Increasing the Business Demon Spawnchance";
    public float MoneyDemon_1_Value = 0.01f;

    public int MoneyDemon_2_Price = 100;
    public string MoneyDemon_2_Name = "Business Demon";
    public string MoneyDemon_2_Desc = "Increasing the Business Demon Spawnchance";
    public float MoneyDemon_2_Value = 0.01f;

    public int DistanceBoost_1_Price = 2000;
    public string DistanceBoost_1_Name = "Boost by Distance";
    public string DistanceBoost_1_Desc = "Additional boost if the target is farther away";
    public float DistanceBoost_1_Value = 0.1f;

    public int FlyingBoost_1_Price = 1;
    public string FlyingBoost_1_Name = "Boost per Kill";
    public string FlyingBoost_1_Desc = "Additional boost when an enemy ist killed";
    public float FlyingBoost_1_Value = 0.5f;

    public int Demon_3_Price = 300;
    public string Demon_3_Name = "Demon Spawnrate";
    public string Demon_3_Desc = "Increasing the Demon Spawnrate";
    public int Demon_3_Value = 1;

    public int SplitDemon_1_Price = 300;
    public string SplitDemon_1_Name = "Split Demon";
    public string SplitDemon_1_Desc = "Increasing the Split Demon Spawnchance";
    public float SplitDemon_1_Value = 0.05f;

    public int BombDemon_1_Price = 300;
    public string BombDemon_1_Name = "Bomb Demon";
    public string BombDemon_1_Desc = "Increasing the Bomb Demon Spawnchance";
    public float BombDemon_1_Value = 0.05f;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    // Nur pruefen, ob genug Geld da ist - zieht NICHTS ab
    public bool HasEnoughMoney(PlayerSkills.SkillType skillType)
    {
        int price = GetPrice(skillType);
        if (price < 0) return false;   // unbekannter Skill
        return MoneyManager.Instance.GetMoney() >= price;
    }

    // Liefert den Preis eines Skills
    public int GetPrice(PlayerSkills.SkillType skillType)
    {
        switch (skillType)
        {
            case PlayerSkills.SkillType.DamageAdd_1: return DamageAdd_1_Price;
            case PlayerSkills.SkillType.DamageAdd_2: return DamageAdd_2_Price;
            case PlayerSkills.SkillType.MagazineMulti_1: return MagazineMulti_1_Price;
            case PlayerSkills.SkillType.AmmoAdd_1: return AmmoAdd_1_Price;
            case PlayerSkills.SkillType.AmmoAdd_2: return AmmoAdd_2_Price;
            case PlayerSkills.SkillType.Reload_1: return Reload_1_Price;
            case PlayerSkills.SkillType.FullAuto: return FullAuto_Price;
            case PlayerSkills.SkillType.PiercingShot: return PiercingShot_Price;
            case PlayerSkills.SkillType.Demon_1: return Demon_1_Price;
            case PlayerSkills.SkillType.Demon_2: return Demon_2_Price;
            case PlayerSkills.SkillType.BulletDemon_1: return BulletDemon_1_Price;
            case PlayerSkills.SkillType.BulletDemon_2: return BulletDemon_2_Price;
            case PlayerSkills.SkillType.OrbitDemon_1: return OrbitDemon_1_Price;
            case PlayerSkills.SkillType.MoneyDemon_1: return MoneyDemon_1_Price;
            case PlayerSkills.SkillType.MoneyDemon_2: return MoneyDemon_2_Price;
            case PlayerSkills.SkillType.DistanceBoost_1: return DistanceBoost_1_Price;
            case PlayerSkills.SkillType.FlyingBoost_1: return FlyingBoost_1_Price;
            case PlayerSkills.SkillType.Demon_3: return Demon_3_Price;
            case PlayerSkills.SkillType.SplitDemon_1: return SplitDemon_1_Price;
            case PlayerSkills.SkillType.BombDemon_1: return BombDemon_1_Price;
            default: return -1;
        }
    }

    public string GetName(PlayerSkills.SkillType skillType)
    {
        switch (skillType)
        {
            case PlayerSkills.SkillType.DamageAdd_1: return DamageAdd_1_Name;
            case PlayerSkills.SkillType.DamageAdd_2: return DamageAdd_2_Name;
            case PlayerSkills.SkillType.MagazineMulti_1: return MagazineMulti_1_Name;
            case PlayerSkills.SkillType.AmmoAdd_1: return AmmoAdd_1_Name;
            case PlayerSkills.SkillType.AmmoAdd_2: return AmmoAdd_2_Name;
            case PlayerSkills.SkillType.Reload_1: return Reload_1_Name;
            case PlayerSkills.SkillType.FullAuto: return FullAuto_Name;
            case PlayerSkills.SkillType.PiercingShot: return PiercingShot_Name;
            case PlayerSkills.SkillType.Demon_1: return Demon_1_Name;
            case PlayerSkills.SkillType.Demon_2: return Demon_2_Name;
            case PlayerSkills.SkillType.BulletDemon_1: return BulletDemon_1_Name;
            case PlayerSkills.SkillType.BulletDemon_2: return BulletDemon_2_Name;
            case PlayerSkills.SkillType.OrbitDemon_1: return OrbitDemon_1_Name;
            case PlayerSkills.SkillType.MoneyDemon_1: return MoneyDemon_1_Name;
            case PlayerSkills.SkillType.MoneyDemon_2: return MoneyDemon_2_Name;
            case PlayerSkills.SkillType.DistanceBoost_1: return DistanceBoost_1_Name;
            case PlayerSkills.SkillType.FlyingBoost_1: return FlyingBoost_1_Name;
            case PlayerSkills.SkillType.Demon_3: return Demon_3_Name;
            case PlayerSkills.SkillType.SplitDemon_1: return SplitDemon_1_Name;
            case PlayerSkills.SkillType.BombDemon_1: return BombDemon_1_Name;
            default: return ("");
        }
    }

    public string GetDesc(PlayerSkills.SkillType skillType)
    {
        switch (skillType)
        {
            case PlayerSkills.SkillType.DamageAdd_1: return DamageAdd_1_Desc;
            case PlayerSkills.SkillType.DamageAdd_2: return DamageAdd_2_Desc;
            case PlayerSkills.SkillType.MagazineMulti_1: return MagazineMulti_1_Desc;
            case PlayerSkills.SkillType.AmmoAdd_1: return AmmoAdd_1_Desc;
            case PlayerSkills.SkillType.AmmoAdd_2: return AmmoAdd_2_Desc;
            case PlayerSkills.SkillType.Reload_1: return Reload_1_Desc;
            case PlayerSkills.SkillType.FullAuto: return FullAuto_Desc;
            case PlayerSkills.SkillType.PiercingShot: return PiercingShot_Desc;
            case PlayerSkills.SkillType.Demon_1: return Demon_1_Desc;
            case PlayerSkills.SkillType.Demon_2: return Demon_2_Desc;
            case PlayerSkills.SkillType.BulletDemon_1: return BulletDemon_1_Desc;
            case PlayerSkills.SkillType.BulletDemon_2: return BulletDemon_2_Desc;
            case PlayerSkills.SkillType.OrbitDemon_1: return OrbitDemon_1_Desc;
            case PlayerSkills.SkillType.MoneyDemon_1: return MoneyDemon_1_Desc;
            case PlayerSkills.SkillType.MoneyDemon_2: return MoneyDemon_2_Desc;
            case PlayerSkills.SkillType.DistanceBoost_1: return DistanceBoost_1_Desc;
            case PlayerSkills.SkillType.FlyingBoost_1: return FlyingBoost_1_Desc;
            case PlayerSkills.SkillType.Demon_3: return Demon_3_Desc;
            case PlayerSkills.SkillType.SplitDemon_1: return SplitDemon_1_Desc;
            case PlayerSkills.SkillType.BombDemon_1: return BombDemon_1_Desc;
            default: return ("");
        }
    }

    // Kauft tatsaechlich: prueft UND zieht Geld ab
    public bool CanBuy(PlayerSkills.SkillType skillType)
    {
        int price = GetPrice(skillType);
        if (price < 0) return false;

        if (MoneyManager.Instance.GetMoney() >= price)
        {
            MoneyManager.Instance.SpendMoney(price);
            return true;
        }
        return false;
    }


}
