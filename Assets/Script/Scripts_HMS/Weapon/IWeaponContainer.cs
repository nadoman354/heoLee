using System;

public interface IWeaponContainer : IDisposable
{
    IWeaponLogic Current { get; }
    void AddWeapon(IWeaponLogic logic, SO_WeaponMetaData meta);
    void DropWeapon(int idx);
    void SwapWeapon();
    void Tick(float dt);
    void OnKeyDown();
    void OnKeyUp();
    void Skill1KeyUp();
    void Skill2KeyUp();
    IWeaponLogic FindByMeta(SO_WeaponMetaData meta);
    event Action EquippedChanged;  // 현재 장착 슬롯 변경
    event Action SlotsChanged;     // 슬롯 구성 변경(추가/제거/교체)
}
