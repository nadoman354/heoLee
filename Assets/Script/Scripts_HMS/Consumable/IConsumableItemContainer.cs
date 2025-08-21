using System;
using UnityEngine;

public interface IConsumableItemContainer
{
    void Init(Player p);
    bool IsFull();
    bool IsEmpty();
    bool TryAdd(IConsumableItem item, out int placedIndex);
    bool TryUse(int index);
    bool TryRemove(int index, out IConsumableItem removed);

    event Action<int, IConsumableItem> OnSlotChanged;
}