/*
 * [BossPatternSet.cs]
 * 무엇: 보스 패턴 데이터 세트(SO).
 *  - HP% 구간(Phase)별 패턴 목록 + 가중치/쿨다운.
 *  - 특수 트리거 패턴 목록.
 *
 * 책임:
 *  - 디자이너 데이터로 보스 패턴 로테이션 구성.
 *
 * 사용법:
 *  - BossEnemy에 연결, PatternRunner가 이 데이터를 참조해 선택/실행.
 */


using System.Collections;
using UnityEngine;

public abstract class BossPatternBase : ScriptableObject
{
    protected System.Action onFinishedCallback;
    public void SetFinishCallback(System.Action cb) => onFinishedCallback = cb;

    public virtual void Enter(BossBase boss) { }
    public virtual void UpdatePattern(BossBase boss) { }
    public virtual void Exit(BossBase boss) { }

    public virtual IEnumerator RunOnce(BossBase boss) { yield break; }

    protected void FinishPattern() => onFinishedCallback?.Invoke();
}
