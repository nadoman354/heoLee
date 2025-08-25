using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerWeaponHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform weaponRoot;       // 무기 붙는 자리이자 회전 대상
    [SerializeField] private Transform playerCenter;     // 회전 기준점 (보통 플레이어 본체)

    private PlayerController input;

    private bool facingRight = true;

    private void Awake()
    {
        input = GetComponent<PlayerController>();
    }

    private void Update()
    {
        RotationWeapon();
    }

    private void RotationWeapon()
    {
        Vector3 mouseWorld = input.mouseWorldPosition;
        Vector2 dir = mouseWorld - playerCenter.position;

        float rawAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        bool shouldFaceRight = rawAngle > -90f && rawAngle < 90f;

        if (shouldFaceRight != facingRight)
        {
            Flip();
        }

        //핵심: 좌우 대칭 회전값 보정
        float weaponAngle = facingRight ? rawAngle : 180f + rawAngle;

        weaponRoot.rotation = Quaternion.Euler(0, 0, weaponAngle);
    }

    private void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = weaponRoot.localScale;
        scale.x = facingRight ? 1f : -1f;
        weaponRoot.localScale = scale;
    }

    public bool IsFacingRight() => facingRight;
}