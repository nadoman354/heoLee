using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }

    public int CurrentStage { get; private set; } = 1;

    // 엔트로피 소스
    public int GlobalDropIndex { get; private set; } = 0; // 노말 드롭용
    public int BossClearIndex { get; private set; } = 0; // 보스 드롭용

    private const int MinStage = 1;
    private const int MaxStage = 3;

    private void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this; DontDestroyOnLoad(gameObject);
    }

    public void SetStage(int stage)
        => CurrentStage = Mathf.Clamp(stage, MinStage, MaxStage);

    // DropSystem이 내부에서 호출
    public int NextDropEntropy() => GlobalDropIndex++;
    public int NextBossEntropy() => BossClearIndex++;
}
