using System;
using UnityEngine;

/// <summary>
/// 2개 무기 슬롯 컨테이너
/// - 무기 획득/장착/교체/해제
/// - 슬롯이 가득 찼을 때 새 무기를 얻으면, 현재 장착 중인 무기를 버리고 그 자리에 장착
/// - 비장착 무기도 Tick을 돌아서 스킬 쿨다운이 진행되도록 함
/// - NOTE: 이 클래스는 MonoBehaviour가 아니므로, 외부(플레이어)에서 Tick/입력 래핑을 호출해야 함.
/// </summary>
public class WeaponContainer : IWeaponContainer
{
    const int MAX_WEAPON = 2;
    const int MAX = 2;
    const int FIRST_WEAPON_IDX = 0;
    const int SECOND_WEAPON_IDX = 1;

    // 현재 장착 중인 슬롯(0/1). 하나만 있을 때는 0을 유지.
    int curEquippedWeaponIndex = FIRST_WEAPON_IDX;

    // 무기 로직 보관소(각 요소는 서로 다른 무기 인스턴스)
    readonly IWeaponLogic[] weapon = new IWeaponLogic[MAX_WEAPON];
    readonly SO_WeaponMetaData[] _metas = new SO_WeaponMetaData[MAX];

    // 단일 View를 공유(장착된 무기만 실제로 애니/이펙트 사용)
    // ⚠ View의 애님 이벤트는 "현재 장착 무기"로만 라우팅되도록 설계해야 함.
    //    (ex) View가 Container를 알고 Container.Current에 이벤트 전달
    readonly WeaponView weaponView;

    public event Action EquippedChanged;
    public event Action SlotsChanged;

    public WeaponContainer(WeaponView weaponView)
    {
        this.weaponView = weaponView ?? throw new System.ArgumentNullException(nameof(weaponView));
    }

    /// <summary>
    /// 무기 획득:
    /// 1) 빈 슬롯이 있으면 빈 슬롯에 장착(첫 무기면 당연히 0번 장착)
    /// 2) 두 슬롯이 모두 차있으면, 현재 장착 중인 무기를 버리고 그 자리에 새 무기 장착
    /// </summary>
    /// 
  
    public bool AddWeapon(IWeaponLogic newWeapon, SO_WeaponMetaData metaData)
    {
        if (newWeapon == null) { Debug.LogError("New IWeaponLogic Missing"); return false; }
        if (weapon == null) { Debug.LogError("Slot Missing"); return false; }

        // 1) 빈 슬롯 탐색
        int empty = FindEmptySlot();
        if (empty != -1)
        {
            // 빈 슬롯에 넣기
            PutIntoSlot(empty, newWeapon, metaData);

            // 규칙 2: 획득한 무기가 하나뿐이면(= 지금까지 비어있었으면) 0번 장착 유지
            // - 첫 무기라면 empty는 0이므로 자연스럽게 0번 장착 상태가 됨
            // - 두 번째 무기면 equip 변경 없음
        }

        // 2) 가득 찬 경우 → 현재 장착 무기를 버리고 그 자리에 새 무기
        ReplaceEquipped(newWeapon, metaData);
        return true;
    }

    /// <summary>현재 장착 무기를 버리고 같은 슬롯에 새 무기 삽입</summary>
    private void ReplaceEquipped(IWeaponLogic newWeapon, SO_WeaponMetaData meta)
    {
        int idx = curEquippedWeaponIndex;

        // 기존 무기 정리(Dispose 등)
        DropWeapon(idx);

        // 새 무기 삽입 + 장착 상태 유지
        PutIntoSlot(idx, newWeapon, meta);
    }

    /// <summary>슬롯에 무기를 삽입하고 초기화. 장착 슬롯이면 View로 애니/이벤트를 사용.</summary>
    private void PutIntoSlot(int slotIndex, IWeaponLogic newWeapon, SO_WeaponMetaData meta)
    {
        // 장착 슬롯 여부
        bool isEquippedSlot = (slotIndex == curEquippedWeaponIndex);

        // ⚠ 현재 구조에선 Initialize 시점에 View를 넘김.
        //   비장착 슬롯에도 같은 View를 넘기면 이벤트 중복 구독 위험이 있으니
        //   "이벤트 라우팅을 Container가 담당"하도록 설계하거나,
        //   로직이 장착 시점에만 View 이벤트를 구독하도록 주의.

        //weaponView.BindHandlers()
        weapon[slotIndex] = newWeapon;
        _metas[slotIndex] = meta;
        SlotsChanged?.Invoke();

        // (옵션) View에 무기별 애니 오버라이드를 반영하려면, 장착 슬롯에서만 반영하도록 주의
        if (isEquippedSlot)
        {
            EquipSlot(slotIndex);
            // weaponView.SetAnimator(meta.attackClipOverride, new SetAnimatorInfo()); 
            // ← 필요한 경우 장착 시점에만 적용
        }
    }
    void EquipSlot(int idx)
    {
        curEquippedWeaponIndex = idx;
        var logic = weapon[idx]; // IWeaponLogic

        // 어떤 능력이 있는지에 따라 선택적으로 바인딩
        Action onAtk = (logic is IAttackableWeapon a) ? a.Attack : null;
        Action onS1 = (logic is ISkillAnimDriven s1) ? s1.OnSkill1AnimEvent : null;
        Action onS2 = (logic is ISkillAnimDriven s2) ? s2.OnSkill2AnimEvent : null;

        weaponView.UnbindHandlers();
        weaponView.BindHandlers(onAtk, onS1, onS2);

        // 애니 오버라이드/스킨 적용이 필요하면 여기서
        weaponView.SetAnimator(_metas[idx].attackClipOverride, new SetAnimatorInfo(logic.GetMetaData().zRotationOffset), _metas[idx].spriteClipOverride);
    }

