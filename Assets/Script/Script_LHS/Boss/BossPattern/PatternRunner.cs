/*
 * [PatternRunner.cs]
 * ����: ���� ���� �����.
 *  - ���� ����, �ߺ� ����, ��ٿ�, HP% ������ ����.
 *  - ���� ����(RunOnce) �ڷ�ƾ ����, ���� �� �ڵ� ���� ����.
 *
 * å��:
 *  - ���� ���� ��ȯ ����(���� ��Ʈ�ѷ��� ����� ���).
 *
 * ����:
 *  - BossEnemy.Start���� new PatternRunner(this, set).
 *  - Update���� runner.Tick() ȣ��.
 *  - �׷α� ���� �� Pause(), ���� �� �ڵ� �簳.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternRunner
{
    private readonly BossBase boss;
    private readonly BossPatternSet set;
    private readonly Dictionary<BossPatternBase, float> cdRemain = new();

    private BossPatternBase last;
    private Coroutine running;

    public PatternRunner(BossBase boss, BossPatternSet set)
    {
        this.boss = boss;
        this.set = set;
    }

    public void Tick()
    {
        float dt = Time.deltaTime;

        // ��ٿ� ����
        foreach (var k in new List<BossPatternBase>(cdRemain.Keys))
            if ((cdRemain[k] -= dt) <= 0f) cdRemain.Remove(k);

        // ���� �� �ƴϰ� �׷α� �ƴϸ� ���� ����
        if (running == null && !boss.IsGroggy)
        {
            var pick = PickNext();
            if (pick != null) running = boss.StartCoroutine(RunRoutine(pick));
        }
    }

    public void Pause()
    {
        if (running != null) { boss.StopCoroutine(running); running = null; }
    }

    public void ForceSpecial(string trigger)
    {
        var e = set?.specialConditions?.Find(x => x.triggerName == trigger);
        if (e?.pattern == null) return;
        Pause();
        running = boss.StartCoroutine(RunRoutine(e.pattern));
    }

    private BossPatternBase PickNext()
    {
        if (set == null || set.phases == null || set.phases.Count == 0) return null;

        float hpPct = boss.HealthPercent;

        BossPatternSet.PatternPhase phase = null;
        foreach (var ph in set.phases)
            if (hpPct >= ph.minHPPercent && hpPct <= ph.maxHPPercent) { phase = ph; break; }
        if (phase == null || phase.patterns.Count == 0) return null;

        var cands = new List<BossPatternSet.PatternEntry>();
        foreach (var e in phase.patterns)
        {
            if (e?.pattern == null) continue;
            if (cdRemain.ContainsKey(e.pattern)) continue;
            if (e.pattern == last) continue;
            cands.Add(e);
        }
        if (cands.Count == 0) // ����
        {
            foreach (var e in phase.patterns)
            {
                if (e?.pattern == null) continue;
                if (cdRemain.ContainsKey(e.pattern)) continue;
                cands.Add(e);
            }
        }
        if (cands.Count == 0) return null;

        int total = 0; foreach (var e in cands) total += Mathf.Max(1, e.weight);
        int roll = Random.Range(0, total);
        BossPatternSet.PatternEntry pick = null;
        foreach (var e in cands)
        {
            roll -= Mathf.Max(1, e.weight);
            if (roll < 0) { pick = e; break; }
        }
        return pick?.pattern;
    }

    private IEnumerator RunRoutine(BossPatternBase p)
    {
        last = p;

        var entry = FindEntry(p);
        if (entry != null && entry.cooldown > 0f) cdRemain[p] = entry.cooldown;

        p.SetFinishCallback(boss.OnPatternFinished);

        var co = p.RunOnce(boss);
        if (co != null) yield return boss.StartCoroutine(co);
        else { p.Enter(boss); p.UpdatePattern(boss); p.Exit(boss); boss.OnPatternFinished(); }

        running = null;
    }

    private BossPatternSet.PatternEntry FindEntry(BossPatternBase p)
    {
        foreach (var ph in set.phases)
            foreach (var e in ph.patterns)
                if (e?.pattern == p) return e;
        return null;
    }
}
