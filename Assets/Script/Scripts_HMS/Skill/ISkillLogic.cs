using UnityEngine;

public interface ISkillLogic : System.IDisposable, IModifierSink
{
    void Initialize(SkillContext ctx);      // Player, StatSystem, View µÓ ¡÷¿‘
    bool CanUse();
    void OnSkill();
    void OnKeyUp();
    void Tick(float dt);
}