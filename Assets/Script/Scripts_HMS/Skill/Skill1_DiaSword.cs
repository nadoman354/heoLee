using UnityEngine;

public class Skill1_DiaSword : ISkillLogic
{
    SO_SkillMetaData_SingleProjectile metaData;
    StatBlock statBlock;
    StackCoolTime coolTimeLogic;

    GameObject skillProjectile;

    // ctx로 부터 데이터 전달 받음
    IAnchors anchors = null;
    Vector2 muzzlePos => anchors.MuzzlePos;
    Quaternion muzzleRot => anchors.MuzzleRot;
    public void Dispose()
    {
    }

    public void Initialize(SkillContext ctx)
    {
        metaData = (SO_SkillMetaData_SingleProjectile)ctx.metadata;
        statBlock = new StatBlock();
        statBlock.Init(metaData.stat);

        coolTimeLogic = new StackCoolTime(statBlock.GetStat(Stats.BaseSkillStat.coolTime),(int)statBlock.GetStat(Stats.BaseSkillStat.maxStack));
        ctx.caps.TryGet<IAnchors>(out IAnchors anchors);
        this.anchors = anchors;
    }

    public void OnKeyUp()
    {
    }

    public void Tick(float dt)
    {
        coolTimeLogic.Tick();
    }
    public void AddModifier(Modifier mod)
    {
        statBlock.AddModifier(mod);
    }
    public void RemoveModifier(Modifier mod)
    {
        statBlock.RemoveModifier(mod);
    }

    public void OnSkill()
    {
        if (coolTimeLogic.CanUse())
        {
            coolTimeLogic.Use();
            Bullet bullet = BulletPool.Instance.Get();
            metaData.projectile.damage = (int)statBlock.GetStat(Stats.BaseSkillStat.damage);
            bullet.Init(metaData.projectile, muzzlePos, muzzleRot);
        }
    }

    public bool CanUse()
    {
        return coolTimeLogic.CanUse();
    }
}