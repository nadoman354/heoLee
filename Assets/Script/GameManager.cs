using UnityEngine;

/// <summary>
/// ��Ÿ�� ���� �������� ������. (1~3)
/// - �̱���
/// - �� ��ȯ���� ����
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }

    /// <summary>���� ��������(1..3)</summary>
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
    /// �������� ���� (Ŭ���� ����)
    /// </summary>
    public void SetStage(int stage)
    {
        CurrentStage = Mathf.Clamp(stage, MinStage, MaxStage);
        // �ʿ��ϸ� ���⼭ UI ����/�α� ����� �� ó��
        // Debug.Log($"[GameManager] Stage = {CurrentStage}");
    }
}
