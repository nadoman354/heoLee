
namespace Core.Interfaces
{
    /*
     * [Core.Interfaces.cs]
     * ����: ���� ���� �������̽�/������ ����.
     *  - IDamageable: HP ���� ���� ������ ���.
     *  - IStaggerable: �׷α�(����ȭ) �������� �ִ� ���.
     *  - AttackerTeam: ������ �� ����(�÷��̾�/��/�߸�).
     *  - AttributeType: �Ӽ� �ý��ۿ� Ÿ��(��/����/����/����).
     *
     * å��:
     *  - ���� �ý����� ���� ���(�������̽�) ����.
     *  - HP�� �׷α� å�� �и�(������ ����).
     *
     * ����:
     *  - ��� HP�� �ְ������ IDamageable ���� �� TakeDamage ȣ��.
     *  - ����� �׷α⸦ �����ٸ� IStaggerable ���� �� AddStagger ȣ��.
     *  - ����/��Ʈ ó�� �������� ���.GetComponent<IDamageable/IStaggerable>()�� ���.
     */

    /// <summary>
    /// HP ���ظ� ������ �� �ִ� ���.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>����� ü���� ���, 0 ���� ���� �����մϴ�.</summary>
        void TakeDamage(float amount);
    }

    /// <summary>
    /// �׷α�(����ȭ) �������� ������ ���.
    /// </summary>
    public interface IStaggerable
    {
        /// <summary>�׷α� ������ �������� ���մϴ�(����� ��ȿ).</summary>
        void AddStagger(float amount);
    }

    /// <summary>
    /// �������� �Ҽ�(����Ʈ ȿ�� �б� � ���).
    /// </summary>
    public enum AttackerTeam
    {
        Player,
        Enemy,
        Neutral
    }

    /// <summary>
    /// �Ӽ� Ÿ��(���Ѵٸ� Ȯ��: Poison, Holy, Dark ��).
    /// None �̸� �Ӽ�/������ ������������ �����ϰų� �⺻ ó���� �Ӵϴ�.
    /// </summary>
    public enum AttributeType
    {
        None,
        Fire,
        Poison,
        Ice,
        Lightning,
        Bleed
    }
}