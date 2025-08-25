/*
 Health.cs  -  �ܼ��ϰ� ������ HP �ý��� (MonoBehaviour �ƴ�)

 [��� �� üũ����Ʈ]
 1) �ݵ�� Init(...)�� ȣ���� MaxHp Ű�� ������ Regen Ű, ��å�� �����ϼ���.
    - ��:    Init(stats, Stats.BasicEnemyStat.MaxHp, Stats.BasicEnemyStat.HealthRegenPerSec, MaxChangePolicy.Clamp);
    - ����:  Init(stats, Stats.BasicBossStat.MaxHp, null, MaxChangePolicy.Clamp);
    - �÷��̾�: Init(stats, Stats.BasicPlayerStat.MaxHp, Stats.BasicPlayerStat.HealthRegenPerSec, MaxChangePolicy.KeepRatio);

 2) ���� �ֱ�
    - Awake �Ǵ� Start: health = new Health(); health.Init(...); health.OnDeath += OnDeath;
    - Update: health.Tick();  // ����� �ʿ��� ���� ȣ��
    - OnDestroy ��: health.Dispose();  // StatBlock �̺�Ʈ ����

 3) �������� ȸ��
    - ����: health.Damage(value);  // 0 �̸����� �������� 0���� Ŭ����, OnDeath�� �ѹ��� ȣ��
    - ȸ��: health.Heal(value);    // Max�� ���� ����. allowOverheal=true�� ���� ���� ����

 4) ���� ���� ����
    - StatBlock.OnStatChanged(string key)�� �����Ѵٰ� ����
    - MaxHp�� ���ϸ� ��å���� Current�� ����
      Clamp: ���� HP�� �� Max�� ���� Ŭ����
      KeepRatio: ����/��Max ������ ����
      KeepAbsolute: ���� HP ����, �� Max�� �ݿ��ϵ� ��ġ�� Ŭ����

 5) ��Ƽ�� �� ó��
    - ������Ʈ���� ���� ������ ��Ƽ���� -999�� ����Ѵٸ�, GetStatSafe�� fallback���� ��ü

 6) ����
    - �� Ÿ���� MonoBehaviour�� �ƴ�. SerializeField�� �������� ����. �ݵ�� Init(...) �Ķ���ͷ� ����
*/

using UnityEngine;

public enum MaxChangePolicy { Clamp, KeepRatio, KeepAbsolute }
/*
 MaxChangePolicy.Clamp �ּ��� ���� ����

 �� �� ���
 - Max�� �ٲ� �� ���� HP�� �״�� �ΰ�, �� Max�� ������ �� Max�θ� �߶� �����.

 ��Ģ
 - Current = min(Current, NewMax)

 ����
 - ���� 80, �� Max 60  => ��� 60  (��ģ ��ŭ �߸�)
 - ���� 40, �� Max 60  => ��� 40  (�״��)
 - ���� 30, �� Max 100 => ��� 30  (�״��)

 ���� ����
 - ����, ����ó�� Max�� �ٲ� ü�� �������� �ٲٰ� ���� ���� ��

 ����: �ٸ� ��å
 - KeepRatio   : ���� ����. ��) 80/100, �� Max 60 => 48
 - KeepAbsolute: ���� ������ Clamp�� ����. ��ġ�� �� Max�θ� �߸�
*/


public sealed class Health
{
    // Init���� ���ԵǴ� ������
    string _maxHpKey;                 // MaxHp�� ���� StatBlock Ű
    string _regenKey;                 // �ʴ� ȸ���� ���� Ű. null�̸� �̻��
    MaxChangePolicy _policy = MaxChangePolicy.Clamp;

    // �̺�Ʈ
    public event System.Action<float> OnDamaged;   // ������ ���� �縸 ����
    public event System.Action<float> OnHealed;    // ������ ȸ���� �縸 ����
    public event System.Action OnDeath;            // Current�� 0�� �Ǵ� ������ �ѹ��� ȣ��

