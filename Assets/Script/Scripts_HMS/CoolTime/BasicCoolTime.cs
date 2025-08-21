using Core.Enums;
using UnityEngine;

public class BasicCoolTime : BaseCoolTime
{
    float baseCoolTime;
    float timer;
    public float RemainTimeRatio => timer / baseCoolTime;// 0�̸� ��Ÿ�� �� �Ȱ���
    public BasicCoolTime(float BaseCoolTime)
    {
        coolTimeType = SkillCoolTimeType.Basic;
        SetFullCoolTime(BaseCoolTime);
        timer = baseCoolTime;
    }
    public void SetFullCoolTime(float NewBaseCoolTime)
    {
        this.baseCoolTime = NewBaseCoolTime;
    }
    public bool Tick()
    {
        timer -= Time.deltaTime;
        timer = Mathf.Max(timer, 0);
        return CanUse();
    }
    public bool CanUse()
    {
        return timer <= 0;
    }
    public void Use()
    {
        if (CanUse())
            timer = baseCoolTime;
    }
    public void Reduce(float ratio)// 0.1f �� 10% ��Ÿ�� ���� 
    {
        timer -= timer * ratio;
        timer = Mathf.Max(timer, 0);
    }

}
