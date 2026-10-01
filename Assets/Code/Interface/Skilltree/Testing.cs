using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private WeaponController weapon;
    [SerializeField] private SkillManager skill;
    [SerializeField] private UI_SkillTree uiSkillTree;

    private void Start()
    {
        uiSkillTree.SetPlayerSkills(skill.GetPlayerSkills());
    }

}
