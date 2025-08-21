using Core.Enums;
using UnityEngine;

public class StackCoolTime : BaseCoolTime
{
    float baseCoolTime;
    float timer;
    int maxStack;
    int stack;
    public float RemainTimeRatio => timer / baseCoolTime;// 0이면 쿨타임 다 된거임
    public int Stack => stack;// 0이면 쿨타임 다 된거임
    public StackCoolTime(float BaseCoolTime, int maxStack)
    {
        coolTimeType = SkillCoolTimeType.Stack;
        baseCoolTime = BaseCoolTime;
        this.maxStack = maxStack;
        stack = 0;
    }
    public bool Tick()
    {
        if (stack == maxStack) return CanUse();

        timer -= Time.deltaTime;
        timer = Mathf.Max(timer, 0);
        if (timer <= 0)
        {
            if (stack < maxStack)
            {
                ++stack;
                if (stack != maxStack)
                    ReFill();
            }
        }
        return CanUse();
    }
    public bool CanUse()
    {
        return stack > 0;
    }
    private void ReFill()
    {
        timer = baseCoolTime;
    }
    public void Use()
    {
        if (CanUse())
        {
            if (stack == maxStack)
                ReFill();
            --stack;

        }
    }
}
