using UnityEngine;
namespace Stats
{
    public class BasicBossStat : MonoBehaviour
    {
        // ���� ����
        public static readonly string MaxHp = "Boss.MaxHp";
        public static readonly string Damage = "Boss.Damage";
        public static readonly string MoveSpeed = "Boss.MoveSpeed";
        public static readonly string HealthRegenPerSec = "Boss.HealthRegenPerSec";

        // �׷α�(������/���� ��Ģ: + ����, �� ���� ��)
        public static readonly string GroggyMax = "Boss.GroggyMax";          // ������ �ִ�ġ(�ʿ� ��)
        public static readonly string GroggyBaseDuration = "Boss.GroggyBaseDuration"; // �⺻ ���ӽð�

        // ����/���� ����(+)
        public static readonly string GroggyGainAdd = "Boss.GroggyGainAdd";
        public static readonly string GroggyDurationAdd = "Boss.GroggyDurationAdd";

        // ����/���� ������(��, %���� ���� 1+������ �������� ��)
        public static readonly string GroggyGainMulSum = "Boss.GroggyGainMulSum";
        public static readonly string GroggyDurationMulSum = "Boss.GroggyDurationMulSum";
    }
}