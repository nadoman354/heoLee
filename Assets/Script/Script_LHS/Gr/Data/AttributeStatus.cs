using Core.Interfaces;
using UnityEngine;

/// <summary>개별 속성의 게이지 상태 관리(누적/감소/발현 체크).</summary>
public class AttributeStatus
{
    public AttributeType Type;
    public float CurrentGauge;
    public float MaxGauge;

    public bool IsActivated => CurrentGauge >= MaxGauge;

    public void AddGauge(float v) => CurrentGauge = Mathf.Min(CurrentGauge + v, MaxGauge);
    public void Decay(float v) => CurrentGauge = Mathf.Max(0f, CurrentGauge - v);
    public void ResetGauge() => CurrentGauge = 0f;
}