    // ����
    public float Current { get; private set; }
    public float Max => GetStatSafe(_maxHpKey, 1f);
    public bool IsDead { get; private set; }

    StatBlock _stats;
    float _lastMax;                   // Max ���� �� �� ����

    // �ʱ�ȭ. �ݵ�� ȣ�� �ʿ�
    public void Init(StatBlock stats, string maxHpKey, string regenKey = null, MaxChangePolicy policy = MaxChangePolicy.Clamp)
    {
        _stats = stats;
        _maxHpKey = maxHpKey;
        _regenKey = regenKey;         // null�̸� Tick���� ȸ�� ����� �ǳʶ�
        _policy = policy;

        _lastMax = GetStatSafe(_maxHpKey, 1f);
        Current = _lastMax;           // ���� �� Ǯ�� ����
        IsDead = false;

        if (_stats != null) _stats.OnStatChanged += OnStatChanged;
    }

    // �̺�Ʈ ���� ����
    public void Dispose()
    {
        if (_stats != null) _stats.OnStatChanged -= OnStatChanged;
        _stats = null;
    }

    // ���� ����. 0 �̸� �Է��� ����. 0���� �������� OnDeath �ѹ��� ȣ��
    public void Damage(float raw)
    {
        if (IsDead) return;

        float before = Current;
        float amount = Mathf.Max(0f, raw);
        Current = Mathf.Max(0f, before - amount);

        float dealt = before - Current;
        if (dealt > 0f) OnDamaged?.Invoke(dealt);

        if (Current <= 0f && !IsDead)
        {
            Current = 0f;
            IsDead = true;
            OnDeath?.Invoke();
        }
    }

    // ȸ��. �⺻�� Max����. allowOverheal=true�� ���� ����
    public void Heal(float amount, bool allowOverheal = false)
    {
        if (amount <= 0f) return;

        float before = Current;
        float cap = allowOverheal ? float.PositiveInfinity : Max;
        Current = Mathf.Min(cap, before + amount);

        float healed = Current - before;
        if (healed > 0f) OnHealed?.Invoke(healed);
    }

    // StatBlock�� MaxHp Ű�� ������ �� ȣ��
    void OnStatChanged(string key)
    {
        if (string.IsNullOrEmpty(key) || key != _maxHpKey) return;

        float newMax = GetStatSafe(_maxHpKey, 1f);
        float oldMax = _lastMax;
        _lastMax = newMax;

        if (oldMax <= 0f)
        {
            Current = newMax;
            return;
        }

        // Max ���� ��å ����
        switch (_policy)
        {
            case MaxChangePolicy.KeepRatio:
                {
                    float ratio = Mathf.Clamp01(Current / oldMax);
                    Current = Mathf.Clamp(ratio * newMax, 0f, newMax);
                    break;
                }
            case MaxChangePolicy.KeepAbsolute:
            case MaxChangePolicy.Clamp:
            default:
                Current = Mathf.Min(Current, newMax);
                break;
        }

        // Max ��ȭ�� 0 ���ϰ� �Ǿ��� ���� ��� ó���� �ѹ���
        if (Current <= 0f && !IsDead)
        {
            Current = 0f;
            IsDead = true;
            OnDeath?.Invoke();
        }
    }

    // �� ������ ȣ�� ����. ��� Ű�� ���� ���� ȸ��
    public void Tick()
    {
        if (IsDead) return;

        float regenPerSec = GetStatSafe(_regenKey, 0f);
        if (regenPerSec > 0f && Current > 0f && Current < Max)
            Heal(regenPerSec * Time.deltaTime);
    }

    // ���� ��ƿ. ���� ���� ��Ƽ��(-999)�� fallback���� ��ü
    float GetStatSafe(string key, float fallback)
    {
        if (_stats == null || string.IsNullOrEmpty(key)) return fallback;
        float v = _stats.GetStat(key);
        if (Mathf.Approximately(v, -999f)) return fallback;
        return v;
    }
}
