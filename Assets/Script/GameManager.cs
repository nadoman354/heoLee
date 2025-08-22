using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }

    public int CurrentStage { get; private set; } = 1;

    // ��Ʈ���� �ҽ�
    public int GlobalDropIndex { get; private set; } = 0; // �븻 ��ӿ�
    public int BossClearIndex { get; private set; } = 0; // ���� ��ӿ�

    private const int MinStage = 1;
    private const int MaxStage = 3;

    private void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this; DontDestroyOnLoad(gameObject);
    }

    public void SetStage(int stage)
        => CurrentStage = Mathf.Clamp(stage, MinStage, MaxStage);

    // DropSystem�� ���ο��� ȣ��
    public int NextDropEntropy() => GlobalDropIndex++;
    public int NextBossEntropy() => BossClearIndex++;
}
