/*
 * [CombatResolver.cs]
 * 무엇: DamageRequest(입력) → DamageResult(출력) 계산기.
 *  - HP: 크리티컬 반영
 *  - 그로기: (무기 groggyPower + Σ(+)) × (1 + Σ(×%)) 규칙
 *  - ComputeGroggyDuration 유틸 제공
 *
 * 책임:
 *  - 밸런싱 로직의 단일 진입점, 규칙 중앙집중화.
 *
 * 사용법:
 *  - var result = CombatResolver.Resolve(req, attackerStats);
 *  - result.finalHpDamage / finalGroggyAdd를 대상에 적용.
 *  - 보스 그로기 지속시간 산정은 ComputeGroggyDuration 사용.
 */

using UnityEngine;

public struct DamageResult
{
    public float finalHpDamage;
    public float finalGroggyAdd;
    public bool  isCritical;
}