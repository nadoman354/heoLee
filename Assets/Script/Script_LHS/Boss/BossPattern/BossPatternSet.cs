/*
 * [BossPatternSet.cs]
 * - HP% 페이즈와 패턴 엔트리(가중치/쿨다운)로 구성된 데이터 세트
 * - 디자이너가 코드 없이 조합 가능
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

    public virtual void ApplyAffinityVariant() { /* 필요 시 변형 */ }
}
