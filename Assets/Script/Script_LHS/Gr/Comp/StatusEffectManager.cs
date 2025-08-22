using Core.Interfaces;
using System;
using UnityEngine;

/// <summary>
/// �����̻�/���� �Ѱ�(�ǰ��� ��). ����Ʈ ���ο� �� ��Ÿ ȿ�� ����.
/// </summary>
[DisallowMultipleComponent]
public class StatusEffectManager : MonoBehaviour
{
    private float currentMoveSlow;  // 0~1, ���� ���ο� ����
    private float slowEndTime;      // ���ο� ���� �ð�(Time.time)

    private const float PLAYER_SLOW_CAP = 0.30f;
    private const float ENEMY_SLOW_CAP = 0.50f;
    private const float BOSS_SLOW_CAP = 0.60f;

    float GetSlowCap()
    {
        if (TryGetComponent<BossBase>(out _)) return BOSS_SLOW_CAP;
        //if (TryGetComponent<PlayerDamageReceiver>(out _)) return PLAYER_SLOW_CAP;
        return ENEMY_SLOW_CAP;
    }

    void Update()
    {
        if (currentMoveSlow > 0f && Time.time >= slowEndTime)
        {
            currentMoveSlow = 0f;
            slowEndTime = 0f;
        }
    }

    // --- ������� ���� ---
    /// <summary>
    /// ����Ʈ ���ο� ����(�÷��̾ ������ �� AttributeDamageHandler���� ȣ��).
    /// ������ ���� �� ĸ, ������ ��������(�ִ밪 ����).
    /// </summary>
    public void ApplyOnHitSlow(float addAmount, float duration)
    {
        float cap = GetSlowCap();
        currentMoveSlow = Mathf.Clamp(currentMoveSlow + addAmount, 0f, cap);
        slowEndTime = Mathf.Max(slowEndTime, Time.time + Mathf.Max(0f, duration));
    }
    // --- ������� ���� ---

    /// <summary>���� �̵��ӵ� ����(1=����)</summary>
    public float GetMoveSpeedMultiplier() => 1f - currentMoveSlow;

    // (����) ���� ȿ����
    public void ApplyAttributeEffect(AttributeType type, string effectName, AttributeMods mods)
    {
        switch (type)
        {
            case AttributeType.Poison:
                ApplyPoison(mods.dotDpsAdd, mods.dotDurAdd);
                break;

            case AttributeType.Ice:
                ApplyMoveSlow(mods.moveSlowAdd, mods.moveSlowDurAdd);
                break;

            case AttributeType.Bleed:
                ApplyAttackSpeedSlow(mods.atkSlowAdd, mods.atkSlowDurAdd);
                break;

            case AttributeType.Fire: // �ʿ� �� ȭ��(���� DOT) �� ����
                break;

                // �Ӽ� ���� ȿ��(DOT/���ο�/���Ӱ��� ��)�� ������Ʈ �꿡 �°� ó��
        }
    }


    //���� ȿ��
    private void ApplyAttackSpeedSlow(float atkSlowAdd, float atkSlowDurAdd)
    {
        throw new NotImplementedException();
    }

    private void ApplyMoveSlow(float moveSlowAdd, float moveSlowDurAdd)
    {
        throw new NotImplementedException();
    }

    private void ApplyPoison(float dotDpsAdd, float dotDurAdd)
    {
        throw new NotImplementedException();
    }
}
