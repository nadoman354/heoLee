using System;

public interface IRelicContainer :IDisposable
{
    void Init(Player player);
    void Tick(float dt);
    void AddRelic(BaseRelic relic);
    void RemoveRelic(BaseRelic relic);

    event Action SlotsChanged;     // ���� ���� ����(�߰�/����/��ü)
}
