public interface IWeaponLogic : System.IDisposable, IModifierSink
{
    void Initialize(WeaponContext ctx);      // Player, StatSystem, View µÓ ¡÷¿‘
    void OnKeyDown();
    void OnKeyUp();
    void Tick(float dt);
    WeaponView WeaponView { get;}

    public void AddSkillModifier(int skillIndex,Modifier mod);
    public void RemoveSkillModifier(int skillIndex, Modifier mod);


    void Skill1KeyDown();
    void Skill2KeyDown();
    void Skill1KeyUp();
    void Skill2KeyUp();
    IModifierSink GetSkill(int skillIndex);
}