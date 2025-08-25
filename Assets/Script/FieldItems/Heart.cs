using UnityEngine;

[CreateAssetMenu(menuName = "Game/FieldItem/Heart")]
public class Heart : SO_FieldItemMetaData
{
    int HealAmount;

    public override bool Apply(Player player)
    {
        if(player == null || player.health.Current >= player.health.Max)
        {
            return false;
        }

        player.health.Heal(HealAmount);

        return true;
    }
}
