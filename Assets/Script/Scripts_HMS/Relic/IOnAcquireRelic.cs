using UnityEngine;

namespace InterfaceRelic
{
    public interface IOnAcquireRelic : IRelicHook
    {
        public void Invoke(Player player);
    }
}
