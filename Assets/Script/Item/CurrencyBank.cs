using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 멀티 재화 은행.
/// - Earn(id, amount): 획득
/// - TrySpend(id, amount): 소비(부족하면 false)
/// - GetAmount(id): 보유량 조회
/// - CanAfford(id, amount): 구매 가능 여부
/// - 이벤트: OnChanged(id, old, @new)
/// </summary>
public class CurrencyBank : MonoBehaviour
{
    public static CurrencyBank I { get; private set; }

    // 재화 묶음용(번들 지급/소비에 사용)
    [Serializable] public struct Stack { public string id; public int amount; }

    // 내부 지갑
    private readonly Dictionary<string, int> wallet = new();

    // 변경 이벤트: (id, old, new)
    public event Action<string, int, int> OnChanged;

    [Header("초기 보유(선택)")]
    [SerializeField] private List<Stack> initialBalances = new();

    private void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this; DontDestroyOnLoad(gameObject);

        // 초기 세팅
        for (int i = 0; i < initialBalances.Count; i++)
        {
            var s = initialBalances[i];
            if (string.IsNullOrEmpty(s.id) || s.amount <= 0) continue;
            SetAmountInternal(s.id, s.amount, invokeEvent: false);
        }
    }

    // ===== Public API =====

    /// <summary>재화 획득(플러스). amount>0만 의미 있음.</summary>
    public void Earn(string id, int amount)
    {
        if (string.IsNullOrEmpty(id) || amount <= 0) return;
        int old = GetAmount(id);
        int @new = old + amount;
        SetAmountInternal(id, @new, invokeEvent: true);
    }

    /// <summary>재화 소비(마이너스). 부족하면 false.</summary>
    public bool TrySpend(string id, int amount)
    {
        if (string.IsNullOrEmpty(id) || amount <= 0) return false;
        int old = GetAmount(id);
        if (old < amount) return false;
        int @new = old - amount;
        SetAmountInternal(id, @new, invokeEvent: true);
        return true;
    }

    /// <summary>보유량 조회(없으면 0).</summary>
    public int GetAmount(string id)
        => (string.IsNullOrEmpty(id) ? 0 : (wallet.TryGetValue(id, out var v) ? v : 0));

    /// <summary>구매 가능?</summary>
    public bool CanAfford(string id, int amount)
        => amount <= 0 || GetAmount(id) >= amount;

    /// <summary>디버그/치트용 강제 설정.</summary>
    public void SetAmount(string id, int value)
    {
        if (string.IsNullOrEmpty(id)) return;
        if (value < 0) value = 0;
        SetAmountInternal(id, value, invokeEvent: true);
    }

    // ----- 번들(여러 재화 동시) -----

    /// <summary>여러 재화를 한 번에 획득.</summary>
    public void Earn(IEnumerable<Stack> bundle)
    {
        if (bundle == null) return;
        foreach (var s in bundle) Earn(s.id, s.amount);
    }

    /// <summary>
    /// 여러 재화를 원자적으로 소비(전부 가능하면 차감, 아니면 아무것도 안 함).
    /// </summary>
    public bool TrySpend(IEnumerable<Stack> cost)
    {
        if (cost == null) return true;

        // 스냅샷 + 검증
        var buffer = new List<Stack>();
        foreach (var s in cost)
        {
            if (string.IsNullOrEmpty(s.id) || s.amount <= 0) continue;
            buffer.Add(s);
            if (!CanAfford(s.id, s.amount)) return false;
        }

        // 차감
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
