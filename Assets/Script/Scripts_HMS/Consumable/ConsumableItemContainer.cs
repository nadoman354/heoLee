using System;
using UnityEngine;


public sealed class ConsumableItemContainer : IConsumableItemContainer
{
    public int Capacity { get; }
    private readonly IConsumableItem[] _slots;
    private Player _player;

    /// <summary>UI 갱신/세이브 트리거용: (slotIndex, newItem)</summary>
    public event Action<int, IConsumableItem> OnSlotChanged;

    public ConsumableItemContainer(int capacity = 4)
    {
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
        Capacity = capacity;
        _slots = new IConsumableItem[capacity];
    }

    public void Init(Player p) => _player = p ?? throw new ArgumentNullException(nameof(p));

    // ---- 조회 ----
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

    // ---- 추가 ----
    public bool TryAdd(IConsumableItem item, out int placedIndex)
    {
        placedIndex = -1;
        if (item == null) return false;

        // (옵션) 스택 병합 자리 찾기:
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
        return false; // 가득 참
    }

    // ---- 사용 ----
    public bool TryUse(int index)
    {
        if (!InRange(index) || _slots[index] == null || _player == null) return false;

        bool shouldRemove = _slots[index].Use(_player);
        if (shouldRemove)
        {
            _slots[index] = null;
            OnSlotChanged?.Invoke(index, null);
            CompactLeft(index); // 사용해 빈칸 생긴 지점부터 압축
        }
        return true;
    }

    // ---- 제거 ----
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

    // ---- 드롭(월드 스폰은 상층에서) ----
    public bool TryDrop(int index, out IConsumableItem dropped)
    {
        // 슬롯에서 빼서 반환만 함. 실제 프리팹 스폰/월드 배치는 인벤토리/게임월드가 담당.
        return TryRemove(index, out dropped);
    }

    // ---- 내부 유틸 ----
    private bool InRange(int i) => (uint)i < (uint)Capacity;

    /// <summary>왼쪽 압축: null을 오른쪽으로 몰아주며 상대적 순서 유지(O(n))</summary>
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

    // (옵션) 스택 병합 로직 예시
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
    //     // incoming.Stack이 남으면 TryAdd로 이어서 빈 슬롯에 넣기
    // }
}
