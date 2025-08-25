using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerWeaponHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform weaponRoot;       // ���� �ٴ� �ڸ����� ȸ�� ���
    [SerializeField] private Transform playerCenter;     // ȸ�� ������ (���� �÷��̾� ��ü)

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

        //�ٽ�: �¿� ��Ī ȸ���� ����
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