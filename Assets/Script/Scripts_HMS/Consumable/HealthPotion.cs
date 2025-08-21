using UnityEngine;
[CreateAssetMenu(menuName = "Game/Consumable/HealthPotion")]
public class HealthPotion : ConsumableData
{
    public int HealAmount;
    public override bool Use(Player player)
    {
        player.health.Heal(HealAmount);
        return true;
    }
}