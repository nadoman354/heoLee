using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.ContentSizeFitter;

public interface IDamageable
{
    void TakeDamage(int dmg);
}
public enum BulletFitMode { FitInside, Fill, MatchWidth, MatchHeight }

public class Bullet : MonoBehaviour
{
    // Cached refs
    [Header("Refs")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Animator animator;
    [SerializeField] Transform shape;          // ★ 콜라이더  트랜스폼
    [SerializeField] Transform gfx;       // 스프라이트 스케일 대상

    [Header("Colliders (pre-attached & set as Trigger)")]
    [SerializeField] CircleCollider2D circleCol;
    [SerializeField] BoxCollider2D boxCol;
    [SerializeField] CapsuleCollider2D capsuleCol;
    [SerializeField] PolygonCollider2D polygonCol;

    // 설정
    [SerializeField] bool fitVisualToCollider = true;
    [SerializeField] BulletFitMode fitMode = BulletFitMode.FitInside;
    [SerializeField] bool onlyShrink = true; // true면 "줄이기만" (커지지 않음)
    // Runtime
    SO_BulletConfig cfg;
    Vector2 dir;
    float lifeRemain;
    BulletPool pool;

    // ����: �� ������ GetComponent ���� �� Awake���� ���� ����
    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!sr) sr = GetComponentInChildren<SpriteRenderer>(true);
        if (!animator) animator = GetComponentInChildren<Animator>(true);
    }

    public void SetPool(BulletPool p) => pool = p;


    public void Init(SO_BulletConfig config, Vector2 pos, Vector2 direction)
    {
        cfg = config;
        transform.position = pos;
        dir = direction.sqrMagnitude > 0 ? direction.normalized : Vector2.right;
        lifeRemain = cfg.lifetime;

        SetupVisualAndShape();
        SetupCollider();            // 같은 Shape에 붙은 콜라이더만 On/Off
        SetupPhysics();
        OrientToDirectionImmediate();
        gameObject.SetActive(true);
        if (fitVisualToCollider) FitGfxToActiveCollider(); // ★ 여기서 맞춤
    }
    public void Init(SO_BulletConfig config, Vector2 pos, Quaternion direction)
    {
        cfg = config;
        transform.position = pos;
        transform.rotation = direction;
        dir = transform.right.sqrMagnitude > 0 ? transform.right : Vector2.right;
        lifeRemain = cfg.lifetime;

        SetupVisualAndShape();
        SetupCollider();            // 같은 Shape에 붙은 콜라이더만 On/Off
        SetupPhysics();
        OrientToDirectionImmediate();
        gameObject.SetActive(true);
        if (fitVisualToCollider) FitGfxToActiveCollider(); // ★ 여기서 맞춤
    }
    void FitGfxToActiveCollider()
    {
        if (sr == null || sr.sprite == null || gfx == null) return;

        Vector2 spriteLocalSize = sr.sprite.bounds.size; // 스케일 1 기준 로컬 사이즈(유닛)
        if (spriteLocalSize.x <= 0f || spriteLocalSize.y <= 0f) return;

        Vector2 targetSize = GetActiveColliderLocalSize(); // 콜라이더 로컬 AABB

        // 원하는 스케일 계산
        float sx = targetSize.x / spriteLocalSize.x;
        float sy = targetSize.y / spriteLocalSize.y;

        float ux, uy;
        switch (fitMode)
        {
            case BulletFitMode.FitInside:   // 최대한 크게, 넘치지 않게
                float sIn = Mathf.Min(sx, sy);
                ux = uy = sIn;
                break;
            case BulletFitMode.Fill:        // 빈틈 없이 덮기
                float sFill = Mathf.Max(sx, sy);
                ux = uy = sFill;
                break;
            case BulletFitMode.MatchWidth:
                ux = sx; uy = sx;
                break;
            case BulletFitMode.MatchHeight:
                ux = sy; uy = sy;
                break;
            default:
                ux = sx; uy = sy;
                break;
        }

        if (onlyShrink)
        {
            ux = Mathf.Min(ux, 1f);
            uy = Mathf.Min(uy, 1f);
        }

        
        gfx.localScale = new Vector3(ux, uy, 1f);
    }

    Vector2 GetActiveColliderLocalSize()
    {
        if (circleCol && circleCol.enabled)
        {
            float d = circleCol.radius * 2f;
            return new Vector2(d, d);
        }
        if (boxCol && boxCol.enabled)
        {
            return boxCol.size;
        }
        if (capsuleCol && capsuleCol.enabled)
        {
            return capsuleCol.size; // 캡슐의 로컬 가로/세로
        }
        if (polygonCol && polygonCol.enabled)
        {
            // 폴리곤 로컬 AABB 계산(월드 bounds는 회전 영향 받아 부정확할 수 있음)
            var min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            var max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
            var pts = new List<Vector2>(32);
            for (int i = 0; i < polygonCol.pathCount; i++)
            {
                polygonCol.GetPath(i, pts);
                foreach (var p in pts)
                {
                    if (p.x < min.x) min.x = p.x;
                    if (p.y < min.y) min.y = p.y;
                    if (p.x > max.x) max.x = p.x;
                    if (p.y > max.y) max.y = p.y;
                }
            }
            return max - min;
        }
        return Vector2.one; // fallback
    }

    void SetupVisualAndShape()
    {
        // Animator 우선
        if (animator && cfg.animatorOverride)
        {
            animator.runtimeAnimatorController = cfg.animatorOverride;
            animator.enabled = true;
            sr.enabled = false;
        }
        else
        {
            sr.enabled = true;
            sr.sprite = cfg.sprite;
            if (cfg.materialOverride) sr.material = cfg.materialOverride;
            animator.enabled = false;
        }

        // ★ 스케일/회전은 Shape에만 적용 → 비주얼과 콜라이더가 동기화
        shape.localScale = new Vector3(cfg.spriteScale.x, cfg.spriteScale.y, 1f);
        shape.localPosition = Vector3.zero;
        shape.localRotation = Quaternion.identity;
    }

    void OrientToDirectionImmediate()
    {
        // 진행 방향이 '위(Up)'인 아트 기준
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        // ★ 회전도 Shape에 적용하면 콜라이더가 함께 회전
        shape.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    

    void SetupVisual()
    {
        // Animator �켱 �� ������ �ִϸ��̼�, ������ ��������Ʈ
        if (animator && cfg.animatorOverride)
        {
            animator.runtimeAnimatorController = cfg.animatorOverride;
            animator.enabled = true;
            if (sr) sr.enabled = false;
        }
        else
        {
            if (sr)
            {
                sr.enabled = true;
                sr.sprite = cfg.sprite;
                sr.transform.localScale = (Vector3)cfg.spriteScale;
                if (cfg.materialOverride) sr.material = cfg.materialOverride;
            }
            if (animator) animator.enabled = false;
        }
    }

    void SetupCollider()
    {
        // ���� ���� �ʿ��� �͸� On
        if (circleCol) circleCol.enabled = false;
        if (boxCol) boxCol.enabled = false;
        if (capsuleCol) capsuleCol.enabled = false;
        if (polygonCol) polygonCol.enabled = false;

        switch (cfg.colliderType)
        {
            case BulletColliderType.Circle:
                if (circleCol)
                {
                    circleCol.radius = cfg.circleRadius;
                    circleCol.enabled = true;
                }
                break;
            case BulletColliderType.Box:
                if (boxCol)
                {
                    boxCol.size = cfg.boxSize;
                    boxCol.enabled = true;
                }
                break;
            case BulletColliderType.Capsule:
                if (capsuleCol)
                {
                    capsuleCol.size = cfg.capsuleSize;
                    capsuleCol.direction = cfg.capsuleDirection;
                    capsuleCol.enabled = true;
                }
                break;
            case BulletColliderType.Polygon:
                if (polygonCol)
                {
                    polygonCol.pathCount = 1;
                    if (cfg.polygonPoints != null && cfg.polygonPoints.Length >= 3)
                        polygonCol.SetPath(0, System.Array.ConvertAll(cfg.polygonPoints, v => (Vector2)v));
                    polygonCol.enabled = true;
                }
                break;
        }
    }

    void SetupPhysics()
    {
        if (rb)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            if (cfg.useRigidbodyVelocity)
            {
                rb.linearVelocity = dir * cfg.speed;
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    void Update()
    {
        // ���� �̵� (���� ź������ �� ������)
        if (!cfg.useRigidbodyVelocity)
        {
            transform.position += (Vector3)(dir * cfg.speed * Time.deltaTime);
        }

        if (cfg.rotateToVelocity)
            OrientToDirectionImmediate();

        lifeRemain -= Time.deltaTime;
        if (lifeRemain <= 0f)
            Despawn();
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        // Hit Mask ����
        if ((cfg.hitMask.value & (1 << other.gameObject.layer)) == 0)
            return;

        if (other.TryGetComponent<IDamageable>(out var target))
        {
            target.TakeDamage(cfg.damage);
        }
        Despawn();
    }

    public void Despawn()
    {
        // ���� �ʱ�ȭ�� �ʿ��ϸ� ���⼭ ó�� (��: Trail, Animator ���� ��)
        pool.Return(this);
    }
}
