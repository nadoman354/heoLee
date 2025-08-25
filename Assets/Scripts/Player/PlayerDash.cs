using System;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("대쉬 세팅")]
    [SerializeField] private    float dashSpeed;                                //대쉬 속도
    [SerializeField] private    float dashDistance;                             //대쉬 거리
    [SerializeField] private    float dashCoolTime;                             //대쉬 쿨타임 시간
    [SerializeField] public     float dashInvincibleTime { get; private set; }  //대쉬 무적 시간

    [Header("인스펙터 참조")]
    [SerializeField] private ParticleSystem dashAfterimage; //대쉬 잔상 파티클시스템

    private Rigidbody2D rigid2D;
    private LayerMask wallMask = 1 << 8;

    public  bool    isDashing   { get; private set; } = false;  //대쉬 여부
    public  float   dashTimer   { get; private set; } = 0.0f;   //대쉬 시간
    public float dashCoolTimeRatio => dashTimer / dashCoolTime;

    private Vector2     startPos;       //시작 위치
    private Vector2     dashDirection;  //대쉬 방향
    private Vector2     targetPos;      //목표 위치

    public event Action OnDashExecuted;

    private void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
    }

    public void TryDash(Vector2 direction, Vector2 mousePos)
    {
        dashDirection   = direction.normalized;
        startPos        = rigid2D.position;

        //레이캐스트
        RaycastHit2D hit = Physics2D.Raycast(startPos, dashDirection, dashDistance, wallMask);
        if (hit.collider != null)
        {
            // 레이캐스트 히트 지점 - 0.05f를 targetPos로 설정
            targetPos = hit.point - dashDirection * 0.05f;
        }
        else targetPos = startPos + dashDirection * dashDistance;

        isDashing = true;
        dashTimer = dashCoolTime;

        OnDashExecuted?.Invoke();

        CreateDashAfterimage(mousePos);
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            float   step            = dashSpeed * Time.fixedDeltaTime;
            Vector2 nextPos         = rigid2D.position + dashDirection * step;
            float   remaining       = Vector2.Distance(rigid2D.position, targetPos);

            if (step >= remaining)
            {
                rigid2D.MovePosition(targetPos);
                isDashing = false;
            }
            else rigid2D.MovePosition(nextPos);
        }

        if (dashTimer > 0f) dashTimer -= Time.fixedDeltaTime;
    }

    private void CreateDashAfterimage(Vector2 mousePos)
    {
        if (mousePos.x < transform.position.x)  dashAfterimage.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else                                    dashAfterimage.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        dashAfterimage.Play();
    }

    private void Update()
    {
        //Debug.Log(isDashing);
    }
}