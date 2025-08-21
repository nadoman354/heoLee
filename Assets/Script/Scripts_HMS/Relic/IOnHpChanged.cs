namespace InterfaceRelic
{
    public interface IOnHpChanged : IRelicHook
    {
        void OnHpChanged(Player player, in HealthChangeContext ctx);
    }
}