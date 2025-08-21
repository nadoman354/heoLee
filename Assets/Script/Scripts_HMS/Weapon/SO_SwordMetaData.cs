using UnityEngine;

[CreateAssetMenu(fileName = "MetaData_Sword", menuName = "Game/MetaData/WeaponType/Sword")]
public class SO_SwordMetaData : SO_WeaponMetaData
{
    [Space(20)]
    [Header("�� ���� ���� �Ķ���� �Դϴ�.")]
    public int MaxAtkCount;
    public GameObject[] AtkEffectList;

}
