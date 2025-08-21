using UnityEngine;
[CreateAssetMenu(menuName = "Game/Consumable/HealthPotion")]
public class HealthPotion : SO_ConsumableMetaData
{
    public int HealAmount;
    public override bool Use(Player player)
    {
        player.health.Heal(HealAmount);
        return true;
    }
}