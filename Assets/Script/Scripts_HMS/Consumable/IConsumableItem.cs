public interface IConsumableItem
{
    /// <summary>��� �� ���Կ��� �����ؾ� �ϸ� true ��ȯ</summary>
    bool Use(Player player);

    // (�ɼ�) ����/������/ID�� �ʿ��ϸ� ���⿡ �߰�:
    // string Id { get; }
    // bool IsStackable { get; }
    // int  Stack { get; }
    // int  MaxStack { get; }
    // void AddStack(int amount);
}