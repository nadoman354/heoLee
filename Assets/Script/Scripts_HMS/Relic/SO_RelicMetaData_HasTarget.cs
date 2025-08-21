using UnityEngine;

[CreateAssetMenu(fileName = "Meta_Relic_Targeted", menuName = "Game/MetaData/RelicType/BaseRelic (Targeted)")]
public class SO_RelicMetaData_HasTarget : SO_RelicMetaData
{
    [Header("Target IWeaponLogic / Skill")]
    [SerializeField] private SO_WeaponMetaData targetWeapon; // ���� ��� ����
    [SerializeField] private int targetSkillIndex = -1;      // -1�̸� ���� ��ü, 0/1�̸� �ش� ��ų

    public SO_WeaponMetaData TargetWeapon => targetWeapon;
    public int TargetSkillIndex => targetSkillIndex;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (targetWeapon == null)
            Debug.LogWarning($"{name}: TargetWeapon�� ��� �ֽ��ϴ�.");
        if (targetSkillIndex < -1)
            targetSkillIndex = -1;
    }
#endif
}
