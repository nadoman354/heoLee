using UnityEngine;
using System.Collections;

public class HitEffect : MonoBehaviour
{
    [Header("피격 이펙트 지속시간")]
    [SerializeField] private float effectDuration = 0.2f;         // 효과 지속 시간

    private SpriteRenderer spriteRenderer;

    private Color originalColor;
    private Color hitColor          = Color.red;  // 피격 시 스프라이트 색상

    private void Start()
    {
        spriteRenderer  = GetComponent<SpriteRenderer>();
        originalColor   = spriteRenderer.color;
    }

    public void TriggerEffect()
    {
        StopAllCoroutines(); // 이전 효과가 남아있다면 초기화
        StartCoroutine(HitFlash());
    }

    private IEnumerator HitFlash()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(effectDuration);
        spriteRenderer.color = originalColor;
    }
}
