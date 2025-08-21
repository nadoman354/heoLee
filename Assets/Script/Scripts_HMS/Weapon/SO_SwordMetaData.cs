using UnityEngine;

[CreateAssetMenu(fileName = "MetaData_Sword", menuName = "Game/MetaData/WeaponType/Sword")]
public class SO_SwordMetaData : SO_WeaponMetaData
{
    [Space(20)]
    [Header("검 무기 전용 파라미터 입니다.")]
    public int MaxAtkCount;
    public GameObject[] AtkEffectList;

}
