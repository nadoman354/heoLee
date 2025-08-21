using UnityEngine;

/// <summary>
/// 애니메이션 State가 끝나면 자신이 달린 GameObject를 파괴.
/// </summary>
public class DestroyOnStateExit : StateMachineBehaviour
{
    // State 를 벗어나는 순간 호출됩니다.
    public override void OnStateExit(Animator animator,
                                     AnimatorStateInfo stateInfo,
                                     int layerIndex)
    {
        // 같은 오브젝트(Animator 가 붙어있는) 제거
        Object.Destroy(animator.gameObject);
    }
}