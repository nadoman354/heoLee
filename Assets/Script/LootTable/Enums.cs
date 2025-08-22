namespace enums
{
    public enum ItemType
    {
        None = 0,   // �̵�� ����(���ƹ� �͵� ��� �� �ԡ�)
        Weapon = 1,
        Relic = 2,
        Consumable = 3,
        FieldItem = 4,
        Currency = 5
    }

    public enum InteractState
    {
        Idle,       // ��ȣ�ۿ� �ƴ�
        Attack,     // ���� ����
        Run,        // �̵�/�޸���
        Damage,     // �ǰ�/����
        Dash,       // ��� Ÿ�̹�
        Talking,    // ��ȭ
        Shopping,   // ����/�ŷ�
        Looting,    // ����(��� ������ �ݱ�)
        None        // Ư���� �з� ����(����Ʈ/��Ȱ��)
    }

    public enum RarityType { Normal, Rare, Epic }
}