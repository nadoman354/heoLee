using UnityEngine;

/// <summary>
/// * README *
/// 1. WeaponView와 함께 추가 해야 하는 컴포넌트이다.
/// 2. 순수 클래스인 무기가 정보를 제공 받을 수 있는 어댑터 역할을 한다.
/// 3. 해당 클래스와 Capabilities.cs 문서는 강하게 연결 되어 있다.
/// 4. Weapon은 필요한 정보를 Init 시점에 WeaponController로 부터 꺼내 온다. [Caps 변수를 사용]
/// 5. 무기의 최종 초기화를 담당한다. => Inventory에서 WeaponController를 호출한다.
/// </summary>
public sealed class WeaponController : MonoBehaviour
{
    [Header("Scene refs")]
    [SerializeField] WeaponView view;
    [SerializeField] Transform owner;   // 플레이어 루트
    [SerializeField] Player player;   // 플레이어 루트
    [SerializeField] Transform muzzle;  // 총구/발사 원점
    [SerializeField] Camera cam;

    IWeaponContainer weaponContainer => player.Inventory.Weapons;

    IWeaponLogic logic;
    ICapabilities caps;
    public ICapabilities Caps=>caps;

    // (선택) 신 Input System 읽는 람다
    Vector2 ReadStick() => /* e.g., Gamepad.current.rightStick.ReadValue() */ Vector2.zero;

    void Start()
    {
        // 1) Cap 조립 (Unity 의존을 여기서만)
        var anchors = new ViewAnchors(muzzle, owner);
        var mouseAim = new MouseAimProvider(cam, () => (Vector2)owner.position);
        var stickAim = new StickAimProvider(ReadStick);
        var aim = new CombinedAimProvider(stickAim, mouseAim);

        caps = new Capabilities()
            .Add<IAnchors>(anchors)
            .Add<IAimProvider>(aim)
            .Add<ITimeSource>(new UnityTimeSource())
            .Add<ISpawner>(new DefaultSpawner())
            .Add<ICollisionQuery>(new Physics2DQuery());

    }
    public void CreateWeapon(SO_WeaponMetaData meta)
    {
        // 2) 로직 생성(POCO) + 초기화(캡/메타만)
        logic = LogicFactoryHub.WeaponFactory.Create(meta, view, caps);   // 생성 + 초기화

        weaponContainer.AddWeapon(logic, meta);// 인벤토리에 추가
    }
}
