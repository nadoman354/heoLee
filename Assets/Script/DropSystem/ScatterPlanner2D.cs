using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 착지점 산포 플래너(2D).
/// - 매직넘버 제거: DropScatterSettings로 외부 튜닝
/// - 기존 호출부 호환: 설정 없는 오버로드 그대로 유지
/// </summary>
public static class ScatterPlanner2D
{
    // 수학 상수는 상수로 명시
    private const float GOLDEN_ANGLE_DEG = 137.50776405f;

    // 글로벌 프리셋(선택). 어디선가 SetGlobalSettings로 주입하거나,
    // Resources/Drop/ScatterSettings 에셋을 자동 로드한다.
    private static DropScatterSettings _global;

    /// <summary>전역 프리셋 주입</summary>
    public static void SetGlobalSettings(DropScatterSettings settings) => _global = settings;

    /// <summary>기존 호출부 호환 버전(설정 미지정)</summary>
    public static List<Vector3> GenerateValid(
        int count, Vector3 center, System.Random rng,
        float rMin, float rMax, float minSep,
        LayerMask blockMask, float clearance = 0.28f)
    {
        var settings = GetSettingsOrDefault(null);
        return GenerateValidImpl(count, center, rng, rMin, rMax, minSep, blockMask, clearance, settings);
    }

    /// <summary>설정 지정 버전(권장)</summary>
    public static List<Vector3> GenerateValid(
        int count, Vector3 center, System.Random rng,
        float rMin, float rMax, float minSep,
        LayerMask blockMask, float clearance, DropScatterSettings settings)
    {
        settings = GetSettingsOrDefault(settings);
        return GenerateValidImpl(count, center, rng, rMin, rMax, minSep, blockMask, clearance, settings);
    }

    // ---- 내부 코어 ----
    private static List<Vector3> GenerateValidImpl(
        int count, Vector3 center, System.Random rng,
        float rMin, float rMax, float minSep,
        LayerMask blockMask, float clearance, DropScatterSettings s)
    {
        var result = new List<Vector3>(Mathf.Max(0, count));
        if (count <= 0) return result;

        float sep = Mathf.Max(0f, minSep);
        float clear = Mathf.Max(0f, clearance);

        for (int pass = 0; pass < s.relaxPasses && result.Count < count; pass++)
        {
            float passSep = sep * Mathf.Lerp(1f, 0.7f, s.relaxPasses <= 1 ? 1f : pass / (s.relaxPasses - 1f));
            float passClear = clear * Mathf.Lerp(1f, 0.8f, s.relaxPasses <= 1 ? 1f : pass / (s.relaxPasses - 1f));

            for (int i = result.Count; i < count; i++)
            {
                float baseAngle = ((i * GOLDEN_ANGLE_DEG) + (float)(rng.NextDouble() * s.baseAngleJitter)) % 360f;
                float baseRad = Mathf.Lerp(rMin, rMax, (float)rng.NextDouble());
                Vector3 best = center; bool placed = false;

                for (int t = 0; t < s.localTries && !placed; t++)
                {
                    float ang = baseAngle + t * s.stepAngle;
                    float rad = baseRad + t * (s.radialStepBase + rMax * s.radialStepScale);
                    var pos = Polar(center, ang, rad);
                    if (!IsClear(pos, passClear, blockMask)) continue;
                    if (!IsFarEnough(pos, result, passSep)) continue;
                    best = pos; placed = true; break;
                }

                if (!placed)
                {
                    if (ResolveOutward(center, ref best, passClear, blockMask, s.resolveStep, s.resolveSteps) &&
                        IsFarEnough(best, result, passSep))
                        placed = true;
                }

                if (placed) result.Add(best);
                else
                {
                    if (ResolveOutward(center, ref best, passClear, blockMask, s.fallbackStep, s.fallbackSteps))
                        result.Add(best);
                    else
                        result.Add(center);
                }
            }
        }
        return result;
    }

    // ---- 유틸 ----
    private static Vector3 Polar(Vector3 c, float deg, float r)
    {
        float rad = deg * Mathf.Deg2Rad;
        return c + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * r;
    }

    private static bool IsClear(Vector2 p, float clearance, LayerMask mask)
        => Physics2D.OverlapCircle(p, clearance, mask) == null;

    private static bool IsFarEnough(Vector3 p, List<Vector3> list, float minSep)
    {
        float sq = minSep * minSep;
        for (int i = 0; i < list.Count; i++)
            if ((p - list[i]).sqrMagnitude < sq) return false;
        return true;
    }

    private static bool ResolveOutward(Vector3 center, ref Vector3 p, float clearance, LayerMask mask, float step, int steps)
    {
        Vector3 dir = (p - center);
        if (dir.sqrMagnitude < 1e-4f) dir = Vector3.right;
        dir.Normalize();

        float baseDist = (p - center).magnitude;
        for (int k = 0; k < steps; k++)
        {
            var test = center + dir * (baseDist + step * (k + 1));
            if (IsClear(test, clearance, mask)) { p = test; return true; }
        }
        return false;
    }

    // 설정 가져오기(전역 → 리소스 → 디폴트)
    private static DropScatterSettings GetSettingsOrDefault(DropScatterSettings s)
    {
        if (s) return s;
        if (_global) return _global;

        // Resources에서 기본 프리셋 자동 로드 시도(원하면 파일명/경로 바꿔도 됨)
        var fromRes = Resources.Load<DropScatterSettings>("Drop/ScatterSettings");
        if (fromRes) { _global = fromRes; return _global; }

        // 마지막 수단: 메모리 프리셋 생성(프로젝트 기본값)
        var d = ScriptableObject.CreateInstance<DropScatterSettings>();
        _global = d;
        return _global;
    }
}
