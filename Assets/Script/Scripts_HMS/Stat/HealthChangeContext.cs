// 체력 변경 컨텍스트
public enum HealthChangeReason { Damage, Heal, MaxChanged, Init }
public readonly struct HealthChangeContext
{
    public readonly float Prev, Cur, Delta, Max;
    public readonly HealthChangeReason Reason;
    public HealthChangeContext(float prev, float cur, float max, HealthChangeReason reason)
    {
        Prev = prev; Cur = cur; Delta = cur - prev; Max = max; Reason = reason;
    }
}