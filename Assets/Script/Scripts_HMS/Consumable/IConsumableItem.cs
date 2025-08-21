public interface IConsumableItem
{
    /// <summary>사용 후 슬롯에서 제거해야 하면 true 반환</summary>
    bool Use(Player player);

    // (옵션) 스택/아이콘/ID가 필요하면 여기에 추가:
    // string Id { get; }
    // bool IsStackable { get; }
    // int  Stack { get; }
    // int  MaxStack { get; }
    // void AddStack(int amount);
}