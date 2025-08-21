// Modifier 받는 클래스는 모두 상속 받아야 하는 클래스임
public interface IModifierSink
{
    void AddModifier(Modifier m);
    void RemoveModifier(Modifier m);
}