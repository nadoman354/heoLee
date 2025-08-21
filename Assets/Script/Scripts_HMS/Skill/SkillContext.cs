using UnityEngine;

public sealed class SkillContext
{
    public SO_SkillMetaData metadata;
    public ICapabilities caps;
    //public Transform playerPos;
    //public WeaponView view;
    //public StatSystem Stats;          // 무기/스킬 스탯
    //public Transform Muzzle;          // 발사 포인트 등
    //public Player Owner;              // 소유자 참조
    //public IAnimEventSource Anim;     // 애니메이션 이벤트 브릿지
    // 필요시 더 추가
    public SkillContext(SO_SkillMetaData metadata, ICapabilities caps)
    {
        this.metadata = metadata;
        this.caps = caps;
        //this.playerPos = playerPos;
        //this.view = view;
    }
}