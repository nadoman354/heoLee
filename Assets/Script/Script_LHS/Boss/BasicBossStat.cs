using UnityEngine;
namespace Stats
{
    public class BasicBossStat : MonoBehaviour
    {
        // 전투 공통
        public static readonly string MaxHp = "Boss.MaxHp";
        public static readonly string Damage = "Boss.Damage";
        public static readonly string MoveSpeed = "Boss.MoveSpeed";
        public static readonly string HealthRegenPerSec = "Boss.HealthRegenPerSec";

        // 그로기(게이지/유물 규칙: + 먼저, 그 다음 ×)
        public static readonly string GroggyMax = "Boss.GroggyMax";          // 게이지 최대치(필요 시)
        public static readonly string GroggyBaseDuration = "Boss.GroggyBaseDuration"; // 기본 지속시간

        // 유물/버프 가산(+)
        public static readonly string GroggyGainAdd = "Boss.GroggyGainAdd";
        public static readonly string GroggyDurationAdd = "Boss.GroggyDurationAdd";

        // 유물/버프 곱가산(×, %들을 더해 1+합으로 마지막에 곱)
        public static readonly string GroggyGainMulSum = "Boss.GroggyGainMulSum";
        public static readonly string GroggyDurationMulSum = "Boss.GroggyDurationMulSum";
    }
}