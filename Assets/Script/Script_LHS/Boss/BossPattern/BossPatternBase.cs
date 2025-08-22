/*
 * [BossPatternSet.cs]
 * ����: ���� ���� ������ ��Ʈ(SO).
 *  - HP% ����(Phase)�� ���� ��� + ����ġ/��ٿ�.
 *  - Ư�� Ʈ���� ���� ���.
 *
 * å��:
 *  - �����̳� �����ͷ� ���� ���� �����̼� ����.
 *
 * ����:
 *  - BossEnemy�� ����, PatternRunner�� �� �����͸� ������ ����/����.
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
