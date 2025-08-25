using UnityEngine;
using Core.Enums;
using Ink.Parsed;
using System;
using Unity.VisualScripting;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerDash))]
[RequireComponent(typeof(PlayerAnimation))]
public class Player : MonoBehaviour, IModifierSink
{
    private static Player instance;
    public static Player Instancs => instance;
    private PlayerController        input;
    private PlayerMovement          movement;
    private PlayerDash              dash;
    private StatBlock             stats;
    private PlayerWeaponHandler     weaponHandler;
    private Inventory         inventory;
    private PlayerAnimation         anim;
    //private PlayerAudioController   audio;
    public Inventory Inventory => inventory;

    public Health health { get; private set; }

    WeaponController weaponController;
    public WeaponController WeaponController => weaponController;

    float damage => stats.GetStat(Stats.BasicPlayerStat.Damage);

    //여기부턴 임시 변수임!!!
    [SerializeField]
    SO_StatData statData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            instance.transform.position = transform.position;
            Destroy(gameObject);
        }

        Init(statData);

        input           = GetComponent<PlayerController>();
        movement        = GetComponent<PlayerMovement>();
        dash            = GetComponent<PlayerDash>();
        inventory       = GetComponent<Inventory>();
        anim            = GetComponent<PlayerAnimation>();
        weaponHandler   = GetComponent<PlayerWeaponHandler>();
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDeath += HandleDeath;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDeath -= HandleDeath;
    }

    private void Update()
    {
        anim.LookAtMouse(input.mouseWorldPosition);

        if (input.dashPressed && input.moveDirection != Vector2.zero && dash.dashTimer <= 0.0f)
        {
            dash.TryDash(input.moveDirection, input.mouseWorldPosition);
            //stats.Invincible(dash.dashInvincibleTime);
            anim.PlayDashAnim();
        }

        if (input.interactPressed)  { /* TODO : NPC 대화 */ }

        if (input.attackPressed)
        {
            inventory.Weapons.OnKeyDown();
        }
        if(input.skill1Pressed)
        {
            inventory.Skill1Down();
        }
        if (input.skill2Pressed)
        {
            inventory.Skill2Down();
        }
        if (input.item1Pressed)
            inventory.UseConsumableItem(1);
        if(input.item2Pressed)
            inventory.UseConsumableItem(2);
        if(input.item3Pressed)
            inventory.UseConsumableItem(3);
        if(input.item4Pressed)
            inventory.UseConsumableItem(4);

        if (health.Current <= 0) //플레이어 사망
        {
            input.InputLocker(true);
            anim.PlayDieAnim();
        }

        //RelicTest();
        inventory.Tick(Time.deltaTime);
        health.Tick();
    }

    private void FixedUpdate()
    {
        if (!dash.isDashing)
        {
            movement.Move(input.moveDirection, stats.GetStat(Stats.BasicPlayerStat.MoveSpeed), dash.isDashing);
        }

        anim.PlayMoveAnim(input.moveDirection);
    }
    public void Init(SO_StatData data)
    {
        stats = new StatBlock();
        stats.Init(data);
        health = new Health();
        health.Init(stats, Stats.BasicPlayerStat.MaxHp, null, MaxChangePolicy.Clamp);
        inventory = new Inventory(this, GetComponentInChildren<WeaponView>());
        weaponController = GetComponentInChildren<WeaponController>();
    }
    public void AddModifier(Modifier mod) => stats.AddModifier(mod);
    public void RemoveModifier(Modifier mod) => stats.RemoveModifier(mod);

    private void HandleDeath()
    {
        input.InputLocker(true);
        anim.PlayDieAnim();
    }

    public Inventory GetInventory()
    {
        return inventory;
    }

    public void AddRelic(BaseRelic relic) => inventory.TryAddRelic(relic);
    public void RemoveRelic(BaseRelic relic) => inventory.RemoveRelic(relic);

    // 무기 장착 스크립트. 무기 생성은 DroppedWeapon이 수행 후 inv에 전달!
    public void AcquireWeapon(IWeaponLogic logic, SO_WeaponMetaData meta) => inventory.TryAddWeapon(logic, meta);
    public void SwapWeapon() => inventory.SwapWeapon();

    //-------- 소비 아이템 ----------
    public void UseConsumableItem(int idx) => inventory.UseConsumableItem(idx);
    bool TryAddConsumableItem(IConsumableItem item, out int placedIndex) => inventory.TryAddConsumable(item, out placedIndex);
}
