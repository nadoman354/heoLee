using Core.Interfaces;
using System;
using UnityEngine;

/// <summary>
/// 상태이상/버프 총괄(피격자 측). 온히트 슬로우 및 기타 효과 적용.
/// </summary>
[DisallowMultipleComponent]
public class StatusEffectManager : MonoBehaviour
{
    private float currentMoveSlow;  // 0~1, 누적 슬로우 강도
    private float slowEndTime;      // 슬로우 종료 시각(Time.time)

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

    // --- 여기부터 변경 ---
    /// <summary>
    /// 온히트 슬로우 적용(플레이어가 때렸을 때 AttributeDamageHandler에서 호출).
    /// 강도는 누적 후 캡, 지속은 리프레시(최대값 유지).
    /// </summary>
    public void ApplyOnHitSlow(float addAmount, float duration)
    {
        float cap = GetSlowCap();
        currentMoveSlow = Mathf.Clamp(currentMoveSlow + addAmount, 0f, cap);
        slowEndTime = Mathf.Max(slowEndTime, Time.time + Mathf.Max(0f, duration));
    }
    // --- 여기까지 변경 ---

    /// <summary>현재 이동속도 배율(1=정상)</summary>
    public float GetMoveSpeedMultiplier() => 1f - currentMoveSlow;

    // (선택) 발현 효과용
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

            case AttributeType.Fire: // 필요 시 화상(별도 DOT) 등 구현
                break;

                // 속성 발현 효과(DOT/슬로우/공속감소 등)를 프로젝트 룰에 맞게 처리
        }
    }


    //발현 효과
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
