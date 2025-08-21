using UnityEngine;

[CreateAssetMenu(fileName = "DropScatterSettings", menuName = "Game/Drop/ScatterSettings")]
public class DropScatterSettings : ScriptableObject
{
    [Header("Search Budget")]
    [Min(1)] public int localTries = 14; // 후보 지점 로컬 시도 횟수
    [Min(1)] public int relaxPasses = 3;  // 제약 완화 패스 횟수

    [Header("Angular / Radial Jitter")]
    public float baseAngleJitter = 23f;   // 기본 각도 지터
    public float stepAngle = 9.5f;  // 각도 보정 스텝
    public float radialStepBase = 0.06f; // 반경 증가 기본값
    public float radialStepScale = 0.02f; // rMax 비례 증가 비율

    [Header("Clearance Resolve (primary)")]
    public float resolveStep = 0.18f;   // 바깥으로 밀 때 한 스텝 거리
    public int resolveSteps = 16;      // 스텝 수

    [Header("Clearance Resolve (fallback)")]
    public float fallbackStep = 0.20f;   // 2차 시도 스텝 거리
    public int fallbackSteps = 24;      // 스텝 수
}
