using UnityEngine;
using enums;

public interface IInteractable
{
    void OnPlayerNearby();                 // 플레이어 감지 범위 진입 시 (Player쪽 센서가 호출)
    void OnPlayerLeave();                  // 플레이어 감지 범위 이탈 시
    void OnPlayerInteract(Player player);  // 상호작용 입력 시 (예: E키)

    void GetInteract(string text);         // “획득” 등 안내 텍스트 갱신
    void OnHighlight();                    // 하이라이트 ON
    void OnUnHighlight();                  // 하이라이트 OFF

    bool CanInteract(Player player);       // 지금 상호작용 가능한 상태인지
    InteractState GetInteractType();       // 상호작용 타입(루팅 등)
    string GetInteractDescription();       // UI 표시 문구
}