using UnityEngine;

/// <summary>
/// 테스트 컨트롤러:
/// - B: 보스 드롭
/// - N: 노말 드롭
/// - 1/2/3: GameManager 스테이지 변경
/// - (선택) Start에서 런 Seed 고정
/// </summary>
public class DropTestHarness : MonoBehaviour
{
    [Header("Loot Tables (SO)")]
    [SerializeField] private NormalLootTable normalTable;
    [SerializeField] private BossLootTable bossTable;

    [Header("Drop Center")]
    [Tooltip("비우면 이 컴포넌트의 Transform 위치를 사용")]
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
        // 선택: 이번 판 시드 고정 (재현성 테스트)
        if (setSeedOnStart && RngService.I != null)
        {
            RngService.I.SetRunSeed(runSeed);
            Debug.Log($"[DropTestHarness] RunSeed set to {runSeed}");
        }

        if (!GameManager.I)
            Debug.LogWarning("[DropTestHarness] GameManager가 씬에 없습니다. 스테이지 변경 키(1/2/3)는 무시됩니다.");

        if (!DropSystem.I)
            Debug.LogWarning("[DropTestHarness] DropSystem 싱글톤이 없습니다. 드롭 호출이 실패할 수 있습니다.");
    }

    private void Update()
    {
        // 스테이지 변경 (1~3)
        if (Input.GetKeyDown(KeyCode.Alpha1)) GameManager.I?.SetStage(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) GameManager.I?.SetStage(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) GameManager.I?.SetStage(3);

        // 노말 드롭
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (!normalTable) { Debug.LogWarning("[DropTestHarness] NormalLootTable 미지정"); return; }
            if (!DropSystem.I) { Debug.LogWarning("[DropTestHarness] DropSystem 없음"); return; }

            DropSystem.I.DropNormal(CenterPos, normalTable);
            Debug.Log($"[DropTestHarness] Normal Drop @ {CenterPos} (Stage={GameManager.I?.CurrentStage})");
        }

        // 보스 드롭
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (!bossTable) { Debug.LogWarning("[DropTestHarness] BossLootTable 미지정"); return; }
            if (!DropSystem.I) { Debug.LogWarning("[DropTestHarness] DropSystem 없음"); return; }

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
