using System;
using UnityEngine;

public struct SetAnimatorInfo { /* 필요 시 확장 (레이어, 블렌드 등) */ }

public class WeaponView : MonoBehaviour
{
    [SerializeField] Animator animator;

    public event Action OnAE_Attack;
    public event Action OnAE_Skill1;
    public event Action OnAE_Skill2;

    static readonly int H_Attack = Animator.StringToHash("Attack");
    static readonly int H_AtkCnt = Animator.StringToHash("AtkCount");
    static readonly int H_Skill1 = Animator.StringToHash("Skill1");
    static readonly int H_Skill2 = Animator.StringToHash("Skill2");

    public void SetAnimator(AnimatorOverrideController controller, in SetAnimatorInfo _)
        => animator.runtimeAnimatorController = controller;

    public void TriggerAttackAnim(int comboCnt)
    {
        animator.SetInteger(H_AtkCnt, comboCnt);
        animator.SetTrigger(H_Attack);
    }
    public void TriggerSkill1Anim() => animator.SetTrigger(H_Skill1);
    public void TriggerSkill2Anim() => animator.SetTrigger(H_Skill2);

    // Animation Events (클립에서 호출)
    public void AE_Attack() => OnAE_Attack?.Invoke();
    public void AE_Skill1() => OnAE_Skill1?.Invoke();
    public void AE_Skill2() => OnAE_Skill2?.Invoke();

    // 장착 교체 시 구독/해제를 편하게
    Action _bAtk, _bS1, _bS2;
    public void BindHandlers(Action onAttack = null, Action onSkill1 = null, Action onSkill2 = null)
    {
        UnbindHandlers();
        if (onAttack != null) { OnAE_Attack += onAttack; _bAtk = onAttack; }
        if (onSkill1 != null) { OnAE_Skill1 += onSkill1; _bS1 = onSkill1; }
        if (onSkill2 != null) { OnAE_Skill2 += onSkill2; _bS2 = onSkill2; }
    }
    public void UnbindHandlers()
    {
        if (_bAtk != null) { OnAE_Attack -= _bAtk; _bAtk = null; }
        if (_bS1 != null) { OnAE_Skill1 -= _bS1; _bS1 = null; }
        if (_bS2 != null) { OnAE_Skill2 -= _bS2; _bS2 = null; }
    }
}
