using Core.Interfaces;

/// <summary>
/// 공격자(플레이어/보스/무기 등)가 구현.
/// 요청된 속성(type)에 대해 누적된 수정치(AttributeMods)를 제공.
/// </summary>

public interface IAttributeModifierProvider
{
    AttributeMods GetAttributeMods(AttributeType type);
}