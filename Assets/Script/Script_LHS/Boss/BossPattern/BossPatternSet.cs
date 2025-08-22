/*
 * [BossPatternSet.cs]
 * - HP% ������� ���� ��Ʈ��(����ġ/��ٿ�)�� ������ ������ ��Ʈ
 * - �����̳ʰ� �ڵ� ���� ���� ����
 */
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boss/PatternSet")]
public class BossPatternSet : ScriptableObject
{
    [System.Serializable]
    public class PatternEntry
    {
        public BossPatternBase pattern;
        [Min(1)] public int weight = 10;
        [Min(0)] public float cooldown = 0.5f;
    }

    [System.Serializable]
    public class PatternPhase
    {
        public string name;
        [Range(0f, 1f)] public float minHPPercent = 0f;
        [Range(0f, 1f)] public float maxHPPercent = 1f;
        public List<PatternEntry> patterns = new();
    }

    [System.Serializable]
    public class SpecialConditionPattern
    {
        public string triggerName;
        public BossPatternBase pattern;
    }

    public BossPatternBase defaultStartPattern;
    public List<PatternPhase> phases = new();
    public List<SpecialConditionPattern> specialConditions = new();

    public virtual void ApplyAffinityVariant() { /* �ʿ� �� ���� */ }
}
