using UnityEngine;

public sealed class SkillContext
{
    public SO_SkillMetaData metadata;
    public ICapabilities caps;
    //public Transform playerPos;
    //public WeaponView view;
    //public StatSystem Stats;          // ����/��ų ����
    //public Transform Muzzle;          // �߻� ����Ʈ ��
    //public Player Owner;              // ������ ����
    //public IAnimEventSource Anim;     // �ִϸ��̼� �̺�Ʈ �긴��
    // �ʿ�� �� �߰�
    public SkillContext(SO_SkillMetaData metadata, ICapabilities caps)
    {
        this.metadata = metadata;
        this.caps = caps;
        //this.playerPos = playerPos;
        //this.view = view;
    }
}