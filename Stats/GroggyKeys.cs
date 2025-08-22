/*
 * [BossBase.cs]
 * 무엇: 보스 전투 컨트롤러(HP + Groggy + 패턴 러너).
 *
 * 책임:
 *  - Health/GroggyMeter 초기화 및 상태 관리.
 *  - PatternRunner로 패턴 순환, 그로기 중에는 일시정지.
 *  - DamageRequest를 받아 CombatResolver로 계산 후 HP/그로기 적용.
 *
 * 사용법:
 *  - SO_BossStatData, BossPatternSet 연결.
 *  - 외부(무기/스킬)에서 ApplyHit(req, attackerStats) 호출.
 *  - 패턴 특수 발동이 필요하면 TriggerSpecialPattern 사용.
 */

namespace Stats
{
    internal class GroggyKeys
    {
    }
}