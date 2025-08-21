using System;
using System.Collections.Generic;

// ↓ 참고용: 외부 타입 가정
// Player : IModifierSink 가능
// IWeaponContainer : Current/FindByMeta/GetSlot/EquippedChanged/SlotsChanged/Tick/Dispose ...
// IRelicContainer  : Init/Tick/AddRelic/RemoveRelic/Dispose ...
// IWeaponLogic     : IModifierSink 가능
// ISkillLogic      : IModifierSink 가능
//public interface IModifierSink { void AddModifier(Modifier m); void RemoveModifier(Modifier m); }
//public interface ISkillProvider { ISkillLogic GetSkill(int index); } // 무기 로직이 구현(스킬 접근)
//public enum ModTargetKind { Player, CurrentWeapon, CurrentWeaponSkill, WeaponSlot, WeaponByMeta, WeaponSkillByMeta }

public sealed class Inventory : IDisposable
{
    readonly Player _player;     // 외부 주입
    readonly WeaponView _view;   // 외부 주입
    WeaponController _controller;

    public IWeaponContainer Weapons { get; }
    public IRelicContainer Relics { get; }
    public IConsumableItemContainer Consumables { get; }

    // 동적 스코프(CurrentWeapon/CurrentWeaponSkill/ByMeta*)만 추적
    readonly List<ScopedMod> _dynamicMods = new();

    // --------- 생성 ---------
    public Inventory(Player player, WeaponView view)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _view = view ?? throw new ArgumentNullException(nameof(view));

        Weapons = new WeaponContainer(_view);
        Relics = new RelicContainer();
        Consumables = new ConsumableItemContainer();

        Relics.Init(_player);
        Consumables.Init(player);

