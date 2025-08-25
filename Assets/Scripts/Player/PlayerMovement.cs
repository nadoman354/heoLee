using DG.Tweening;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rigid2D;

    [Header("인스펙터 참조")]
    [SerializeField] private ParticleSystem dust;

    private bool isLocked = false;
    public void LockMovement() => isLocked = true;
    public void UnlockMovement() => isLocked = false;


    private void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 moveDirection, float moveSpeed, bool isDashing)
    {
        if (isLocked) return;

        Vector2 newPosition = rigid2D.position + moveDirection.normalized * moveSpeed * Time.fixedDeltaTime;

        rigid2D.MovePosition(newPosition);

        if (!dust.isPlaying && moveDirection != Vector2.zero)
        {
            if (moveDirection.x < 0)    dust.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            else                        dust.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            dust.Play();
        }
    }

    public void ForceMove(Vector2 moveDirection, float moveDistance)
    {
        Vector2 newPosition = rigid2D.position + moveDirection.normalized * moveDistance;
        rigid2D.MovePosition(newPosition);
    }

    public void ForceMoveTween(Vector2 dir, float distance, float duration = 0.15f)
    {
        Vector2 target = (Vector2)transform.position + dir.normalized * distance;
        transform.DOMove(target, duration).SetEase(Ease.OutQuad);
    }
}