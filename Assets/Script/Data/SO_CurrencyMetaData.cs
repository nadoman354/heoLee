using UnityEngine;

[CreateAssetMenu(fileName = "MetaData_Currency", menuName = "Game/MetaData/Currency")]
public class SO_CurrencyMetaData : ScriptableObject
{
    public string id;                 // 예: "MagicStone"
    public Sprite sprite;
    public string name;
    [TextArea] public string description;

    [Min(1)] public int amountPerPickup = 1; // 줍자마자 지급되는 수량
}