        // ⚠️ 꼭 이벤트 구독: 외부가 Weapons를 직접 조작해도 Inventory가 후처리 수행
        Weapons.EquippedChanged += OnEquippedChanged;
        Weapons.SlotsChanged += OnSlotsChanged;
    }

    // DI 버전
    public Inventory(IWeaponContainer weapons, IRelicContainer relics, WeaponView view)
    {
        Weapons = weapons ?? throw new ArgumentNullException(nameof(weapons));
        Relics = relics ?? throw new ArgumentNullException(nameof(relics));
        _view = view ?? throw new ArgumentNullException(nameof(view));

        Weapons.EquippedChanged += OnEquippedChanged;
        Weapons.SlotsChanged += OnSlotsChanged;
    }

    // --------- Modifier 라우팅 ---------
    // 유물이 “어디에 붙일지”만 고르면, Inventory가 바인드/재바인드를 책임.
    public ScopedMod ApplyModifier(
        Modifier mod,
        ModTargetKind kind,
        SO_WeaponMetaData targetMeta = null,  // WeaponByMeta / WeaponSkillByMeta 용
        int slotIndex = -1,                   // WeaponSlot 용
        int skillIndex = -1                   // *Skill 용
    )
    {
        var node = new ScopedMod(mod, kind, targetMeta, slotIndex, skillIndex);

        switch (kind)
        {
            case ModTargetKind.Player:
                {
                    (_player as IModifierSink)?.AddModifier(mod);
                    // 정적 스코프라 _dynamicMods에 추가 안 함
                    break;
                }
            case ModTargetKind.CurrentWeapon:
                {
                    var sink = Weapons.Current as IModifierSink;
                    node.Bind(sink);
                    _dynamicMods.Add(node);
                    break;
                }
            case ModTargetKind.CurrentWeaponSkill:
                {
                    var sink = (Weapons.Current)?.GetSkill(skillIndex) as IModifierSink;
                    node.Bind(sink);
                    _dynamicMods.Add(node);
                    break;
                }
            //case ModTargetKind.WeaponSlot:
            //    {
            //        var sink = Weapons.GetSlot(slotIndex) as IModifierSink;
            //        node.Bind(sink); // 슬롯 고정이므로 동적 아님
            //        break;
            //    }
            case ModTargetKind.WeaponByMeta:
                {
                    // ★ 기존 코드 버그: Current.GetSkill(...)로 잘못 바인딩 하고 있었음
                    var sink = Weapons.FindByMeta(targetMeta) as IModifierSink;
                    node.Bind(sink);           // 해당 무기가 없으면 null 바인드(대기 상태)
                    _dynamicMods.Add(node);    // 슬롯 변화 시 자동 재바인딩
                    break;
                }
            case ModTargetKind.WeaponSkillByMeta:
                {
                    var logic = Weapons.FindByMeta(targetMeta);
                    var sink = (logic)?.GetSkill(skillIndex) as IModifierSink;
                    node.Bind(sink);
                    _dynamicMods.Add(node);    // 해당 무기/스킬이 등장/교체되면 재바인딩
                    break;
                }
        }

        return node;
    }

    public ScopedMod ApplyModifierToCurrentWeapon(Modifier m)
    {
        var node = ScopedMod.ForCurrentWeapon(m);
        node.Bind(Weapons.Current as IModifierSink);
        _dynamicMods.Add(node);
        return node;
    }

    public ScopedMod ApplyModifierToWeaponMeta(Modifier m, SO_WeaponMetaData meta)
    {
        var node = ScopedMod.ForWeaponByMeta(m, meta);
        var sink = (Weapons.FindByMeta(meta) as IModifierSink);
        node.Bind(sink);              // 없으면 일단 unbound 상태 유지
        _dynamicMods.Add(node);
        return node;
    }

    public ScopedMod ApplyModifierToWeaponSkillMeta(Modifier m, SO_WeaponMetaData meta, int skillIndex)
    {
        var node = ScopedMod.ForWeaponSkillByMeta(m, meta, skillIndex);
        var logic = Weapons.FindByMeta(meta);
        var sink = (logic)?.GetSkill(skillIndex) as IModifierSink;
        node.Bind(sink);
        _dynamicMods.Add(node);
        return node;
    }
    public void RemoveModifier(ScopedMod node)
    {
        if (node == null) return;

        // 1) 동적이면 먼저 Unbind + 리스트 제거
        if (_dynamicMods.Remove(node))
            node.Unbind();

        // 2) 정적 스코프는 직접 Remove 필요
        switch (node.Kind)
        {
            case ModTargetKind.Player:
                (_player as IModifierSink)?.RemoveModifier(node.Mod);
                break;
            //case ModTargetKind.WeaponSlot:
            //    {
            //        var sink = Weapons.GetSlot(node.SlotIndex) as IModifierSink;
            //        sink?.RemoveModifier(node.Mod);
            //        break;
            //    }
                // CurrentWeapon/ByMeta/Skill*는 Unbind로 이미 제거됨
        }
    }

    // 무기 교체/슬롯 변경 시 동적 스코프 전부 재바인딩
    void RebindDynamicMods()
    {
        // 1) 모두 떼고
        foreach (var n in _dynamicMods) n.Unbind();

        // 2) 새 대상에 다시 붙임
        foreach (var n in _dynamicMods)
        {
            switch (n.Kind)
            {
                case ModTargetKind.CurrentWeapon:
                    n.Bind(Weapons.Current as IModifierSink);
                    break;

                case ModTargetKind.CurrentWeaponSkill:
                    {
                        var sink = (Weapons.Current)?.GetSkill(n.SkillIndex) as IModifierSink;
                        n.Bind(sink);
                        break;
                    }

                case ModTargetKind.WeaponByMeta:
                    {
                        var sink = Weapons.FindByMeta(n.TargetMeta) as IModifierSink;
                        n.Bind(sink);
                        break;
                    }

                case ModTargetKind.WeaponSkillByMeta:
                    {
                        var logic = Weapons.FindByMeta(n.TargetMeta);
                        var sink = (logic)?.GetSkill(n.SkillIndex) as IModifierSink;
                        n.Bind(sink);
                        break;
                    }
            }
        }
    }

    // --------- 컨테이너 이벤트(교차 관심사 처리) ---------
    void OnEquippedChanged()
    {
        RebindDynamicMods();  // 동적 모디 재바인딩
    }

    void OnSlotsChanged()
    {
        // 새/기존 무기 메타 변화 → 메타 기반 모디 다시 붙이기
        RebindDynamicMods();
        // 필요 시: 세트 유물 재평가 등
    }

    // --------- 수명 ---------
    public void Dispose()
    {
        
        if (Weapons != null)
        {
            Weapons.EquippedChanged -= OnEquippedChanged;
            Weapons.SlotsChanged -= OnSlotsChanged;
        }

        _view?.UnbindHandlers();

        Weapons?.Dispose();
        Relics?.Dispose();
    }

    // --------- 틱/입력 래핑 ---------
    public void Tick(float dt)
    {
        Weapons.Tick(dt);
        Relics.Tick(dt);
    }

    // 무기를 장착하는 코드 ( 생성은 끝났음을 가정 )
    public void AcquireWeapon(IWeaponLogic logic, SO_WeaponMetaData meta)
    {
        Weapons.AddWeapon(logic, meta);
        // 컨테이너가 SlotsChanged/EquippedChanged를 올리므로,
        // 아래 호출은 없어도 되지만, View 라우팅을 안전하게 갱신하고 싶다면 유지
    }

    public void SwapWeapon()
    {
        Weapons.SwapWeapon();
        // EquippedChanged 이벤트로 대부분 처리되지만, 한 번 더 안전망
    }

    public void PrimaryDown() => Weapons.OnKeyDown();
    public void PrimaryUp() => Weapons.OnKeyUp();

    public void Skill1Down() => _view.TriggerSkill1Anim(); // 실행은 AE_Skill1에서 라우팅
    public void Skill1Up() => Weapons.Skill1KeyUp();

    public void Skill2Down() => _view.TriggerSkill2Anim();
    public void Skill2Up() => Weapons.Skill2KeyUp();

    // -------- 소비 아이템 ----------
    public void UseConsumableItem(int idx) => Consumables.TryUse(idx);
    public bool TryAddConsumableItem(IConsumableItem item, out int placedIndex) => Consumables.TryAdd(item, out placedIndex);



    // 유물 경유 헬퍼(원래 의도대로 얇게)
    internal void AddRelic(BaseRelic relic) => Relics.AddRelic(relic);
    internal void RemoveRelic(BaseRelic relic) => Relics.RemoveRelic(relic);
}
