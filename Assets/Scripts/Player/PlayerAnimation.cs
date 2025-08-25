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
    
    //public void PlayAttackMoveAnim() 공격 시 살짝 움직일 때 애니메이션 걷기 애니메이션 중 2프레임까지만 쓰면 좋을듯

    public void PlayDieAnim()
    {
        animator.SetTrigger("dead");
    }
}
