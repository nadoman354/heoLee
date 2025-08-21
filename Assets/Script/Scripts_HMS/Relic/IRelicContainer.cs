using System;

public interface IRelicContainer :IDisposable
{
    void Init(Player player);
    void Tick(float dt);
    void AddRelic(BaseRelic relic);
    void RemoveRelic(BaseRelic relic);

    event Action SlotsChanged;     // 슬롯 구성 변경(추가/제거/교체)
}
