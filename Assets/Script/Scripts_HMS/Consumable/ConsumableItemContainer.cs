using System;
using UnityEngine;


public sealed class ConsumableItemContainer : IConsumableItemContainer
{
    public int Capacity { get; }
    private readonly IConsumableItem[] _slots;
    private Player _player;

    /// <summary>UI ����/���̺� Ʈ���ſ�: (slotIndex, newItem)</summary>
    public event Action<int, IConsumableItem> OnSlotChanged;

    public ConsumableItemContainer(int capacity = 4)
    {
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
        Capacity = capacity;
        _slots = new IConsumableItem[capacity];
    }

    public void Init(Player p) => _player = p ?? throw new ArgumentNullException(nameof(p));

    // ---- ��ȸ ----
    public IConsumableItem GetSlot(int index) => InRange(index) ? _slots[index] : null;
    public bool IsFull()
    {
        for (int i = 0; i < Capacity; i++) if (_slots[i] == null) return false;
        return true;
    }
    public bool IsEmpty()
    {
        for (int i = 0; i < Capacity; i++) if (_slots[i] != null) return false;
        return true;
    }

    // ---- �߰� ----
    public bool TryAdd(IConsumableItem item, out int placedIndex)
    {
        placedIndex = -1;
        if (item == null) return false;

        // (�ɼ�) ���� ���� �ڸ� ã��:
        // int stackIdx = FindStackableIndex(item);
        // if (stackIdx >= 0) { MergeStack(stackIdx, item); placedIndex = stackIdx; OnSlotChanged?.Invoke(stackIdx, _slots[stackIdx]); return true; }

        for (int i = 0; i < Capacity; i++)
        {
            if (_slots[i] == null)
            {
                _slots[i] = item;
                placedIndex = i;
                OnSlotChanged?.Invoke(i, item);
                return true;
            }
        }
        return false; // ���� ��
    }

    // ---- ��� ----
    public bool TryUse(int index)
    {
        if (!InRange(index) || _slots[index] == null || _player == null) return false;

        bool shouldRemove = _slots[index].Use(_player);
        if (shouldRemove)
        {
            _slots[index] = null;
            OnSlotChanged?.Invoke(index, null);
            CompactLeft(index); // ����� ��ĭ ���� �������� ����
        }
        return true;
    }

    // ---- ���� ----
    public bool TryRemove(int index, out IConsumableItem removed)
    {
        removed = null;
        if (!InRange(index) || _slots[index] == null) return false;

        removed = _slots[index];
        _slots[index] = null;
        OnSlotChanged?.Invoke(index, null);
        CompactLeft(index);
        return true;
    }

    // ---- ���(���� ������ ��������) ----
    public bool TryDrop(int index, out IConsumableItem dropped)
    {
        // ���Կ��� ���� ��ȯ�� ��. ���� ������ ����/���� ��ġ�� �κ��丮/���ӿ��尡 ���.
        return TryRemove(index, out dropped);
    }

    // ---- ���� ��ƿ ----
    private bool InRange(int i) => (uint)i < (uint)Capacity;

    /// <summary>���� ����: null�� ���������� �����ָ� ����� ���� ����(O(n))</summary>
    private void CompactLeft(int start)
    {
        int write = Math.Max(0, Math.Min(start, Capacity - 1));
        for (int read = write; read < Capacity; read++)
        {
            var it = _slots[read];
            if (it != null)
            {
                if (read != write)
                {
                    _slots[write] = it;
                    _slots[read] = null;
                    OnSlotChanged?.Invoke(write, it);
                    OnSlotChanged?.Invoke(read, null);
                }
                write++;
            }
        }
    }

    // (�ɼ�) ���� ���� ���� ����
    // private int FindStackableIndex(IConsumableItem item)
    // {
    //     if (!item.IsStackable) return -1;
    //     for (int i = 0; i < Capacity; i++)
    //     {
    //         var cur = _slots[i];
    //         if (cur != null && cur.IsStackable && cur.Id == item.Id && cur.Stack < cur.MaxStack)
    //             return i;
    //     }
    //     return -1;
    // }
    // private void MergeStack(int idx, IConsumableItem incoming)
    // {
    //     var cur = _slots[idx];
    //     int canAdd = Math.Min(incoming.Stack, cur.MaxStack - cur.Stack);
    //     cur.AddStack(canAdd);
    //     incoming.AddStack(-canAdd);
    //     // incoming.Stack�� ������ TryAdd�� �̾ �� ���Կ� �ֱ�
    // }
}
