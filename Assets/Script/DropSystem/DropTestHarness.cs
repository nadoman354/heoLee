using UnityEngine;

/// <summary>
/// �׽�Ʈ ��Ʈ�ѷ�:
/// - B: ���� ���
/// - N: �븻 ���
/// - 1/2/3: GameManager �������� ����
/// - (����) Start���� �� Seed ����
/// </summary>
public class DropTestHarness : MonoBehaviour
{
    [Header("Loot Tables (SO)")]
    [SerializeField] private NormalLootTable normalTable;
    [SerializeField] private BossLootTable bossTable;

    [Header("Drop Center")]
    [Tooltip("���� �� ������Ʈ�� Transform ��ġ�� ���")]
    [SerializeField] private Transform dropCenter;

    [Header("Run Seed (Optional)")]
    [SerializeField] private bool setSeedOnStart = true;
    [SerializeField] private int runSeed = 12345;

    [Header("Gizmo")]
    [SerializeField] private bool drawGizmo = true;
    [SerializeField] private float gizmoRadius = 0.35f;

    private Vector3 CenterPos => dropCenter ? dropCenter.position : transform.position;

    private void Start()
    {
        // ����: �̹� �� �õ� ���� (������ �׽�Ʈ)
        if (setSeedOnStart && RngService.I != null)
        {
            RngService.I.SetRunSeed(runSeed);
            Debug.Log($"[DropTestHarness] RunSeed set to {runSeed}");
        }

        if (!GameManager.I)
            Debug.LogWarning("[DropTestHarness] GameManager�� ���� �����ϴ�. �������� ���� Ű(1/2/3)�� ���õ˴ϴ�.");

        if (!DropSystem.I)
            Debug.LogWarning("[DropTestHarness] DropSystem �̱����� �����ϴ�. ��� ȣ���� ������ �� �ֽ��ϴ�.");
    }

    private void Update()
    {
        // �������� ���� (1~3)
        if (Input.GetKeyDown(KeyCode.Alpha1)) GameManager.I?.SetStage(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) GameManager.I?.SetStage(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) GameManager.I?.SetStage(3);

        // �븻 ���
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (!normalTable) { Debug.LogWarning("[DropTestHarness] NormalLootTable ������"); return; }
            if (!DropSystem.I) { Debug.LogWarning("[DropTestHarness] DropSystem ����"); return; }

            DropSystem.I.DropNormal(CenterPos, normalTable);
            Debug.Log($"[DropTestHarness] Normal Drop @ {CenterPos} (Stage={GameManager.I?.CurrentStage})");
        }

        // ���� ���
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (!bossTable) { Debug.LogWarning("[DropTestHarness] BossLootTable ������"); return; }
            if (!DropSystem.I) { Debug.LogWarning("[DropTestHarness] DropSystem ����"); return; }

            DropSystem.I.DropBoss(CenterPos, bossTable);
            Debug.Log($"[DropTestHarness] Boss Drop @ {CenterPos} (Stage={GameManager.I?.CurrentStage})");
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmo) return;
        var p = dropCenter ? dropCenter.position : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(p, gizmoRadius);
        Gizmos.DrawRay(p, Vector3.up * (gizmoRadius * 1.2f));
    }
}
