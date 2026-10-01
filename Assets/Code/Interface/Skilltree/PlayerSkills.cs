using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerSkills
{
    public event EventHandler<OnSkillUnlockedEventArgs> OnSkillUnlocked; //Für Update Skills
    public class OnSkillUnlockedEventArgs : EventArgs
    {
        public SkillType skilltype;
    }

    public enum SkillType
    {
        None,
        DamageAdd_1,
        DamageAdd_2,
        MagazineMulti_1,
        AmmoAdd_1,
        AmmoAdd_2,
        Reload_1,
        FullAuto,
        PiercingShot,
        Demon_1,
        Demon_2,
        BulletDemon_1,
        BulletDemon_2,
        OrbitDemon_1,
        MoneyDemon_1,
        MoneyDemon_2,
        DistanceBoost_1,
        FlyingBoost_1,
    }

    private List<SkillType> unlockedSkillTypeList;

    public PlayerSkills()
    {
        unlockedSkillTypeList = new List<SkillType>();
    }

    public void UnlockSkill(SkillType skillType)
    {
        if (!IsSkillUnlocked(skillType))
        {
            unlockedSkillTypeList.Add(skillType);
            OnSkillUnlocked?.Invoke(this, new OnSkillUnlockedEventArgs { skilltype = skillType });
        }
    }

    public bool IsSkillUnlocked(SkillType skillType)
    {
        return unlockedSkillTypeList.Contains(skillType);
    }

    public bool CanUnlock(SkillType skillType) //Kann Skill freigeschalten werden?
    {
        SkillType skillRequirement = GetSkillRequirement(skillType);

        if (skillRequirement != SkillType.None) //Ist es nicht der None Skill?
        {
            if (IsSkillUnlocked(skillRequirement)) //Ist Skill noch nicht freigeschalten?
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }

    public SkillType GetSkillRequirement(SkillType skillType) //Welcher Skill muss zuvor freigeschalten sein?
    {
        switch (skillType)
        {
            case SkillType.DamageAdd_1:
                return SkillType.Reload_1;

            case SkillType.DamageAdd_2:
                return SkillType.DamageAdd_1;

            case SkillType.AmmoAdd_1:
                return SkillType.Reload_1;

            case SkillType.AmmoAdd_2:
                return SkillType.AmmoAdd_1;

            case SkillType.MagazineMulti_1:
                return SkillType.DamageAdd_1;

            case SkillType.Demon_2:
                return SkillType.Demon_1;

            case SkillType.BulletDemon_1:
                return SkillType.Demon_2;

            case SkillType.BulletDemon_2:
                return SkillType.BulletDemon_1;

            case SkillType.OrbitDemon_1:
                return SkillType.Demon_1;

            case SkillType.MoneyDemon_1:
                return SkillType.Demon_1;

            case SkillType.MoneyDemon_2:
                return SkillType.MoneyDemon_1;

            case SkillType.DistanceBoost_1:
                return SkillType.DamageAdd_2;

            case SkillType.PiercingShot:
                return SkillType.Reload_1;

        }
        return SkillType.None;
    }

    public bool TryUnlockSkill(SkillType skillType) //Kann Skill freigeschalten werden? (wurde vereinfacht indem über CanUnlock-Funktion geprüft wird)
    {
        if (CanUnlock(skillType))
        {
            if (ConfigSkills.Instance.CanBuy(skillType)) //checkt in Skill Config ob genug geld
            {
                UnlockSkill(skillType);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
