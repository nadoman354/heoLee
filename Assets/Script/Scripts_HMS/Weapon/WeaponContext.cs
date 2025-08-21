using UnityEngine;

public sealed class WeaponContext
{
    public SO_WeaponMetaData metadata;
    public WeaponView view;
    public readonly ICapabilities caps;
    public WeaponContext(SO_WeaponMetaData metadata, WeaponView view, ICapabilities caps)
    {
        this.metadata = metadata;
        this.view = view;
        this.caps = caps;
    }

    //public StatSystem Stats;          // ����/��ų ����
    //public Transform Muzzle;          // �߻� ����Ʈ ��
    //public Player Owner;              // ������ ����
    //public IAnimEventSource Anim;     // �ִϸ��̼� �̺�Ʈ �긴��
    // �ʿ�� �� �߰�
}