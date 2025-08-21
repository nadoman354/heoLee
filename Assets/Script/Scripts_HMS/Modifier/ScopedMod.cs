public enum ModTargetKind
{
    Player,
    CurrentWeapon,
    WeaponSlot,              // + slotIndex
    CurrentWeaponSkill,      // + skillIndex
    WeaponByMeta,            // ★ 특정 무기 SO 대상
    WeaponSkillByMeta        // ★ 특정 무기의 특정 스킬
}
public sealed class ScopedMod
{
    public readonly Modifier Mod;
    public readonly ModTargetKind Kind;
    public readonly int SlotIndex;               // WeaponSlot
    public readonly int SkillIndex;              // *Skill
    public readonly SO_WeaponMetaData TargetMeta;// WeaponByMeta/WeaponSkillByMeta

    IModifierSink _bound;                        // 현재 붙어 있는 대상
    private ModTargetKind scope;

    // 공용 생성자(내부)
    public ScopedMod(Modifier mod, ModTargetKind kind,
              SO_WeaponMetaData meta = null, int slotIndex = -1, int skillIndex = -1)
    {
        Mod = mod; Kind = kind; TargetMeta = meta;
        SlotIndex = slotIndex; SkillIndex = skillIndex;
    }

    public ScopedMod(Modifier mod, ModTargetKind scope, int skillIndex)
    {
        Mod = mod;
        this.scope = scope;
        SkillIndex = skillIndex;
    }

    // 팩토리들
    public static ScopedMod ForCurrentWeapon(Modifier m)
        => new ScopedMod(m, ModTargetKind.CurrentWeapon);

    public static ScopedMod ForWeaponSlot(Modifier m, int slotIndex)
        => new ScopedMod(m, ModTargetKind.WeaponSlot, slotIndex: slotIndex);

    public static ScopedMod ForCurrentWeaponSkill(Modifier m, int skillIndex)
        => new ScopedMod(m, ModTargetKind.CurrentWeaponSkill, skillIndex: skillIndex);

    public static ScopedMod ForWeaponByMeta(Modifier m, SO_WeaponMetaData meta)
        => new ScopedMod(m, ModTargetKind.WeaponByMeta, meta: meta);

    public static ScopedMod ForWeaponSkillByMeta(Modifier m, SO_WeaponMetaData meta, int skillIndex)
        => new ScopedMod(m, ModTargetKind.WeaponSkillByMeta, meta: meta, skillIndex: skillIndex);

    public void Bind(IModifierSink sink)
    {
        if (_bound == sink) return;
        Unbind();
        _bound = sink;
        _bound?.AddModifier(Mod);
    }

    public void Unbind()
    {
        if (_bound == null) return;
        _bound.RemoveModifier(Mod);
        _bound = null;
    }
}
