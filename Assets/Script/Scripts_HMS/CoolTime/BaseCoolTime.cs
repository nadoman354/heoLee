using Core.Enums;
using UnityEngine;

namespace Core.Enums
{ public enum SkillCoolTimeType { Basic, Stack, Passive } }
public abstract class BaseCoolTime
{
    protected SkillCoolTimeType coolTimeType;
    public SkillCoolTimeType CoolTimeType => coolTimeType;
}
