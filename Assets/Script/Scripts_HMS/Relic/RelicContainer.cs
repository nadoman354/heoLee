using InterfaceRelic;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class RelicContainer : IRelicContainer
{
    Player player;
    List<BaseRelic> relics;
    Dictionary<Type, List<object>> byIface;
    static readonly Dictionary<Type, Type[]> ifaceCache = new(); // Ÿ�Ժ� �������̽� ĳ��
    bool subscribed;

    public event Action SlotsChanged;


    public void Init(Player player)
    {
        this.player = player;
        relics = new List<BaseRelic>();
        byIface = new Dictionary<Type, List<object>>();

        SubscribeHealth(); // <-- �� ����
        // �ʱ� ���µ� �� �� ���(������ ��� ���� �� �����ϵ���)
        var cur = player.health.Current;
        var max = player.health.Max;
        TriggerRelics<IOnHpChanged>(r => r.OnHpChanged(player, new HealthChangeContext(cur, cur, max, HealthChangeReason.Init)));
    }

    void OnDestroy() => UnsubscribeHealth();

    void SubscribeHealth()
    {
        if (subscribed || player?.health == null) return;
        player.health.OnDamaged += OnDamaged;
        player.health.OnHealed += OnHealed;
        // Max ���� �̺�Ʈ�� �ִٸ� ���� ����
        subscribed = true;
    }
    void UnsubscribeHealth()
    {
        if (!subscribed || player?.health == null) return;
        player.health.OnDamaged -= OnDamaged;
        player.health.OnHealed -= OnHealed;
        subscribed = false;
    }

    void OnDamaged(float amount)
    {
        var h = player.health;
        TriggerRelics<IOnHpChanged>(r => r.OnHpChanged(player,
            new HealthChangeContext(h.Current + amount, h.Current, h.Max, HealthChangeReason.Damage)));
    }
    void OnHealed(float amount)
    {
        var h = player.health;
        TriggerRelics<IOnHpChanged>(r => r.OnHpChanged(player,
            new HealthChangeContext(h.Current - amount, h.Current, h.Max, HealthChangeReason.Heal)));
    }

    public bool AddRelic(BaseRelic relic)
    {

        relics.Add(relic);

        // �������̽� ���� (RelicHook��)
        var t = relic.GetType();
        if (!ifaceCache.TryGetValue(t, out var ifaces))
        {
            ifaces = t.GetInterfaces().Where(i => typeof(IRelicHook).IsAssignableFrom(i)).ToArray();
            ifaceCache[t] = ifaces;
        }
        foreach (var iface in ifaces)
        {
            if (!byIface.TryGetValue(iface, out var list)) byIface[iface] = list = new List<object>();
            list.Add(relic);
        }

        if (relic is IOnAcquireRelic acq) acq.Invoke(player);

        // ���� ���� ���� ü�� ���¿� ���� ��� �򰡰� �ʿ��ϸ� �� �� �� ���
        // TriggerRelics<IOnHpChanged>(r => r.OnHpChanged(player, new HealthChangeContext(player.health.Current, player.health.Current, player.health.Max, HealthChangeReason.Init)));
        return true;
    }

    public void RemoveRelic(BaseRelic relic)
    {
        if (!relics.Remove(relic))
        {
            Debug.LogWarning("���� ���� ������ �����Ϸ� �߽��ϴ�.");
            return;
        }

        if (ifaceCache.TryGetValue(relic.GetType(), out var ifaces))
        {
            foreach (var iface in ifaces)
            {
                if (byIface.TryGetValue(iface, out var list))
                {
                    list.Remove(relic);
                    if (list.Count == 0) byIface.Remove(iface); // �� ��Ŷ ����
                }
            }
        }

        if (relic is IOnRemoveRelic rem) rem.Invoke(player);
    }

    public void TriggerRelics<T>(Action<T> action) where T : class
    {
        if (!byIface.TryGetValue(typeof(T), out var list)) return;

        // ������ �ڱ� �ڽ��� ������ ���� �����Ƿ�, ���������� �����ϰ� ��ȸ
        var snapshot = list.ToArray();
        for (int i = 0; i < snapshot.Length; i++)
        {
            if (snapshot[i] is T t) action(t);
        }
    }

    public void Tick(float deltaTime) => TriggerRelics<IUpdatableRelic>(x => x.Invoke(player));


    void IDisposable.Dispose()
    {
        throw new NotImplementedException();
    }
}
