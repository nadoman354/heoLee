using UnityEngine;
namespace InterfaceRelic
{
    public interface IUpdatableRelic : IRelicHook
    {
        public void Invoke(Player player);
    }
}