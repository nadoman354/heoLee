using UnityEngine;
using System.Collections;

public class HitEffect : MonoBehaviour
{
    [Header("�ǰ� ����Ʈ ���ӽð�")]
    [SerializeField] private float effectDuration = 0.2f;         // ȿ�� ���� �ð�

    private SpriteRenderer spriteRenderer;

    private Color originalColor;
    private Color hitColor          = Color.red;  // �ǰ� �� ��������Ʈ ����

    private void Start()
    {
        spriteRenderer  = GetComponent<SpriteRenderer>();
        originalColor   = spriteRenderer.color;
    }

    public void TriggerEffect()
    {
        StopAllCoroutines(); // ���� ȿ���� �����ִٸ� �ʱ�ȭ
        StartCoroutine(HitFlash());
    }

    private IEnumerator HitFlash()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(effectDuration);
        spriteRenderer.color = originalColor;
    }
}
