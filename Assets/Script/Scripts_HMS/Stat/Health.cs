using UnityEngine;

public enum MaxChangePolicy { Clamp, KeepRatio, KeepAbsolute }

public sealed class Health
{
    [SerializeField] string maxHpKey = Stats.BasicPlayerStat.MaxHp;
    [SerializeField] MaxChangePolicy onMaxChange = MaxChangePolicy.Clamp;

    public event System.Action<float> OnDamaged;
    public event System.Action<float> OnHealed;
    public event System.Action OnDeath;
    public float Current { get; private set; }

    StatBlock _stats;

    public void Init(StatBlock stats)
    {
        _stats = stats;
        Current = Max;                          // ���� �� Ǯ�� ����
        _stats.OnStatChanged += OnStatChanged;  // Key�� �̺�Ʈ�� �ִٸ� ����
    }

    public float Max => _stats.GetStat(maxHpKey);

    public void Damage(float raw)
    {
        if (Current <= 0) return;
        // (����/���/���� ���� ���� DamagePipeline���� ��� �� �� ���� ����)
        float before = Current;
        Current = Mathf.Max(0, before - raw);
        OnDamaged?.Invoke(before - Current);
        if (Current <= 0) OnDeath?.Invoke();
    }

    public void Heal(float amount, bool allowOverheal = false)
    {
        float before = Current;
        float cap = allowOverheal ? float.PositiveInfinity : Max;
        Current = Mathf.Min(cap, before + amount);
        OnHealed?.Invoke(Current - before);
    }

    void OnStatChanged(string key)
    {
        if (key != maxHpKey) return;
        float oldMax = Mathf.Max(Max, 0.0001f); // ȣ�� ���� ���� �ʿ��ϸ� ĳ�÷� ���޹޵��� ���� ����
        // ��å ����
        switch (onMaxChange)
        {
            case MaxChangePolicy.KeepRatio:
                float ratio = Current / oldMax;
                Current = Mathf.Clamp(ratio * Max, 0, Max);
                break;
            case MaxChangePolicy.KeepAbsolute:
                Current = Mathf.Min(Current, Max);
                break;
            case MaxChangePolicy.Clamp:
            default:
                Current = Mathf.Min(Current, Max);
                break;
        }
    }

    public void Update()
    {
        // ���(�ʴ� ȸ��) ��: RegenPerSec Ű ���
        float regen = _stats.GetStat("HealthRegenPerSec");
        if (regen != -999)
        {
            if (regen > 0 && Current > 0 && Current < Max)
                Heal((float)regen * Time.deltaTime);
        }
    }
}
