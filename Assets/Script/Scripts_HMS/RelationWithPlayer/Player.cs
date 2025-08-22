using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IModifierSink
{
    StatBlock statBlock;
    public Health health { get; private set; }

    Inventory inventory;
    public Inventory Inventory=>inventory;
    WeaponController weaponController;
    public WeaponController WeaponController => weaponController;

    float damage => statBlock.GetStat(Stats.BasicPlayerStat.Damage);

    //여기부턴 임시 변수임!!!
    [SerializeField]
    SO_StatData statData;

    private void Awake()
    {
        Init(statData);
    }
    public void Init(SO_StatData data)
    {
        statBlock = new StatBlock();
        statBlock.Init(data);
        health = new Health();
        health.Init(statBlock);
        inventory = new Inventory(this, GetComponentInChildren<WeaponView>());
        weaponController = GetComponentInChildren<WeaponController>();
    }
    public void AddModifier(Modifier mod) => statBlock.AddModifier(mod);
    public void RemoveModifier(Modifier mod) => statBlock.RemoveModifier(mod);

    public void Update()
    {
        //RelicTest();
        inventory.Tick(Time.deltaTime);
        health.Tick();
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
