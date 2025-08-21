using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MetaData_Skill", menuName = "Game/MetaData/SkillType/Base")]
public class SO_SkillMetaData : ScriptableObject
{
    public string id;
    [Space(10)]
    public Sprite sprite;
    public string name;
    public string description;
    public SO_StatData stat;
    [Space(10)]
    [TypeRef(typeof(ISkillLogic))]
    [SerializeField] private string _className;
    public string className => _className;
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!FactoryTypeResolver.TryResolveType(_className, typeof(ISkillLogic), out var _))
            Debug.LogError($"{name}: LogicTypeName '{_className}' is invalid or not ISkillLogic.");
    }
#endif
}