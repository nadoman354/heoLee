using UnityEngine;

/// <summary>
/// 런타임 전역 스테이지 보관소. (1~3)
/// - 싱글톤
/// - 씬 전환에도 유지
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }

    /// <summary>현재 스테이지(1..3)</summary>
    public int CurrentStage { get; private set; } = 1;

    private const int MinStage = 1;
    private const int MaxStage = 3;

    private void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 스테이지 설정 (클램프 적용)
    /// </summary>
    public void SetStage(int stage)
    {
        CurrentStage = Mathf.Clamp(stage, MinStage, MaxStage);
        // 필요하면 여기서 UI 갱신/로그 남기기 등 처리
        // Debug.Log($"[GameManager] Stage = {CurrentStage}");
    }
}
