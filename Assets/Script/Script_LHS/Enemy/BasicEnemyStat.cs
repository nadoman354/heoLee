using UnityEngine;

namespace Stats
{
    public class BasicEnemyStat : MonoBehaviour
    {
        public const string MaxHp = "Enemy.MaxHp";
        public const string Damage = "Enemy.Damage";
        public const string MoveSpeed = "Enemy.MoveSpeed";
        public const string HealthRegenPerSec = "Enemy.HealthRegenPerSec";
        // 필요 시 방어력/저항 등 확장 가능
    }
}