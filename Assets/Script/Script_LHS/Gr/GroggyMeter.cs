using System;
using UnityEngine;

/*
 * [GroggyMeter.cs]
 * - 그로기(무력화) 전용 게이지/상태 머신
 * - Add로 누적 → 임계 도달 시 Enter → Duration 경과 후 Exit
 * - 자연감쇠/락아웃(재축적 금지) 옵션
 */
public sealed class GroggyMeter
{
    public event Action OnEnter, OnExit;

    private readonly float max;
    private readonly float decayPerSec;   // 자연감쇠(선택)
    private readonly float lockoutAfter;  // 해제 후 재축적 금지 시간(선택)

    private float current;
    private float lockoutRemain;

    public bool IsGroggy { get; private set; }
    public float Duration { get; private set; }

    public GroggyMeter(float max, float baseDuration, float decayPerSec = 0f, float lockoutAfter = 1.2f)
    {
        this.max = Mathf.Max(1f, max);
        this.decayPerSec = Mathf.Max(0f, decayPerSec);
        this.lockoutAfter = Mathf.Max(0f, lockoutAfter);
        SetDurationFinal(baseDuration);
    }

    public void SetDurationFinal(float final) => Duration = Mathf.Max(0.05f, final);

    public void Tick(float dt)
    {
        if (IsGroggy) return;
        if (lockoutRemain > 0f) lockoutRemain -= dt;
        if (decayPerSec > 0f && current > 0f)
            current = Mathf.Max(0f, current - decayPerSec * dt);
    }

    public void Add(float amount)
    {
        if (IsGroggy || lockoutRemain > 0f || amount <= 0f) return;
        current = Mathf.Min(max, current + amount);
        if (current >= max) Enter();
    }

    private void Enter()
    {
        IsGroggy = true;
        current = 0f;
        lockoutRemain = 0f;
        OnEnter?.Invoke();
    }

    public void Exit()
    {
        if (!IsGroggy) return;
        IsGroggy = false;
        lockoutRemain = lockoutAfter;
        OnExit?.Invoke();
    }
}
