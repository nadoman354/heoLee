using UnityEngine;
using enums;

[CreateAssetMenu(fileName = "MetaData_Relic", menuName = "Game/MetaData/RelicType/Base")]
public class SO_RelicMetaData : ScriptableObject
{
    public string id;
    [Space(10)]
    public Sprite sprite;
    public string name;
    public string description;
    public RarityType rarity;
    public SO_StatData stat;
    
    [Space(10)]
    [TypeRef(typeof(BaseRelic))]
    [SerializeField] private string _className;
    public string className => _className;
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!FactoryTypeResolver.TryResolveType(_className, typeof(BaseRelic), out var _))
            Debug.LogError($"{name}: LogicTypeName '{_className}' is invalid or not BaseRelic.");
    }
#endif
}
