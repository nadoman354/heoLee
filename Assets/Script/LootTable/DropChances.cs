using enums;
using System;
using System.Collections.Generic;
using UnityEngine;

// 보스: 타입별 드롭 "개수" 확률
[Serializable] public class DropCountChance { public int count; [Range(0f, 1f)] public float probability; }

// 유물 등급 확률
[Serializable] public class RelicRarityChance { public RarityType rarity; [Range(0f, 1f)] public float probability; }

// 스테이지별 유물 등급 테이블
[Serializable]
public class StageRelicRarityChance
{
    public int stage; // 1..3
    public List<RelicRarityChance> rarityChances = new();
}
