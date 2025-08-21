using System;
using UnityEngine;

public class DiaSword : IWeaponLogic, IAttackableWeapon, ISkillAnimDriven
{
    [SerializeField]
    SO_SwordMetaData metaData;
    [SerializeReference]
    WeaponView view;
    public WeaponView WeaponView => view;
    [SerializeField]
    float effectSpawnOffset = 1f;

    StatBlock stat = new StatBlock();     // GetFromMeta
    GameObject[] Effect;// GetFromMeta
    int MaxAttackCount = 2;
    int curAttackCount = 0;
    float lastAtkTime = 0;

    ISkillLogic[] baseSkills = new ISkillLogic[2];

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
            OnKeyDown();
        if(Input.GetKeyDown(KeyCode.V))
        {
            Skill1KeyDown();
        }
        Tick(Time.deltaTime);
    }
    public void PlayAttackAnim()
    {
        view.TriggerAttackAnim(curAttackCount);
    } 
    public void AddAtkCount()
    {
        curAttackCount += 1;
        curAttackCount = curAttackCount % MaxAttackCount;
    }
    public void Attack()
    {
        if (Effect.Length < MaxAttackCount)
        {
            Debug.LogWarning("이펙트 프리팹 수가 콤보 수보다 적어요.");
        }
        if (curAttackCount >= Effect.Length) return; // 방어

        GameObject effect = GameObject.Instantiate(Effect[curAttackCount], view.transform.parent.position + view.transform.parent.right * effectSpawnOffset, view.transform.parent.rotation );
        var col = effect.GetComponent<BoxCollider2D>();
        var center = col.transform.TransformPoint(col.offset);                 // 로컬→월드
        var worldSize = Vector2.Scale(col.size, (Vector2)col.transform.lossyScale);
        var angle = col.transform.eulerAngles.z;
        var mask = 1 << LayerMask.NameToLayer("Monster");

        var hits = Physics2D.OverlapBoxAll(center, worldSize, angle, mask);
        foreach (var hit in hits)
        {
            Debug.Log(hit.gameObject.name);
        }

        AddAtkCount();
    }

    public void Initialize(WeaponContext ctx)
    {
        metaData = (SO_SwordMetaData)ctx.metadata;
        view = (WeaponView)ctx.view;

        stat.Init(metaData.stat);
        MaxAttackCount = metaData.MaxAtkCount;
        Effect = metaData.AtkEffectList;

        view.SetAnimator(metaData.attackClipOverride, new SetAnimatorInfo());

        baseSkills[0] = SkillFactory.Instance.CreateSkill(new SkillContext(metaData.skillData[0], ctx.caps));
        baseSkills[1] = SkillFactory.Instance.CreateSkill(new SkillContext(metaData.skillData[1], ctx.caps));
    }

    public void OnKeyDown()
    {
        if (lastAtkTime + stat.GetStat(Stats.BaseWeaponStat.attackSpeed) < Time.time)
        {
            lastAtkTime = Time.time;
            PlayAttackAnim();
        }
    }

    public void OnKeyUp()
    {
    }

    public void Tick(float dt)
    {
        foreach(ISkillLogic baseSkill in baseSkills)
        {
            baseSkill.Tick(dt);
        }
    }
    public void AddModifier(Modifier mod)
    {
        stat.AddModifier(mod);
    }
    public void RemoveModifier(Modifier mod)
    {
        stat.RemoveModifier(mod);
    }

    public void AddSkillModifier(int skillIndex, Modifier mod)
    {
        baseSkills[skillIndex].AddModifier(mod);
    }
    public void RemoveSkillModifier(int skillIndex, Modifier mod)
    {
        baseSkills[skillIndex].RemoveModifier(mod);
    }

    public void Dispose()
    {
    }

    public void Skill1KeyDown()
    {
        if (baseSkills[0].CanUse())
            view.TriggerSkill1Anim(/*baseSkills[0].OnSkill*/);
        //baseSkills[0].OnKeyDown();
    }

    public void Skill2KeyDown()
    {
        if (baseSkills[1].CanUse())
            view.TriggerSkill2Anim(/*baseSkills[1].OnSkill*/);
        //baseSkills[1].OnKeyDown();
    }

    public void Skill1KeyUp()
    {
        baseSkills[0].OnKeyUp();
    }

    public void Skill2KeyUp()
    {
        baseSkills[1].OnKeyUp();
    }

    public void OnSkill1AnimEvent() => baseSkills[0].OnSkill();

    public void OnSkill2AnimEvent() => baseSkills[1].OnSkill();

    public IModifierSink GetSkill(int skillIndex)
    {
        if(skillIndex < 0 || skillIndex >= baseSkills.Length) throw new IndexOutOfRangeException(nameof(skillIndex));
        return baseSkills[skillIndex];
    }
}