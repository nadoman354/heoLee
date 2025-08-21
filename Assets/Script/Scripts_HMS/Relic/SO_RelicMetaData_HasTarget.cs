using UnityEngine;

[CreateAssetMenu(fileName = "Meta_Relic_Targeted", menuName = "Game/MetaData/RelicType/BaseRelic (Targeted)")]
public class SO_RelicMetaData_HasTarget : SO_RelicMetaData
{
    [Header("Target IWeaponLogic / Skill")]
    [SerializeField] private SO_WeaponMetaData targetWeapon; // 전용 대상 무기
    [SerializeField] private int targetSkillIndex = -1;      // -1이면 무기 전체, 0/1이면 해당 스킬

    public SO_WeaponMetaData TargetWeapon => targetWeapon;
    public int TargetSkillIndex => targetSkillIndex;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (targetWeapon == null)
            Debug.LogWarning($"{name}: TargetWeapon이 비어 있습니다.");
        if (targetSkillIndex < -1)
            targetSkillIndex = -1;
    }
#endif
}
