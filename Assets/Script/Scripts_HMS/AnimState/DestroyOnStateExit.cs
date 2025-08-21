using UnityEngine;

/// <summary>
/// �ִϸ��̼� State�� ������ �ڽ��� �޸� GameObject�� �ı�.
/// </summary>
public class DestroyOnStateExit : StateMachineBehaviour
{
    // State �� ����� ���� ȣ��˴ϴ�.
    public override void OnStateExit(Animator animator,
                                     AnimatorStateInfo stateInfo,
                                     int layerIndex)
    {
        // ���� ������Ʈ(Animator �� �پ��ִ�) ����
        Object.Destroy(animator.gameObject);
    }
}