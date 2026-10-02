using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillTree : MonoBehaviour
{
    private PlayerSkills playerSkills;
    [SerializeField] private WeaponController weapon;

    public GameObject skillTooltip;
    [SerializeField] private TMP_Text uiName;
    [SerializeField] private TMP_Text uiDesc;
    [SerializeField] private TMP_Text uiStat1;
    [SerializeField] private TMP_Text uiStat2;
    [SerializeField] private TMP_Text uiPrice;
    public MouseLook mouseLook;

    [SerializeField] private Material skillLockedMaterial;
    [SerializeField] private Material skillUnlockedMaterial;
    [SerializeField] private SkillUnlockPath[] skillUnlockPathsArray;



    private void Awake()
    {
        transform.Find("Background").GetComponent<Button_UI>().MouseOverOnceFunc = () => //Lääst beim Hovern auf den Hintergrund den Tooltipp verschwinden
        {
            //Debug.Log("Background Hover");
            skillTooltip.transform.position = transform.Find("Background").position + Vector3.up * 1000;
        };

        transform.Find("FullAuto").GetComponent<Button_UI>().ClickFunc = () =>
        {
            if (!playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.FullAuto))
                playerSkills.TryUnlockSkill(PlayerSkills.SkillType.FullAuto);
            Debug.Log("FullAuto!");
        };
        transform.Find("FullAuto").GetComponent<Button_UI>().MouseOverOnceFunc = () =>
        {
            if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.FullAuto) || playerSkills.CanUnlock(PlayerSkills.SkillType.FullAuto)) //Ist Skill sichtbar > dann Tooltip anzeigen
            {
                skillTooltip.transform.position = transform.Find("FullAuto").position + Vector3.up * 10;
                uiName.text = ConfigSkills.Instance.FullAuto_Name;
                uiDesc.text = ConfigSkills.Instance.FullAuto_Desc;
                uiStat1.text = weapon.weaponData.fireMode.ToString();
                uiStat2.text = "Full Auto";
                uiPrice.text = ConfigSkills.Instance.FullAuto_Price.ToString();
            }
        };

        transform.Find("PiercingShot").GetComponent<Button_UI>().ClickFunc = () =>
        {
            if (!playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.PiercingShot))
                playerSkills.TryUnlockSkill(PlayerSkills.SkillType.PiercingShot);
            Debug.Log("PiercingShot!");
        };
        transform.Find("PiercingShot").GetComponent<Button_UI>().MouseOverOnceFunc = () =>
        {
            if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.PiercingShot) || playerSkills.CanUnlock(PlayerSkills.SkillType.PiercingShot)) //Ist Skill sichtbar > dann Tooltip anzeigen
            {
                skillTooltip.transform.position = transform.Find("PiercingShot").position + Vector3.up * 10;
                uiName.text = ConfigSkills.Instance.PiercingShot_Name;
                uiDesc.text = ConfigSkills.Instance.PiercingShot_Desc;
                uiStat1.text = "Normal";
                uiStat2.text = "Piercing";
                uiPrice.text = ConfigSkills.Instance.PiercingShot_Price.ToString();
            }
        };

        transform.Find("DamageAdd_1").GetComponent<Button_UI>().ClickFunc = () =>
        {
            if (!playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.DamageAdd_1))
                playerSkills.TryUnlockSkill(PlayerSkills.SkillType.DamageAdd_1);
            Debug.Log("Damage1");
        };
        transform.Find("DamageAdd_1").GetComponent<Button_UI>().MouseOverOnceFunc = () =>
        {
            if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.DamageAdd_1) || playerSkills.CanUnlock(PlayerSkills.SkillType.DamageAdd_1)) //Ist Skill sichtbar > dann Tooltip anzeigen
            {
                skillTooltip.transform.position = transform.Find("DamageAdd_1").position + Vector3.up * 10;
                uiName.text = ConfigSkills.Instance.DamageAdd_1_Name;
                uiDesc.text = ConfigSkills.Instance.DamageAdd_1_Desc;
                uiStat1.text = (weapon.weaponData.damage + weapon.damageAdd).ToString();
                uiStat2.text = (weapon.weaponData.damage + weapon.damageAdd + ConfigSkills.Instance.DamageAdd_1_Value).ToString();
                uiPrice.text = ConfigSkills.Instance.DamageAdd_1_Price.ToString();
            }
        };

        transform.Find("DamageAdd_2").GetComponent<Button_UI>().ClickFunc = () =>
        {
            if (!playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.DamageAdd_2))
                playerSkills.TryUnlockSkill(PlayerSkills.SkillType.DamageAdd_2);
            Debug.Log("Damage2");
        };
        transform.Find("DamageAdd_2").GetComponent<Button_UI>().MouseOverOnceFunc = () =>
        {
            if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.DamageAdd_2) || playerSkills.CanUnlock(PlayerSkills.SkillType.DamageAdd_2)) //Ist Skill sichtbar > dann Tooltip anzeigen
            {
                skillTooltip.transform.position = transform.Find("DamageAdd_2").position + Vector3.up * 10;
                uiName.text = ConfigSkills.Instance.DamageAdd_2_Name;
                uiDesc.text = ConfigSkills.Instance.DamageAdd_2_Desc;
                uiStat1.text = (weapon.weaponData.damage + weapon.damageAdd).ToString();
                uiStat2.text = (weapon.weaponData.damage + weapon.damageAdd + ConfigSkills.Instance.DamageAdd_2_Value).ToString();
                uiPrice.text = ConfigSkills.Instance.DamageAdd_2_Price.ToString();
            }
        };

        transform.Find("AmmoAdd_1").GetComponent<Button_UI>().ClickFunc = () =>
        {
            if (!playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.AmmoAdd_1))
                playerSkills.TryUnlockSkill(PlayerSkills.SkillType.AmmoAdd_1);
            Debug.Log("Ammo1");
        };
        transform.Find("AmmoAdd_1").GetComponent<Button_UI>().MouseOverOnceFunc = () =>
        {
            if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.AmmoAdd_1) || playerSkills.CanUnlock(PlayerSkills.SkillType.AmmoAdd_1)) //Ist Skill sichtbar > dann Tooltip anzeigen
            {
                skillTooltip.transform.position = transform.Find("AmmoAdd_1").position + Vector3.up * 10;
                uiName.text = ConfigSkills.Instance.AmmoAdd_1_Name;
                uiDesc.text = ConfigSkills.Instance.AmmoAdd_1_Desc;
                uiStat1.text = weapon.maxAmmo.ToString();
                uiStat2.text = (weapon.maxAmmo + ConfigSkills.Instance.AmmoAdd_1_Value).ToString();
                uiPrice.text = ConfigSkills.Instance.AmmoAdd_1_Price.ToString();
            }
        };

        transform.Find("AmmoAdd_2").GetComponent<Button_UI>().ClickFunc = () =>
        {
            if (!playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.AmmoAdd_2))
                playerSkills.TryUnlockSkill(PlayerSkills.SkillType.AmmoAdd_2);
            Debug.Log("Ammo2");
        };
        transform.Find("AmmoAdd_2").GetComponent<Button_UI>().MouseOverOnceFunc = () =>
        {
            if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.AmmoAdd_2) || playerSkills.CanUnlock(PlayerSkills.SkillType.AmmoAdd_2)) //Ist Skill sichtbar > dann Tooltip anzeigen
            {
                skillTooltip.transform.position = transform.Find("AmmoAdd_2").position + Vector3.up * 10;
                uiName.text = ConfigSkills.Instance.AmmoAdd_2_Name;
                uiDesc.text = ConfigSkills.Instance.AmmoAdd_2_Desc;
                uiStat1.text = weapon.maxAmmo.ToString();
                uiStat2.text = (weapon.maxAmmo + ConfigSkills.Instance.AmmoAdd_2_Value).ToString();
                uiPrice.text = ConfigSkills.Instance.AmmoAdd_2_Price.ToString();
            }
        };

        transform.Find("MagazineMulti_1").GetComponent<Button_UI>().ClickFunc = () =>
        {
            if (!playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.MagazineMulti_1))
                playerSkills.TryUnlockSkill(PlayerSkills.SkillType.MagazineMulti_1);
            Debug.Log("MagazineMulti_1");
        };
        transform.Find("MagazineMulti_1").GetComponent<Button_UI>().MouseOverOnceFunc = () =>
        {
            if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.MagazineMulti_1) || playerSkills.CanUnlock(PlayerSkills.SkillType.MagazineMulti_1)) //Ist Skill sichtbar > dann Tooltip anzeigen
            {
                skillTooltip.transform.position = transform.Find("MagazineMulti_1").position + Vector3.up * 10;
                uiName.text = ConfigSkills.Instance.MagazineMulti_1_Name;
                uiDesc.text = ConfigSkills.Instance.MagazineMulti_1_Desc;
                uiStat1.text = (weapon.weaponData.magazineSize * weapon.magazineMulti).ToString();
                uiStat2.text = (weapon.weaponData.magazineSize * (weapon.magazineMulti + ConfigSkills.Instance.MagazineMulti_1_Value)).ToString();
                uiPrice.text = ConfigSkills.Instance.MagazineMulti_1_Price.ToString();
            }
        };

        transform.Find("Reload_1").GetComponent<Button_UI>().ClickFunc = () =>
        {
            if (!playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.Reload_1))
                playerSkills.TryUnlockSkill(PlayerSkills.SkillType.Reload_1);
            Debug.Log("Reload_1");
        };
        transform.Find("Reload_1").GetComponent<Button_UI>().MouseOverOnceFunc = () =>
        {
            if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.Reload_1) || playerSkills.CanUnlock(PlayerSkills.SkillType.Reload_1)) //Ist Skill sichtbar > dann Tooltip anzeigen
            {
                skillTooltip.transform.position = transform.Find("Reload_1").position + Vector3.up * 10;
                uiName.text = ConfigSkills.Instance.Reload_1_Name;
                uiDesc.text = ConfigSkills.Instance.Reload_1_Desc;
                uiStat1.text = (weapon.weaponData.reloadTime / weapon.reloadMulti).ToString();
                uiStat2.text = (weapon.weaponData.reloadTime / (weapon.reloadMulti + ConfigSkills.Instance.Reload_1_Value)).ToString();
                uiPrice.text = ConfigSkills.Instance.Reload_1_Price.ToString();
            }
        };

        transform.Find("Demon_1").GetComponent<Button_UI>().ClickFunc = () =>
        {
            if (!playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.Demon_1))
                playerSkills.TryUnlockSkill(PlayerSkills.SkillType.Demon_1);
            Debug.Log("Demon_1");
        };
        transform.Find("Demon_1").GetComponent<Button_UI>().MouseOverOnceFunc = () =>
        {
            if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.Demon_1) || playerSkills.CanUnlock(PlayerSkills.SkillType.Demon_1)) //Ist Skill sichtbar > dann Tooltip anzeigen
            {
                skillTooltip.transform.position = transform.Find("Demon_1").position + Vector3.up * 10;
                uiName.text = ConfigSkills.Instance.Demon_1_Name;
                uiDesc.text = ConfigSkills.Instance.Demon_1_Desc;
                uiStat1.text = (RisingEnvironment.Instance.DemonCount).ToString();
                uiStat2.text = (RisingEnvironment.Instance.DemonCount + ConfigSkills.Instance.Demon_1_Value).ToString();
                uiPrice.text = ConfigSkills.Instance.Demon_1_Price.ToString();
            }
        };

        transform.Find("Demon_2").GetComponent<Button_UI>().ClickFunc = () =>
        {
            if (!playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.Demon_2))
                playerSkills.TryUnlockSkill(PlayerSkills.SkillType.Demon_2);
            Debug.Log("Demon_2");
        };
        transform.Find("Demon_2").GetComponent<Button_UI>().MouseOverOnceFunc = () =>
        {
            if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.Demon_2) || playerSkills.CanUnlock(PlayerSkills.SkillType.Demon_2)) //Ist Skill sichtbar > dann Tooltip anzeigen
            {
                skillTooltip.transform.position = transform.Find("Demon_2").position + Vector3.up * 10;
                uiName.text = ConfigSkills.Instance.Demon_2_Name;
                uiDesc.text = ConfigSkills.Instance.Demon_2_Desc;
                uiStat1.text = (RisingEnvironment.Instance.DemonCount).ToString();
                uiStat2.text = (RisingEnvironment.Instance.DemonCount + ConfigSkills.Instance.Demon_2_Value).ToString();
                uiPrice.text = ConfigSkills.Instance.Demon_2_Price.ToString();
            }
        };

        transform.Find("BulletDemon_1").GetComponent<Button_UI>().ClickFunc = () =>
        {
            if (!playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.BulletDemon_1))
                playerSkills.TryUnlockSkill(PlayerSkills.SkillType.BulletDemon_1);
            Debug.Log("BulletDemon_1");
        };
        transform.Find("BulletDemon_1").GetComponent<Button_UI>().MouseOverOnceFunc = () =>
        {
            if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.BulletDemon_1) || playerSkills.CanUnlock(PlayerSkills.SkillType.BulletDemon_1)) //Ist Skill sichtbar > dann Tooltip anzeigen
            {
                skillTooltip.transform.position = transform.Find("BulletDemon_1").position + Vector3.up * 10;
                uiName.text = ConfigSkills.Instance.BulletDemon_1_Name;
                uiDesc.text = ConfigSkills.Instance.BulletDemon_1_Desc;
                uiStat1.text = (RisingEnvironment.Instance.BulletDemonCount).ToString();
                uiStat2.text = (RisingEnvironment.Instance.BulletDemonCount + ConfigSkills.Instance.BulletDemon_1_Value).ToString();
                uiPrice.text = ConfigSkills.Instance.BulletDemon_1_Price.ToString();
            }
        };

        transform.Find("BulletDemon_2").GetComponent<Button_UI>().ClickFunc = () =>
        {
            if (!playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.BulletDemon_2))
                playerSkills.TryUnlockSkill(PlayerSkills.SkillType.BulletDemon_2);
            Debug.Log("BulletDemon_2");
        };
        transform.Find("BulletDemon_2").GetComponent<Button_UI>().MouseOverOnceFunc = () =>
        {
            if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.BulletDemon_2) || playerSkills.CanUnlock(PlayerSkills.SkillType.BulletDemon_2)) //Ist Skill sichtbar > dann Tooltip anzeigen
            {
                skillTooltip.transform.position = transform.Find("BulletDemon_2").position + Vector3.up * 10;
                uiName.text = ConfigSkills.Instance.BulletDemon_2_Name;
                uiDesc.text = ConfigSkills.Instance.BulletDemon_2_Desc;
                uiStat1.text = (RisingEnvironment.Instance.BulletDemonCount).ToString();
                uiStat2.text = (RisingEnvironment.Instance.BulletDemonCount + ConfigSkills.Instance.BulletDemon_2_Value).ToString();
                uiPrice.text = ConfigSkills.Instance.BulletDemon_2_Price.ToString();
            }
        };

        transform.Find("OrbitDemon_1").GetComponent<Button_UI>().ClickFunc = () =>
        {
            if (!playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.OrbitDemon_1))
                playerSkills.TryUnlockSkill(PlayerSkills.SkillType.OrbitDemon_1);
            Debug.Log("OrbitDemon_1");
        };
        transform.Find("OrbitDemon_1").GetComponent<Button_UI>().MouseOverOnceFunc = () =>
        {
            if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.OrbitDemon_1) || playerSkills.CanUnlock(PlayerSkills.SkillType.OrbitDemon_1)) //Ist Skill sichtbar > dann Tooltip anzeigen
            {
                skillTooltip.transform.position = transform.Find("OrbitDemon_1").position + Vector3.up * 10;
                uiName.text = ConfigSkills.Instance.OrbitDemon_1_Name;
                uiDesc.text = ConfigSkills.Instance.OrbitDemon_1_Desc;
                uiStat1.text = (RisingEnvironment.Instance.OrbitDemonCount).ToString();
                uiStat2.text = (RisingEnvironment.Instance.OrbitDemonCount + ConfigSkills.Instance.OrbitDemon_1_Value).ToString();
                uiPrice.text = ConfigSkills.Instance.OrbitDemon_1_Price.ToString();
            }
        };

        transform.Find("MoneyDemon_1").GetComponent<Button_UI>().ClickFunc = () =>
        {
            if (!playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.MoneyDemon_1))
                playerSkills.TryUnlockSkill(PlayerSkills.SkillType.MoneyDemon_1);
            Debug.Log("MoneyDemon_1");
        };
        transform.Find("MoneyDemon_1").GetComponent<Button_UI>().MouseOverOnceFunc = () =>
        {
            if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.MoneyDemon_1) || playerSkills.CanUnlock(PlayerSkills.SkillType.MoneyDemon_1)) //Ist Skill sichtbar > dann Tooltip anzeigen
            {
                skillTooltip.transform.position = transform.Find("MoneyDemon_1").position + Vector3.up * 10;
                uiName.text = ConfigSkills.Instance.MoneyDemon_1_Name;
                uiDesc.text = ConfigSkills.Instance.MoneyDemon_1_Desc;
                uiStat1.text = (RisingEnvironment.Instance.MoneyDemonCount).ToString();
                uiStat2.text = (RisingEnvironment.Instance.MoneyDemonCount + ConfigSkills.Instance.MoneyDemon_1_Value).ToString();
                uiPrice.text = ConfigSkills.Instance.MoneyDemon_1_Price.ToString();
            }
        };

        transform.Find("MoneyDemon_2").GetComponent<Button_UI>().ClickFunc = () =>
        {
            if (!playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.MoneyDemon_2))
                playerSkills.TryUnlockSkill(PlayerSkills.SkillType.MoneyDemon_2);
            Debug.Log("MoneyDemon_2");
        };
        transform.Find("MoneyDemon_2").GetComponent<Button_UI>().MouseOverOnceFunc = () =>
        {
            if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.MoneyDemon_2) || playerSkills.CanUnlock(PlayerSkills.SkillType.MoneyDemon_2)) //Ist Skill sichtbar > dann Tooltip anzeigen
            {
                skillTooltip.transform.position = transform.Find("MoneyDemon_2").position + Vector3.up * 10;
                uiName.text = ConfigSkills.Instance.MoneyDemon_2_Name;
                uiDesc.text = ConfigSkills.Instance.MoneyDemon_2_Desc;
                uiStat1.text = (RisingEnvironment.Instance.MoneyDemonCount).ToString();
                uiStat2.text = (RisingEnvironment.Instance.MoneyDemonCount + ConfigSkills.Instance.MoneyDemon_2_Value).ToString();
                uiPrice.text = ConfigSkills.Instance.MoneyDemon_2_Price.ToString();
            }
        };

        transform.Find("DistanceBoost_1").GetComponent<Button_UI>().ClickFunc = () =>
        {
            if (!playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.DistanceBoost_1))
                playerSkills.TryUnlockSkill(PlayerSkills.SkillType.DistanceBoost_1);
            Debug.Log("DistanceBoost_1");
        };
        transform.Find("DistanceBoost_1").GetComponent<Button_UI>().MouseOverOnceFunc = () =>
        {
            if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.DistanceBoost_1) || playerSkills.CanUnlock(PlayerSkills.SkillType.DistanceBoost_1)) //Ist Skill sichtbar > dann Tooltip anzeigen
            {
                skillTooltip.transform.position = transform.Find("DistanceBoost_1").position + Vector3.up * 10;
                uiName.text = ConfigSkills.Instance.DistanceBoost_1_Name;
                uiDesc.text = ConfigSkills.Instance.DistanceBoost_1_Desc;
                uiStat1.text = (RisingEnvironment.Instance.distanceBoost).ToString();
                uiStat2.text = (RisingEnvironment.Instance.distanceBoost + ConfigSkills.Instance.DistanceBoost_1_Value).ToString();
                uiPrice.text = ConfigSkills.Instance.DistanceBoost_1_Price.ToString();
            }
        };

        transform.Find("FlyingBoost_1").GetComponent<Button_UI>().ClickFunc = () =>
        {
            if (!playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.FlyingBoost_1))
                playerSkills.TryUnlockSkill(PlayerSkills.SkillType.FlyingBoost_1);
            Debug.Log("FlyingBoost_1");
        };
        transform.Find("FlyingBoost_1").GetComponent<Button_UI>().MouseOverOnceFunc = () =>
        {
            if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.FlyingBoost_1) || playerSkills.CanUnlock(PlayerSkills.SkillType.FlyingBoost_1)) //Ist Skill sichtbar > dann Tooltip anzeigen
            {
                skillTooltip.transform.position = transform.Find("FlyingBoost_1").position + Vector3.up * 10;
                uiName.text = ConfigSkills.Instance.FlyingBoost_1_Name;
                uiDesc.text = ConfigSkills.Instance.FlyingBoost_1_Desc;
                uiStat1.text = (RisingEnvironment.Instance.flyingBoost).ToString();
                uiStat2.text = (RisingEnvironment.Instance.flyingBoost + ConfigSkills.Instance.FlyingBoost_1_Value).ToString();
                uiPrice.text = ConfigSkills.Instance.FlyingBoost_1_Price.ToString();
            }
        };
    }

    public void CloseSkillTree()
    {
        gameObject.transform.parent.gameObject.SetActive(false);
        CursorManager.HideCursor();
        Cursor.lockState = CursorLockMode.Locked;
        mouseLook.GetComponent<MouseLook>().enabled = true;   // MouseLook wieder an
    }

    public void SetPlayerSkills(PlayerSkills playerSkills) //Holt sich die aktulle Liste der Skills
    {
        this.playerSkills = playerSkills; //füllt die eigene Liste mit der playerSkill Liste aus dem Weapon Script
        playerSkills.OnSkillUnlocked += PlayerSkills_OnSkillUnlocked;
        UpdateVisuals();
    }

    private void PlayerSkills_OnSkillUnlocked(object sender, PlayerSkills.OnSkillUnlockedEventArgs e)
    {
        UpdateVisuals();
    }

    private void UpdateVisuals() //skill Farbe Ändern
    {
        foreach (PlayerSkills.SkillType skill in Enum.GetValues(typeof(PlayerSkills.SkillType)))
        {
            Transform uiSkill = transform.Find(skill.ToString());

            if (uiSkill == null)
            {
                Debug.LogWarning($"Kein UI-Element für Skill {skill} gefunden!");
                continue;
            }

            Transform imageTransform = uiSkill.Find("Image");

            if (imageTransform == null)
            {
                Debug.LogWarning($"Image für Skill {skill} nicht gefunden!");
                continue;
            }

            Image image = imageTransform.GetComponent<Image>();

            if (playerSkills.IsSkillUnlocked(skill))
            {
                // Skill bereits freigeschaltet
                uiSkill.gameObject.SetActive(true);
                image.material = null;
            }
            else if (playerSkills.CanUnlock(skill))
            {
                // Skill kann freigeschaltet werden
                uiSkill.gameObject.SetActive(true);
                image.material = skillUnlockedMaterial;
            }
            else
            {
                // Skill kann noch nicht freigeschaltet werden
                uiSkill.gameObject.SetActive(false);
            }
        }

        /*
        Transform uiSkill = transform.Find("DamageAdd_1");
        if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.DamageAdd_1)) //wenn freigeschalten dann keine Änderung
        {
            uiSkill.Find("Image").GetComponent<Image>().material = null;
        }
        else
        {
            if (playerSkills.CanUnlock(PlayerSkills.SkillType.DamageAdd_1))
            {
                uiSkill.gameObject.SetActive(true);
                uiSkill.Find("Image").GetComponent<Image>().material = skillUnlockedMaterial;
            }
            else
            {
                uiSkill.gameObject.SetActive(false);
            }
        }


        if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.AmmoAdd_1)) //wenn freigeschalten dann keine Änderung
        {
            transform.Find("AmmoAdd_1").Find("Image").GetComponent<Image>().material = null;
        }
        else
        {
            if (playerSkills.CanUnlock(PlayerSkills.SkillType.AmmoAdd_1))
            {
                transform.Find("AmmoAdd_1").Find("Image").GetComponent<Image>().material = skillUnlockedMaterial;
            }
            else
            {
                transform.Find("AmmoAdd_1").Find("Image").GetComponent<Image>().material = skillLockedMaterial;
            }
        }

        if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.DamageAdd_2)) //wenn freigeschalten dann keine Änderung
        {
            transform.Find("DamageAdd_2").Find("Image").GetComponent<Image>().material = null;
        }
        else
        {
            if (playerSkills.CanUnlock(PlayerSkills.SkillType.DamageAdd_2))
            {
                transform.Find("DamageAdd_2").Find("Image").GetComponent<Image>().material = skillUnlockedMaterial;
            }
            else
            {
                transform.Find("DamageAdd_2").Find("Image").GetComponent<Image>().material = skillLockedMaterial;
            }
        }

        if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.AmmoAdd_2)) //wenn freigeschalten dann keine Änderung
        {
            transform.Find("AmmoAdd_2").Find("Image").GetComponent<Image>().material = null;
        }
        else
        {
            if (playerSkills.CanUnlock(PlayerSkills.SkillType.AmmoAdd_2))
            {
                transform.Find("AmmoAdd_2").Find("Image").GetComponent<Image>().material = skillUnlockedMaterial;
            }
            else
            {
                transform.Find("AmmoAdd_2").Find("Image").GetComponent<Image>().material = skillLockedMaterial;
            }
        }

        if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.MagazineMulti_1)) //wenn freigeschalten dann keine Änderung
        {
            transform.Find("MagazineMulti_1").Find("Image").GetComponent<Image>().material = null;
        }
        else
        {
            if (playerSkills.CanUnlock(PlayerSkills.SkillType.MagazineMulti_1))
            {
                transform.Find("MagazineMulti_1").Find("Image").GetComponent<Image>().material = skillUnlockedMaterial;
            }
            else
            {
                transform.Find("MagazineMulti_1").Find("Image").GetComponent<Image>().material = skillLockedMaterial;
            }
        }

        if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.Reload_1)) //wenn freigeschalten dann keine Änderung
        {
            transform.Find("Reload_1").Find("Image").GetComponent<Image>().material = null;
        }
        else
        {
            if (playerSkills.CanUnlock(PlayerSkills.SkillType.Reload_1))
            {
                transform.Find("Reload_1").Find("Image").GetComponent<Image>().material = skillUnlockedMaterial;
            }
            else
            {
                transform.Find("Reload_1").Find("Image").GetComponent<Image>().material = skillLockedMaterial;
            }
        }

        if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.FullAuto)) //wenn freigeschalten dann keine Änderung
        {
            transform.Find("FullAuto").Find("Image").GetComponent<Image>().material = null;
        }
        else
        {
            if (playerSkills.CanUnlock(PlayerSkills.SkillType.FullAuto))
            {
                transform.Find("FullAuto").Find("Image").GetComponent<Image>().material = skillUnlockedMaterial;
            }
            else
            {
                transform.Find("FullAuto").Find("Image").GetComponent<Image>().material = skillLockedMaterial;
            }
        }

        if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.PiercingShot)) //wenn freigeschalten dann keine Änderung
        {
            transform.Find("PiercingShot").Find("Image").GetComponent<Image>().material = null;
        }
        else
        {
            if (playerSkills.CanUnlock(PlayerSkills.SkillType.PiercingShot))
            {
                transform.Find("PiercingShot").Find("Image").GetComponent<Image>().material = skillUnlockedMaterial;
            }
            else
            {
                transform.Find("PiercingShot").Find("Image").GetComponent<Image>().material = skillLockedMaterial;
            }
        }

        if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.Demon_1)) //wenn freigeschalten dann keine Änderung
        {
            transform.Find("Demon_1").Find("Image").GetComponent<Image>().material = null;
        }
        else
        {
            if (playerSkills.CanUnlock(PlayerSkills.SkillType.Demon_1))
            {
                transform.Find("Demon_1").Find("Image").GetComponent<Image>().material = skillUnlockedMaterial;
            }
            else
            {
                transform.Find("Demon_1").Find("Image").GetComponent<Image>().material = skillLockedMaterial;
            }
        }

        if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.Demon_2)) //wenn freigeschalten dann keine Änderung
        {
            transform.Find("Demon_2").Find("Image").GetComponent<Image>().material = null;
        }
        else
        {
            if (playerSkills.CanUnlock(PlayerSkills.SkillType.Demon_2))
            {
                transform.Find("Demon_2").Find("Image").GetComponent<Image>().material = skillUnlockedMaterial;
            }
            else
            {
                transform.Find("Demon_2").Find("Image").GetComponent<Image>().material = skillLockedMaterial;
            }
        }

        if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.BulletDemon_1)) //wenn freigeschalten dann keine Änderung
        {
            transform.Find("BulletDemon_1").Find("Image").GetComponent<Image>().material = null;
        }
        else
        {
            if (playerSkills.CanUnlock(PlayerSkills.SkillType.BulletDemon_1))
            {
                transform.Find("BulletDemon_1").Find("Image").GetComponent<Image>().material = skillUnlockedMaterial;
            }
            else
            {
                transform.Find("BulletDemon_1").Find("Image").GetComponent<Image>().material = skillLockedMaterial;
            }
        }

        if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.OrbitDemon_1)) //wenn freigeschalten dann keine Änderung
        {
            transform.Find("OrbitDemon_1").Find("Image").GetComponent<Image>().material = null;
        }
        else
        {
            if (playerSkills.CanUnlock(PlayerSkills.SkillType.OrbitDemon_1))
            {
                transform.Find("OrbitDemon_1").Find("Image").GetComponent<Image>().material = skillUnlockedMaterial;
            }
            else
            {
                transform.Find("OrbitDemon_1").Find("Image").GetComponent<Image>().material = skillLockedMaterial;
            }
        }

        if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.MoneyDemon_1)) //wenn freigeschalten dann keine Änderung
        {
            transform.Find("MoneyDemon_1").Find("Image").GetComponent<Image>().material = null;
        }
        else
        {
            if (playerSkills.CanUnlock(PlayerSkills.SkillType.MoneyDemon_1))
            {
                transform.Find("MoneyDemon_1").Find("Image").GetComponent<Image>().material = skillUnlockedMaterial;
            }
            else
            {
                transform.Find("MoneyDemon_1").Find("Image").GetComponent<Image>().material = skillLockedMaterial;
            }
        }

        if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.BulletDemon_2)) //wenn freigeschalten dann keine Änderung
        {
            transform.Find("BulletDemon_2").Find("Image").GetComponent<Image>().material = null;
        }
        else
        {
            if (playerSkills.CanUnlock(PlayerSkills.SkillType.BulletDemon_2))
            {
                transform.Find("BulletDemon_2").Find("Image").GetComponent<Image>().material = skillUnlockedMaterial;
            }
            else
            {
                transform.Find("BulletDemon_2").Find("Image").GetComponent<Image>().material = skillLockedMaterial;
            }
        }

        if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.MoneyDemon_2)) //wenn freigeschalten dann keine Änderung
        {
            transform.Find("MoneyDemon_2").Find("Image").GetComponent<Image>().material = null;
        }
        else
        {
            if (playerSkills.CanUnlock(PlayerSkills.SkillType.MoneyDemon_2))
            {
                transform.Find("MoneyDemon_2").Find("Image").GetComponent<Image>().material = skillUnlockedMaterial;
            }
            else
            {
                transform.Find("MoneyDemon_2").Find("Image").GetComponent<Image>().material = skillLockedMaterial;
            }
        }

        if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.DistanceBoost_1)) //wenn freigeschalten dann keine Änderung
        {
            transform.Find("DistanceBoost_1").Find("Image").GetComponent<Image>().material = null;
        }
        else
        {
            if (playerSkills.CanUnlock(PlayerSkills.SkillType.DistanceBoost_1))
            {
                transform.Find("DistanceBoost_1").Find("Image").GetComponent<Image>().material = skillUnlockedMaterial;
            }
            else
            {
                transform.Find("DistanceBoost_1").Find("Image").GetComponent<Image>().material = skillLockedMaterial;
            }
        }

        if (playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.FlyingBoost_1)) //wenn freigeschalten dann keine Änderung
        {
            transform.Find("FlyingBoost_1").Find("Image").GetComponent<Image>().material = null;
        }
        else
        {
            if (playerSkills.CanUnlock(PlayerSkills.SkillType.FlyingBoost_1))
            {
                transform.Find("FlyingBoost_1").Find("Image").GetComponent<Image>().material = skillUnlockedMaterial;
            }
            else
            {
                transform.Find("FlyingBoost_1").Find("Image").GetComponent<Image>().material = skillLockedMaterial;
            }
        }
        */

        //deactivate all links
        foreach (SkillUnlockPath skillUnlockPath in skillUnlockPathsArray)
        {
            foreach (Image linkImage in skillUnlockPath.linkImageArray)
            {
                linkImage.gameObject.SetActive(false);
            }
        }

        foreach (SkillUnlockPath skillUnlockPath in skillUnlockPathsArray)
        {
            if (playerSkills.CanUnlock(skillUnlockPath.skillType)) //Wenn kaufbar dann grau
            {
                foreach (Image linkImage in skillUnlockPath.linkImageArray)
                {
                    linkImage.gameObject.SetActive(true);
                    linkImage.color = new Color(.5f, .5f, .5f);
                }
            }

            if (playerSkills.IsSkillUnlocked(skillUnlockPath.skillType)) //Wenn gekauft dann weíß
            {
                //skill unlocked or can be unlocked
                foreach (Image linkImage in skillUnlockPath.linkImageArray)
                {
                    linkImage.color = Color.white;
                }
            }
        }
    }

    [System.Serializable]
    public class SkillUnlockPath
    {
        public PlayerSkills.SkillType skillType;
        public Image[] linkImageArray;
    }
}
