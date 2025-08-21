using InterfaceRelic;
using UnityEngine;

public sealed class FireRelic : BaseRelic, IOnAcquireRelic, IOnRemoveRelic, IOnHpChanged
{
    ScopedMod _handle; // ������ ��� ��ū(����/���� ��)
    Modifier _mod;
    float _threshold;
    bool _applied;

    // * ���� ���� ���� *
    string targetStat => Stats.BasicPlayerStat.Damage; // ���� ���ݷ� <- ������ ������ Player���� ������, �� �Ű��� ����!
    float addedDamage => statData.GetStat(Stats.BasicRelicStat.Damage); // ���� ���ݷ� ������

    public override void Init(SO_RelicMetaData meta, Inventory inv)
    {
        base.Init(meta, inv);
        _mod = new Modifier(ModifierType.Flat, targetStat, addedDamage, this);
        _threshold = statData.GetStat(Stats.BasicRelicStat.HealthCondition);
    }

    void IOnAcquireRelic.Invoke(Player p) => Evaluate(p);
    public void OnHpChanged(Player p, in HealthChangeContext ctx) => Evaluate(p);

    void Evaluate(Player p)
    {
        bool should = p.health.Current < _threshold;

        if (should && !_applied)
        {
            // ������ ���� ���⡱ �������� �� ���� ��û�ϸ�, ��ü �� �ڵ����� ������
            _handle = inv.ApplyModifier(_mod, ModTargetKind.CurrentWeapon);
            _applied = true;
        }
        else if (!should && _applied)
        {
            inv.RemoveModifier(_handle);
            _applied = false;
        }
    }

    void IOnRemoveRelic.Invoke(Player p)
    {
        if (_applied) { inv.RemoveModifier(_handle); _applied = false; }
    }

}
