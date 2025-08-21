using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MetaData_Weapon", menuName = "Game/MetaData/WeaponType/Base")]
public class SO_WeaponMetaData : ScriptableObject
{
    public string id;
    [Space(10)]
    public Sprite sprite;
    public string name;
    public string description;
    public SO_StatData stat;
    [Space(10)]
    public AnimatorOverrideController attackClipOverride;
    public AnimatorOverrideController spriteClipOverride;
    public float zRotationOffset = 0f;
    public SO_SkillMetaData[] skillData = new SO_SkillMetaData[2];
    [Space(10)]
    [TypeRef(typeof(IWeaponLogic))]
    [SerializeField] private string _className;
    public string className => _className;
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!FactoryTypeResolver.TryResolveType(_className, typeof(IWeaponLogic), out var _))
            Debug.LogError($"{name}: LogicTypeName '{_className}' is invalid or not IWeaponLogic.");
    }
#endif
}
