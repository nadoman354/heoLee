using UnityEngine;

public static class LogicFactoryHub
{
    private static ILogicWeaponFactory weaponFactory = new LogicWeaponFactory();
    public static ILogicWeaponFactory WeaponFactory => weaponFactory;
    private static ILogicRelicFactory relicFactory = new LogicRelicFactory();
    public static ILogicRelicFactory RelicFactory => relicFactory;
    private static ILogicSkillFactory skillFactory = new LogicSkillFactory();
    public static ILogicSkillFactory SkillFactory => skillFactory;
}