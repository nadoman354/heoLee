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
    event Action EquippedChanged;  // ���� ���� ���� ����
    event Action SlotsChanged;     // ���� ���� ����(�߰�/����/��ü)
}
