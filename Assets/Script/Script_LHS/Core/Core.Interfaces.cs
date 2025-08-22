
namespace Core.Interfaces
{
    /*
     * [Core.Interfaces.cs]
     * 무엇: 전투 공통 인터페이스/열거형 모음.
     *  - IDamageable: HP 피해 적용 가능한 대상.
     *  - IStaggerable: 그로기(무력화) 게이지가 있는 대상.
     *  - AttackerTeam: 공격자 팀 구분(플레이어/적/중립).
     *  - AttributeType: 속성 시스템용 타입(불/얼음/번개/없음).
     *
     * 책임:
     *  - 전투 시스템의 공용 계약(인터페이스) 제공.
     *  - HP와 그로기 책임 분리(느슨한 결합).
     *
     * 사용법:
     *  - 대상에 HP를 주고싶으면 IDamageable 구현 → TakeDamage 호출.
     *  - 대상이 그로기를 가진다면 IStaggerable 구현 → AddStagger 호출.
     *  - 무기/히트 처리 로직에서 대상.GetComponent<IDamageable/IStaggerable>()로 사용.
     */

    /// <summary>
    /// HP 피해를 적용할 수 있는 대상.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>양수면 체력을 깎고, 0 이하 값은 무시합니다.</summary>
        void TakeDamage(float amount);
    }

    /// <summary>
    /// 그로기(무력화) 게이지를 보유한 대상.
    /// </summary>
    public interface IStaggerable
    {
        /// <summary>그로기 게이지 누적량을 더합니다(양수만 유효).</summary>
        void AddStagger(float amount);
    }

    /// <summary>
    /// 공격자의 소속(온히트 효과 분기 등에 사용).
    /// </summary>
    public enum AttackerTeam
    {
        Player,
        Enemy,
        Neutral
    }

    /// <summary>
    /// 속성 타입(원한다면 확장: Poison, Holy, Dark 등).
    /// None 이면 속성/게이지 파이프라인을 생략하거나 기본 처리로 둡니다.
    /// </summary>
    public enum AttributeType
    {
        None,
        Fire,
        Poison,
        Ice,
        Lightning,
        Bleed
    }
}