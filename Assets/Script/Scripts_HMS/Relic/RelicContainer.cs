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
    static readonly Dictionary<Type, Type[]> ifaceCache = new(); // 타입별 인터페이스 캐시
    bool subscribed;

    public event Action SlotsChanged;


    public void Init(Player player)
    {
        this.player = player;
        relics = new List<BaseRelic>();
        byIface = new Dictionary<Type, List<object>>();

        SubscribeHealth(); // <-- 한 번만
        // 초기 상태도 한 번 방송(유물이 즉시 상태 평가 가능하도록)
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
        // Max 변경 이벤트가 있다면 같이 연결
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

        // 인터페이스 수집 (RelicHook만)
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

        // 장착 직후 현재 체력 상태에 따른 즉시 평가가 필요하면 한 번 더 방송
        // TriggerRelics<IOnHpChanged>(r => r.OnHpChanged(player, new HealthChangeContext(player.health.Current, player.health.Current, player.health.Max, HealthChangeReason.Init)));
        return true;
    }

    public void RemoveRelic(BaseRelic relic)
    {
        if (!relics.Remove(relic))
        {
            Debug.LogWarning("얻지 않은 유물을 제거하려 했습니다.");
            return;
        }

        if (ifaceCache.TryGetValue(relic.GetType(), out var ifaces))
        {
            foreach (var iface in ifaces)
            {
                if (byIface.TryGetValue(iface, out var list))
                {
                    list.Remove(relic);
                    if (list.Count == 0) byIface.Remove(iface); // 빈 버킷 정리
                }
            }
        }

        if (relic is IOnRemoveRelic rem) rem.Invoke(player);
    }

    public void TriggerRelics<T>(Action<T> action) where T : class
    {
        if (!byIface.TryGetValue(typeof(T), out var list)) return;

        // 유물이 자기 자신을 제거할 수도 있으므로, 스냅샷으로 안전하게 순회
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