    // 입력 처리 예
    void OnPrimary() => weapon[curEquippedWeaponIndex]?.OnKeyDown();     // 애니 트리거는 로직에서 호출
    void OnSkill1() => weaponView.TriggerSkill1Anim();                // 실제 실행은 AE_Skill1에서 라우팅됨
    void OnSkill2() => weaponView.TriggerSkill2Anim();

    /// <summary>슬롯 무기 제거(버리기). 현재 장착 슬롯을 비웠다면, 남은 무기가 있으면 그쪽을 장착.</summary>
    public void DropWeapon(int index)
    {
        if (index < 0 || index >= MAX_WEAPON) { Debug.LogError("Index Error"); return; }

        var dropTarget = weapon[index];
        if (dropTarget == null) return;

        // 1) 무기 정리: 이벤트 해지/코루틴 정리/풀 반납 등
        dropTarget.Dispose();

        // 2) 슬롯 비우기
        weapon[index] = null;

        // 3) 장착 슬롯을 비웠다면, 나머지 슬롯으로 장착 이동(있다면)
        if (index == curEquippedWeaponIndex)
        {
            int other = (index == FIRST_WEAPON_IDX) ? SECOND_WEAPON_IDX : FIRST_WEAPON_IDX;
            if (weapon[other] != null)
            {
                curEquippedWeaponIndex = other;
                EquipSlot(curEquippedWeaponIndex);
                // (옵션) View에 다른 무기의 애니/스킨 적용
                // weaponView.SetAnimator(otherMeta.attackClipOverride, new SetAnimatorInfo());
            }
            else
            {
                // 둘 다 비었음 → 장착 없음 상태를 표현하고 싶다면 -1 사용
                // 여기서는 0 유지(입력시 null-체크가 있으니 안전)
                // curEquippedWeaponIndex = FIRST_WEAPON_IDX;
            }
        }
    }

    /// <summary>두 슬롯 모두 찬 경우에만 장착 토글(교체)</summary>
    public void SwapWeapon()
    {
        // 규칙 2: 한 개만 있으면 교체 불가
        if (CountOccupied() < 2) return;

        curEquippedWeaponIndex = (curEquippedWeaponIndex == FIRST_WEAPON_IDX)
            ? SECOND_WEAPON_IDX
            : FIRST_WEAPON_IDX;
        EquipSlot(curEquippedWeaponIndex);
        // (옵션) 새로 장착된 무기의 애니 오버라이드/스킨 적용
        // weaponView.SetAnimator(currentMeta.attackClipOverride, new SetAnimatorInfo());
    }

    /// <summary>매 프레임 틱: 모든 무기 로직을 돌려 비장착 무기도 쿨다운 진행</summary>
    public void Tick(float deltaTime)
    {
        for (int i = 0; i < MAX_WEAPON; i++)
        {
            var w = weapon[i];
            if (w == null) continue;
            w.Tick(deltaTime); // 쿨다운/지속효과/차지 등
        }
    }

    /// <summary>현재 장착 무기 입력 래핑</summary>
    public void OnKeyDown() => weapon[curEquippedWeaponIndex]?.OnKeyDown();
    public void OnKeyUp() => weapon[curEquippedWeaponIndex]?.OnKeyUp();

    /// <summary>현재 장착 무기(전역) Modifier</summary>
    public void AddModifier(Modifier mod) => weapon[curEquippedWeaponIndex]?.AddModifier(mod);
    public void RemoveModifier(Modifier mod) => weapon[curEquippedWeaponIndex]?.RemoveModifier(mod);

    /// <summary>현재 장착 무기의 특정 스킬 Modifier</summary>
    public void AddSkillModifier(int skillIndex, Modifier mod)
        => weapon[curEquippedWeaponIndex]?.AddSkillModifier(skillIndex, mod);
    public void RemoveSkillModifier(int skillIndex, Modifier mod)
        => weapon[curEquippedWeaponIndex]?.RemoveSkillModifier(skillIndex, mod);

    /// <summary>현재 장착 무기의 스킬 입력</summary>
    public void Skill1KeyDown() => weapon[curEquippedWeaponIndex]?.Skill1KeyDown();
    public void Skill2KeyDown() => weapon[curEquippedWeaponIndex]?.Skill2KeyDown();
    public void Skill1KeyUp() => weapon[curEquippedWeaponIndex]?.Skill1KeyUp();
    public void Skill2KeyUp() => weapon[curEquippedWeaponIndex]?.Skill2KeyUp();

    // --- helpers ---
    public IWeaponLogic FindByMeta(SO_WeaponMetaData meta)
    {
        // 장착 슬롯 우선
        if (_metas[curEquippedWeaponIndex] == meta) return weapon[curEquippedWeaponIndex];
        int other = curEquippedWeaponIndex == 0 ? 1 : 0;
        if (_metas[other] == meta) return weapon[other];
        return null; // 없으면 null
    }

    int FindEmptySlot()
    {
        if (weapon[FIRST_WEAPON_IDX] == null) return FIRST_WEAPON_IDX;
        if (weapon[SECOND_WEAPON_IDX] == null) return SECOND_WEAPON_IDX;
        return -1;
    }

    int CountOccupied()
    {
        int c = 0;
        if (weapon[FIRST_WEAPON_IDX] != null) c++;
        if (weapon[SECOND_WEAPON_IDX] != null) c++;
        return c;
    }

    internal void Dispose()
    {
        throw new NotImplementedException();
    }

    void IDisposable.Dispose()
    {
        throw new NotImplementedException();
    }

    // (선택) 현재 무기 로직을 외부에서 직접 참조하고 싶을 때
    public IWeaponLogic Current => weapon[curEquippedWeaponIndex];
}
