using UnityEngine;
namespace InterfaceRelic
{
    public interface IOnRemoveRelic : IRelicHook
    {
        public void Invoke(Player player);
    }
}