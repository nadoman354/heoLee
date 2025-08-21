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

    //public StatSystem Stats;          // 무기/스킬 스탯
    //public Transform Muzzle;          // 발사 포인트 등
    //public Player Owner;              // 소유자 참조
    //public IAnimEventSource Anim;     // 애니메이션 이벤트 브릿지
    // 필요시 더 추가
}