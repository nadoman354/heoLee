using UnityEngine;

/// <summary>
/// "한 번의 공격/적용"에 반영할 모든 수정치를 담는 컨테이너.
/// - 곱(×)과 가산(+)을 구분하여 밸런싱 실수를 줄입니다.
/// - 기본값은 '영향 없음' (곱=1, 가산=0).
/// - 일부 필드는 AttributeDamageHandler가 직접 쓰지 않고 다른 시스템에서 소비됩니다(각 주석 참고).
/// </summary>
public struct AttributeMods
{
    // ========= [공격 공통] =========
    /// <summary>기본 피해 배율(전역/속성 공통). 예) 1.10 = +10%.</summary>
    public float baseDamageMul;

    /// <summary>속성 게이지 누적 배율(전역/기본값). 예) 0.8 = 20% 덜 쌓임.</summary>
    public float gaugeGainMul;

    /// <summary>발현(게이지 만충) 추가 피해 배율. 예) 1.25 = +25%.</summary>
    public float activatedDamageMul;

    // ========= [게이지: 속성별 가중치(옵션)] =========
    /// <summary>독 게이지 누적 배율(개별). 0이면 미설정으로 간주.</summary>
    public float poisonGaugeGainMul;

    /// <summary>얼음 게이지 누적 배율(개별). 0이면 미설정으로 간주.</summary>
    public float iceGaugeGainMul;

    /// <summary>출혈 게이지 누적 배율(개별). 0이면 미설정으로 간주.</summary>
    public float bleedGaugeGainMul;

    // ========= [DOT: 독/출혈 등] =========
    /// <summary>DOT 초당 데미지 가산(+). 예) +8 dps.</summary>
    public float dotDpsAdd;

    /// <summary>DOT 지속 시간 가산(+초). 예) +1.5s.</summary>
    public float dotDurAdd;

    // ========= [이속/공속 슬로우] =========
    /// <summary>이속 감속 강도 가산(+퍼센트포인트). 예) +0.15 = +15%p.</summary>
    public float moveSlowAdd;

    /// <summary>이속 감속 지속 가산(+초).</summary>
    public float moveSlowDurAdd;

    /// <summary>공속 감속 강도 가산(+퍼센트포인트).</summary>
    public float atkSlowAdd;

    /// <summary>공속 감속 지속 가산(+초).</summary>
    public float atkSlowDurAdd;

    // ========= [크리티컬] =========
    /// <summary>크리 확률 가산(+퍼센트포인트). 예) +0.25 = +25%p.</summary>
    public float critChanceAdd;

    /// <summary>크리 배율 곱(×). 예) 1.15 = +15% 크리 데미지.</summary>
    public float critMultiplierMul;

    // ========= [전투 스탯(다른 시스템에서 소비)] =========
    /// <summary>공격 속도 배율(무기 스윙/재장전/쿨타임 등에서 소비). 예) 1.2 = 20% 빠름.</summary>
    public float attackSpeedMul;

    /// <summary>힘(공격력) 가산. DamageRequest.baseDamage에 더하는 용도.</summary>
    public float strengthAdd;

    /// <summary>최대 체력 배율(Stat/Health 시스템에서 소비). 예) 1.1 = MaxHP +10%.</summary>
    public float maxHealthMul;

    /// <summary>최대 체력 가산(절대값). 예) +25 HP.</summary>
    public float maxHealthAdd;

    // ========= [기본값 프리셋] =========
    public static AttributeMods Identity => new AttributeMods
    {
        baseDamageMul = 1f,
        gaugeGainMul = 1f,
        activatedDamageMul = 1f,

        poisonGaugeGainMul = 0f,
        iceGaugeGainMul = 0f,
        bleedGaugeGainMul = 0f,

        dotDpsAdd = 0f,
        dotDurAdd = 0f,

        moveSlowAdd = 0f,
        moveSlowDurAdd = 0f,
        atkSlowAdd = 0f,
        atkSlowDurAdd = 0f,

        critChanceAdd = 0f,
        critMultiplierMul = 1f,

        attackSpeedMul = 1f,
        strengthAdd = 0f,
        maxHealthMul = 1f,
        maxHealthAdd = 0f
    };

    /// <summary>
    /// 다른 수정치와 합성(누적). 곱은 곱끼리, 가산은 가산끼리.
    /// 0 또는 1의 중립값은 무시(덮어쓰기 대신 누적 안전).
    /// </summary>
    public void Combine(in AttributeMods o)
    {
        // 곱
        baseDamageMul *= (o.baseDamageMul == 0f ? 1f : o.baseDamageMul);
        gaugeGainMul *= (o.gaugeGainMul == 0f ? 1f : o.gaugeGainMul);
        activatedDamageMul *= (o.activatedDamageMul == 0f ? 1f : o.activatedDamageMul);

        critMultiplierMul *= (o.critMultiplierMul == 0f ? 1f : o.critMultiplierMul);
        attackSpeedMul *= (o.attackSpeedMul == 0f ? 1f : o.attackSpeedMul);
        maxHealthMul *= (o.maxHealthMul == 0f ? 1f : o.maxHealthMul);

        // 개별 게이지 배율(0이면 미설정 → 합성 시 덮어쓰지 않음)
        if (o.poisonGaugeGainMul != 0f) poisonGaugeGainMul = (poisonGaugeGainMul == 0f ? 1f : poisonGaugeGainMul) * o.poisonGaugeGainMul;
        if (o.iceGaugeGainMul != 0f) iceGaugeGainMul = (iceGaugeGainMul == 0f ? 1f : iceGaugeGainMul) * o.iceGaugeGainMul;
        if (o.bleedGaugeGainMul != 0f) bleedGaugeGainMul = (bleedGaugeGainMul == 0f ? 1f : bleedGaugeGainMul) * o.bleedGaugeGainMul;

        // 가산
        dotDpsAdd += o.dotDpsAdd;
        dotDurAdd += o.dotDurAdd;
        moveSlowAdd += o.moveSlowAdd;
        moveSlowDurAdd += o.moveSlowDurAdd;
        atkSlowAdd += o.atkSlowAdd;
        atkSlowDurAdd += o.atkSlowDurAdd;
        critChanceAdd += o.critChanceAdd;
        strengthAdd += o.strengthAdd;
        maxHealthAdd += o.maxHealthAdd;
    }
}
