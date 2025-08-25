using UnityEngine;
using System;
using System.Collections.Generic;
using Core.Interfaces;
// using Core.Interfaces;  // 실제 네임스페이스 구조에 맞게 조정

/// <summary>
/// 피격자에 부착되는 컴포넌트.
/// - 속성 게이지/발현/배율 계산
/// - 크리/온히트 슬로우 반영
/// - 최종 데미지를 같은 오브젝트의 IDamageable로 전달
/// </summary>
[DisallowMultipleComponent]
public class AttributeDamageHandler : MonoBehaviour, IAttributeDamageable
{
    [Header("Common")]
    [SerializeField] private float baseDamageMultiplier = 1f; // 피격자 공통 피해 보정

    // 속성 설정/상태(간단히 표기)
    private readonly Dictionary<AttributeType, AttributeConfig> _cfg = new();
    private readonly Dictionary<AttributeType, AttributeStatus> _status = new();

    //최종 피해 전달 델리게이트는 float 하나만
    private Action<float> _applyFinal;

    void Awake()
    {
        // (설정 테이블 구성/초기화 로직은 프로젝트 코드에 맞게)
        // ...

        // 같은 오브젝트의 IDamageable로 최종 피해 전달(플레이어/몬스터/보스 공통)
        if (TryGetComponent<IDamageable>(out var any))
            _applyFinal = dmg => any.TakeDamage(dmg); //시그니처 맞춤
    }

    // (호환용) 예전 시그니처
    public void ApplyAttributeDamage(AttributeType type, float baseDamage, float gaugeAmount, IAttributeModifierProvider provider = null)
    {
        var req = new DamageRequest
        {
            baseDamage = baseDamage,
            type = type,
            gaugeGain = gaugeAmount,
            provider = provider,
            attackerTeam = AttackerTeam.Neutral,
            bypassAttributes = false,

            // 크리/온히트 기본값
            canCrit = false,
            critChance = 0f,
            critMultiplier = 1f,
            onHitSlowAmount = 0f,
            onHitSlowDuration = 0f
        };
        ApplyAttributeDamage(in req);
    }

    /// <summary>
    /// 표준 경로: DamageRequest 기반 처리(크리/온히트 슬로우/게이지/발현 포함)
    /// </summary>
    public void ApplyAttributeDamage(in DamageRequest req)
    {
        if (_applyFinal == null) return;

        // 1) 공격자 수정치(유물/버프/장비) 조회
        AttributeMods mods = (req.provider != null) ? req.provider.GetAttributeMods(req.type) : AttributeMods.Identity;

        // 2) 기본 피해
        float final = Mathf.Max(0f, req.baseDamage)
                    * Mathf.Max(0f, mods.baseDamageMul)
                    * Mathf.Max(0f, baseDamageMultiplier);

        // 2-1) 크리티컬: 무기 기본 + 유물 가산 / 무기 배율 × 유물 배율
        if (req.canCrit && req.critMultiplier > 1f)
        {
            float chance = Mathf.Clamp01(req.critChance + Mathf.Max(0f, mods.critChanceAdd));
            float cmul = Mathf.Max(1f, req.critMultiplier * Mathf.Max(0.0001f, mods.critMultiplierMul));
            if (UnityEngine.Random.value < chance)
                final *= cmul;
        }

        bool triggered = false;

        // 3) 속성/게이지(트루 데미지/무속성은 스킵)
        if (!req.bypassAttributes && req.type != AttributeType.None
            && _status.TryGetValue(req.type, out var s) && _cfg.TryGetValue(req.type, out var cfg))
        {
            float adjGauge = req.gaugeGain * Mathf.Max(0f, mods.gaugeGainMul);
            if (adjGauge > 0f) s.AddGauge(adjGauge);  // 주의: AttributeStatus가 struct면 참조 방식 확인

            if (s.IsActivated)
            {
                final *= Mathf.Max(0f, cfg.activatedDamageMultiplier)
                       * Mathf.Max(0f, mods.activatedDamageMul);

                s.ResetGauge();
                triggered = true;

                // 발현 효과 전달(예: DOT/슬로우/공속감소)
                if (TryGetComponent<StatusEffectManager>(out var sem))
                    sem.ApplyAttributeEffect(req.type, cfg.effectName, mods);
            }
        }

        // 4) 온히트 슬로우(플레이어가 때릴 때만)
        if (req.attackerTeam == AttackerTeam.Player
            && req.onHitSlowAmount > 0f && req.onHitSlowDuration > 0f)
        {
            if (TryGetComponent<StatusEffectManager>(out var sem))
                sem.ApplyOnHitSlow(req.onHitSlowAmount, req.onHitSlowDuration);
        }

        // 5) 최종 피해 전달 (IDamageable.TakeDamage(float) 규격)
        _applyFinal(final);

        // 필요하다면 triggered/type을 이벤트로 내보내 UI/로그에 쓰세요.
        // OnAttributeTriggered?.Invoke(req.type, triggered);
    }
}
