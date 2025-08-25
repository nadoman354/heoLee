using UnityEngine;
using System;
using System.Collections.Generic;
using Core.Interfaces;
// using Core.Interfaces;  // ���� ���ӽ����̽� ������ �°� ����

/// <summary>
/// �ǰ��ڿ� �����Ǵ� ������Ʈ.
/// - �Ӽ� ������/����/���� ���
/// - ũ��/����Ʈ ���ο� �ݿ�
/// - ���� �������� ���� ������Ʈ�� IDamageable�� ����
/// </summary>
[DisallowMultipleComponent]
public class AttributeDamageHandler : MonoBehaviour, IAttributeDamageable
{
    [Header("Common")]
    [SerializeField] private float baseDamageMultiplier = 1f; // �ǰ��� ���� ���� ����

    // �Ӽ� ����/����(������ ǥ��)
    private readonly Dictionary<AttributeType, AttributeConfig> _cfg = new();
    private readonly Dictionary<AttributeType, AttributeStatus> _status = new();

    //���� ���� ���� ��������Ʈ�� float �ϳ���
    private Action<float> _applyFinal;

    void Awake()
    {
        // (���� ���̺� ����/�ʱ�ȭ ������ ������Ʈ �ڵ忡 �°�)
        // ...

        // ���� ������Ʈ�� IDamageable�� ���� ���� ����(�÷��̾�/����/���� ����)
        if (TryGetComponent<IDamageable>(out var any))
            _applyFinal = dmg => any.TakeDamage(dmg); //�ñ״�ó ����
    }

    // (ȣȯ��) ���� �ñ״�ó
    public void ApplyAttributeDamage(AttributeType type, float baseDamage, float gaugeAmount, IAttributeModifierProvider provider = null)
    {
        var req = new DamageRequest
        {
            baseDamage = baseDamage,
            type = type,
            gaugeGain = gaugeAmount,
            provider = provider,
            attackerTeam = AttackerTeam.Neutral,
            bypassAttributes = false,

            // ũ��/����Ʈ �⺻��
            canCrit = false,
            critChance = 0f,
            critMultiplier = 1f,
            onHitSlowAmount = 0f,
            onHitSlowDuration = 0f
        };
        ApplyAttributeDamage(in req);
    }

    /// <summary>
    /// ǥ�� ���: DamageRequest ��� ó��(ũ��/����Ʈ ���ο�/������/���� ����)
    /// </summary>
    public void ApplyAttributeDamage(in DamageRequest req)
    {
        if (_applyFinal == null) return;

        // 1) ������ ����ġ(����/����/���) ��ȸ
        AttributeMods mods = (req.provider != null) ? req.provider.GetAttributeMods(req.type) : AttributeMods.Identity;

        // 2) �⺻ ����
        float final = Mathf.Max(0f, req.baseDamage)
                    * Mathf.Max(0f, mods.baseDamageMul)
                    * Mathf.Max(0f, baseDamageMultiplier);

        // 2-1) ũ��Ƽ��: ���� �⺻ + ���� ���� / ���� ���� �� ���� ����
        if (req.canCrit && req.critMultiplier > 1f)
        {
            float chance = Mathf.Clamp01(req.critChance + Mathf.Max(0f, mods.critChanceAdd));
            float cmul = Mathf.Max(1f, req.critMultiplier * Mathf.Max(0.0001f, mods.critMultiplierMul));
            if (UnityEngine.Random.value < chance)
                final *= cmul;
        }

        bool triggered = false;

        // 3) �Ӽ�/������(Ʈ�� ������/���Ӽ��� ��ŵ)
        if (!req.bypassAttributes && req.type != AttributeType.None
            && _status.TryGetValue(req.type, out var s) && _cfg.TryGetValue(req.type, out var cfg))
        {
            float adjGauge = req.gaugeGain * Mathf.Max(0f, mods.gaugeGainMul);
            if (adjGauge > 0f) s.AddGauge(adjGauge);  // ����: AttributeStatus�� struct�� ���� ��� Ȯ��

            if (s.IsActivated)
            {
                final *= Mathf.Max(0f, cfg.activatedDamageMultiplier)
                       * Mathf.Max(0f, mods.activatedDamageMul);

                s.ResetGauge();
                triggered = true;

                // ���� ȿ�� ����(��: DOT/���ο�/���Ӱ���)
                if (TryGetComponent<StatusEffectManager>(out var sem))
                    sem.ApplyAttributeEffect(req.type, cfg.effectName, mods);
            }
        }

        // 4) ����Ʈ ���ο�(�÷��̾ ���� ����)
        if (req.attackerTeam == AttackerTeam.Player
            && req.onHitSlowAmount > 0f && req.onHitSlowDuration > 0f)
        {
            if (TryGetComponent<StatusEffectManager>(out var sem))
                sem.ApplyOnHitSlow(req.onHitSlowAmount, req.onHitSlowDuration);
        }

        // 5) ���� ���� ���� (IDamageable.TakeDamage(float) �԰�)
        _applyFinal(final);

        // �ʿ��ϴٸ� triggered/type�� �̺�Ʈ�� ������ UI/�α׿� ������.
        // OnAttributeTriggered?.Invoke(req.type, triggered);
    }
}
