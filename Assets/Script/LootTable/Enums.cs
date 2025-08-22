namespace enums
{
    public enum ItemType
    {
        None = 0,   // 미드롭 라인(“아무 것도 드롭 안 함”)
        Weapon = 1,
        Relic = 2,
        Consumable = 3,
        FieldItem = 4,
        Currency = 5
    }

    public enum InteractState
    {
        Idle,       // 상호작용 아님
        Attack,     // 공격 관련
        Run,        // 이동/달리기
        Damage,     // 피격/위험
        Dash,       // 대시 타이밍
        Talking,    // 대화
        Shopping,   // 상점/거래
        Looting,    // 루팅(드롭 아이템 줍기)
        None        // 특별히 분류 없음(디폴트/비활성)
    }

    public enum RarityType { Normal, Rare, Epic }
}