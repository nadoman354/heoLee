using UnityEngine;

public enum BulletColliderType { Circle, Box, Capsule, Polygon }

[CreateAssetMenu(fileName = "SO_BulletConfig", menuName = "Game/Bullet/SO_BulletConfig")]
public class SO_BulletConfig : ScriptableObject
{
    [Header("Visual")]
    public Sprite sprite;                                   // ���� ��������Ʈ��
    public AnimatorOverrideController animatorOverride;     // �ִϸ��̼ǿ�(����)
    public Vector2 spriteScale = Vector2.one;
    public Material materialOverride;                       // ����

    [Header("Collider")]
    public BulletColliderType colliderType = BulletColliderType.Circle;
    public float circleRadius = 0.1f;
    public Vector2 boxSize = new Vector2(0.1f, 0.1f);
    public CapsuleDirection2D capsuleDirection = CapsuleDirection2D.Vertical;
    public Vector2 capsuleSize = new Vector2(0.1f, 0.2f);
    public Vector2[] polygonPoints;                         // ������ ��� ��

    [Header("Gameplay")]
    public float speed = 6f;
    public float lifetime = 4f;
    public int damage = 1;
    public LayerMask hitMask;
    public bool rotateToVelocity = true;                    // �������� ȸ��
    public bool useRigidbodyVelocity = false;               // ���� ������ false ����(���� �̵�)
}
