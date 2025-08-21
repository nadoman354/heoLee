using DG.Tweening;
using UnityEngine;
using enums;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public abstract class DroppedItem : MonoBehaviour, IInteractable
{
    [Header("Refs")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D interactCollider; // isTrigger=true ����

    [Header("Flight FX")]
    [Min(0f)][SerializeField] private float launchPower = 4f;  // ���� �Ŀ�(����)
    [Min(0f)][SerializeField] private float dropDuration = 0.6f; // ���� ���� �ð�

    [Tooltip("�⺻ ȸ����(��)")][SerializeField] private float spinBaseDegrees = 540f;
    [Tooltip("�߰� ȸ�� ���� ����(��)")][SerializeField] private float spinVariance = 360f;
    [Tooltip("���� �Ŀ� ������ �ּҰ�")][SerializeField] private float jumpScaleMin = 0.9f;
    [Tooltip("���� �Ŀ� ������ �ִ밪")][SerializeField] private float jumpScaleMax = 1.2f;

    private bool isDropping;

    protected void SetSprite(Sprite s) { if (spriteRenderer) spriteRenderer.sprite = s; }

    /// <summary>
    /// ���� ���� (������ RNG)
    /// - spinBaseDegrees + [0,spinVariance] ��ŭ ȸ��
    /// - launchPower * [jumpScaleMin, jumpScaleMax] ���̷� ����
    /// - �Ϸ� �� ��ȣ�ۿ� �ݶ��̴� Ȱ��ȭ
    /// </summary>
    public void DropTo(Vector3 landingPosition, System.Random rng)
    {
        if (interactCollider) interactCollider.enabled = false;
        isDropping = true;

        float dir = rng.NextDouble() < 0.5 ? -1f : 1f;
        float spin = spinBaseDegrees + (float)rng.NextDouble() * spinVariance;
        float jump = launchPower * Mathf.Lerp(jumpScaleMin, jumpScaleMax, (float)rng.NextDouble());

        transform.DORotate(new Vector3(0, 0, spin * dir), dropDuration, RotateMode.FastBeyond360);

        transform
            .DOJump(landingPosition, jump, 1, dropDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                transform.DORotate(Vector3.zero, 0.2f).SetEase(Ease.OutCubic);
                if (interactCollider) interactCollider.enabled = true;
                isDropping = false;
            });
    }

    // ---- IInteractable ----
    public abstract void Setup(ScriptableObject data);
    public abstract void OnPlayerNearby();
    public abstract void OnPlayerInteract(Player player);

    public void OnPlayerLeave() { }
    public void GetInteract(string text) { }
    public void OnHighlight() { }
    public void OnUnHighlight() { }
    public bool CanInteract(Player player) => !isDropping;
    public InteractState GetInteractType() => InteractState.Looting;
    public string GetInteractDescription() => "ȹ��";
}
