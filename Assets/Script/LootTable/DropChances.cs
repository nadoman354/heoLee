using enums;
using System;
using System.Collections.Generic;
using UnityEngine;

// ����: Ÿ�Ժ� ��� "����" Ȯ��
[Serializable] public class DropCountChance { public int count; [Range(0f, 1f)] public float probability; }

// ���� ��� Ȯ��
[Serializable] public class RelicRarityChance { public RarityType rarity; [Range(0f, 1f)] public float probability; }

// ���������� ���� ��� ���̺�
[Serializable]
public class StageRelicRarityChance
{
    public int stage; // 1..3
    public List<RelicRarityChance> rarityChances = new();
}
