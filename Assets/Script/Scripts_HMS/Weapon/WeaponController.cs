using UnityEngine;

/// <summary>
/// * README *
/// 1. WeaponView�� �Բ� �߰� �ؾ� �ϴ� ������Ʈ�̴�.
/// 2. ���� Ŭ������ ���Ⱑ ������ ���� ���� �� �ִ� ����� ������ �Ѵ�.
/// 3. �ش� Ŭ������ Capabilities.cs ������ ���ϰ� ���� �Ǿ� �ִ�.
/// 4. Weapon�� �ʿ��� ������ Init ������ WeaponController�� ���� ���� �´�. [Caps ������ ���]
/// 5. ������ ���� �ʱ�ȭ�� ����Ѵ�. => Inventory���� WeaponController�� ȣ���Ѵ�.
/// </summary>
public sealed class WeaponController : MonoBehaviour
{
    [Header("Scene refs")]
    [SerializeField] WeaponView view;
    [SerializeField] Transform owner;   // �÷��̾� ��Ʈ
    [SerializeField] Player player;   // �÷��̾� ��Ʈ
    [SerializeField] Transform muzzle;  // �ѱ�/�߻� ����
    [SerializeField] Camera cam;

    IWeaponContainer weaponContainer => player.Inventory.Weapons;

    IWeaponLogic logic;
    ICapabilities caps;
    public ICapabilities Caps=>caps;

    // (����) �� Input System �д� ����
    Vector2 ReadStick() => /* e.g., Gamepad.current.rightStick.ReadValue() */ Vector2.zero;

    void Start()
    {
        // 1) Cap ���� (Unity ������ ���⼭��)
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
        // 2) ���� ����(POCO) + �ʱ�ȭ(ĸ/��Ÿ��)
        logic = LogicFactoryHub.WeaponFactory.Create(meta, view, caps);   // ���� + �ʱ�ȭ

        weaponContainer.AddWeapon(logic, meta);// �κ��丮�� �߰�
    }
}
