using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ƽ ��ȭ ����.
/// - Earn(id, amount): ȹ��
/// - TrySpend(id, amount): �Һ�(�����ϸ� false)
/// - GetAmount(id): ������ ��ȸ
/// - CanAfford(id, amount): ���� ���� ����
/// - �̺�Ʈ: OnChanged(id, old, @new)
/// </summary>
public class CurrencyBank : MonoBehaviour
{
    public static CurrencyBank I { get; private set; }

    // ��ȭ ������(���� ����/�Һ� ���)
    [Serializable] public struct Stack { public string id; public int amount; }

    // ���� ����
    private readonly Dictionary<string, int> wallet = new();

    // ���� �̺�Ʈ: (id, old, new)
    public event Action<string, int, int> OnChanged;

    [Header("�ʱ� ����(����)")]
    [SerializeField] private List<Stack> initialBalances = new();

    private void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this; DontDestroyOnLoad(gameObject);

        // �ʱ� ����
        for (int i = 0; i < initialBalances.Count; i++)
        {
            var s = initialBalances[i];
            if (string.IsNullOrEmpty(s.id) || s.amount <= 0) continue;
            SetAmountInternal(s.id, s.amount, invokeEvent: false);
        }
    }

    // ===== Public API =====

    /// <summary>��ȭ ȹ��(�÷���). amount>0�� �ǹ� ����.</summary>
    public void Earn(string id, int amount)
    {
        if (string.IsNullOrEmpty(id) || amount <= 0) return;
        int old = GetAmount(id);
        int @new = old + amount;
        SetAmountInternal(id, @new, invokeEvent: true);
    }

    /// <summary>��ȭ �Һ�(���̳ʽ�). �����ϸ� false.</summary>
    public bool TrySpend(string id, int amount)
    {
        if (string.IsNullOrEmpty(id) || amount <= 0) return false;
        int old = GetAmount(id);
        if (old < amount) return false;
        int @new = old - amount;
        SetAmountInternal(id, @new, invokeEvent: true);
        return true;
    }

    /// <summary>������ ��ȸ(������ 0).</summary>
    public int GetAmount(string id)
        => (string.IsNullOrEmpty(id) ? 0 : (wallet.TryGetValue(id, out var v) ? v : 0));

    /// <summary>���� ����?</summary>
    public bool CanAfford(string id, int amount)
        => amount <= 0 || GetAmount(id) >= amount;

    /// <summary>�����/ġƮ�� ���� ����.</summary>
    public void SetAmount(string id, int value)
    {
        if (string.IsNullOrEmpty(id)) return;
        if (value < 0) value = 0;
        SetAmountInternal(id, value, invokeEvent: true);
    }

    // ----- ����(���� ��ȭ ����) -----

    /// <summary>���� ��ȭ�� �� ���� ȹ��.</summary>
    public void Earn(IEnumerable<Stack> bundle)
    {
        if (bundle == null) return;
        foreach (var s in bundle) Earn(s.id, s.amount);
    }

    /// <summary>
    /// ���� ��ȭ�� ���������� �Һ�(���� �����ϸ� ����, �ƴϸ� �ƹ��͵� �� ��).
    /// </summary>
    public bool TrySpend(IEnumerable<Stack> cost)
    {
        if (cost == null) return true;

        // ������ + ����
        var buffer = new List<Stack>();
        foreach (var s in cost)
        {
            if (string.IsNullOrEmpty(s.id) || s.amount <= 0) continue;
            buffer.Add(s);
            if (!CanAfford(s.id, s.amount)) return false;
        }

        // ����
        for (int i = 0; i < buffer.Count; i++)
        {
            var s = buffer[i];
            SetAmountInternal(s.id, GetAmount(s.id) - s.amount, invokeEvent: true);
        }
        return true;
    }

    // ===== Internal =====

    private void SetAmountInternal(string id, int value, bool invokeEvent)
    {
        int old = GetAmount(id);
        wallet[id] = value;
        if (invokeEvent) OnChanged?.Invoke(id, old, value);
        // Debug.Log($"[CurrencyBank] {id}: {old} -> {value}");
    }
}
