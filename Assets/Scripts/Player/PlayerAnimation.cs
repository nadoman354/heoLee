using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAnimation : MonoBehaviour
{
    private SpriteRenderer  sprite;
    private Animator        animator;

    private void Awake()
    {
        sprite      = GetComponent<SpriteRenderer>();
        animator    = GetComponent<Animator>();
    }

    public void LookAtMouse(Vector2 mousePos)
    {
        if (mousePos.x < transform.position.x)  sprite.flipX = true;
        else                                    sprite.flipX = false;
    }

    public void PlayMoveAnim(Vector2 moveDirection)
    {
        animator.SetFloat("speed", moveDirection.normalized.magnitude);
    }

    public void PlayDashAnim()
    {
        animator.SetTrigger("dash");
    }
    
    //public void PlayAttackMoveAnim() ���� �� ��¦ ������ �� �ִϸ��̼� �ȱ� �ִϸ��̼� �� 2�����ӱ����� ���� ������

    public void PlayDieAnim()
    {
        animator.SetTrigger("dead");
    }
}
