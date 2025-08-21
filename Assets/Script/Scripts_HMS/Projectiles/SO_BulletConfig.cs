using UnityEngine;

public enum BulletColliderType { Circle, Box, Capsule, Polygon }

[CreateAssetMenu(fileName = "SO_BulletConfig", menuName = "Game/Bullet/SO_BulletConfig")]
public class SO_BulletConfig : ScriptableObject
{
    [Header("Visual")]
    public Sprite sprite;                                   // 정적 스프라이트용
    public AnimatorOverrideController animatorOverride;     // 애니메이션용(선택)
    public Vector2 spriteScale = Vector2.one;
    public Material materialOverride;                       // 선택

    [Header("Collider")]
    public BulletColliderType colliderType = BulletColliderType.Circle;
    public float circleRadius = 0.1f;
    public Vector2 boxSize = new Vector2(0.1f, 0.1f);
    public CapsuleDirection2D capsuleDirection = CapsuleDirection2D.Vertical;
    public Vector2 capsuleSize = new Vector2(0.1f, 0.2f);
    public Vector2[] polygonPoints;                         // 폴리곤 사용 시

    [Header("Gameplay")]
    public float speed = 6f;
    public float lifetime = 4f;
    public int damage = 1;
    public LayerMask hitMask;
    public bool rotateToVelocity = true;                    // 방향으로 회전
    public bool useRigidbodyVelocity = false;               // 수가 많으면 false 권장(수동 이동)
}
