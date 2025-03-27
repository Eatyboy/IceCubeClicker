using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;
using static UnityEditor.Experimental.GraphView.Port;

public class Drill : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private IceCube iceCube;
    [SerializeField] public Sprite[] partSprites;

    public bool drillOwned;

    public int drillPower;
    private const int baseCharge = 100;
    public int charge;
    public int maxCharge;
    public int batteryDrain;
    public int chargeCost;

    private int damage;
    private float batteryEfficiency;
    private float batteryCapacity;
    private float speed;
    public float partDropChance;
    public int bonusIce;

    public enum PartType
    {
        drillBit,
        battery,
        motor,
        gearbox
    }

    public enum ModifierType
    {
        damage,
        efficiency,
        capacity,
        speed,
        partDropChance,
        bonusIce
    }

    public struct Modifier
    {
        public float value;
        public ModifierType type;

        public Modifier(float value, ModifierType type)
        {
            this.value = value;
            this.type = type;
        }
    }

    private struct Range
    {
        public float min;
        public float max;

        public Range(float max, float min)
        {
            this.min = min;
            this.max = max;
        }
    }

    private readonly Range[] implicitRanges = { new(20f,30f), new(0.1f,0.3f), new(1.3f,1.7f), new(1.1f,1.5f), new(0.01f,0.05f), new(2f,5f) };
    private readonly int[] implicitRoundingDigits = { 0, 2, 2, 2, 3, 0 };
    private readonly string[] implicit1Names = { "damaging", "battery efficient", "battery capacity", "speedy", "lucky", "icy" };
    private readonly string[] implicit2Names = { "damage", "battery efficiency", "battery", "speed", "luck", "extra ice" };

    public class Part
    {
        public string name;
        public Sprite sprite;
        public PartType type;
        public Modifier implicit1;
        public Modifier implicit2;

        public Part(string name, Sprite sprite, PartType type, Modifier implicit1, Modifier implicit2)
        {
            this.name = name;
            this.type = type;
            this.sprite = sprite;
            this.implicit1 = implicit1;
            this.implicit2 = implicit2;
        }
    }

    public Part[] drillParts = new Part[4];

    public List<Part> partInventory = new();

    public Part GeneratePart()
    {
        ModifierType[,] modifiers =
        {
            {ModifierType.damage, ModifierType.partDropChance},
            {ModifierType.capacity, ModifierType.efficiency},
            {ModifierType.damage, ModifierType.speed},
            {ModifierType.speed, ModifierType.bonusIce}
        };
        int partType = Random.Range(0, 4);
        ModifierType implicit1Type = modifiers[partType, Random.Range(0, 2)];
        ModifierType implicit2Type = modifiers[partType, Random.Range(0, 2)];
        float implicit1Value = (float)System.Math.Round((double)Random.Range(implicitRanges[(int)implicit1Type].min, implicitRanges[(int)implicit1Type].max), implicitRoundingDigits[(int)implicit1Type]);
        float implicit2Value = (float)System.Math.Round((double)Random.Range(implicitRanges[(int)implicit2Type].min, implicitRanges[(int)implicit2Type].max), implicitRoundingDigits[(int)implicit2Type]);
        string name = implicit1Names[(int)implicit1Type] + " " + ((PartType)partType).ToString() + " of " + implicit2Names[(int)implicit2Type];
        return new Part(name, partSprites[partType], (PartType)partType, new(implicit1Value, implicit1Type), new(implicit2Value, implicit2Type));
    }

    public void AddPartToInventory(Part part)
    {
        partInventory.Add(part);
        uiManager.PartInventorySprite(part.sprite, partInventory.Count - 1);
    }

    // Start is called before the first frame update
    void Start()
    {
        drillOwned = false;
        drillParts[0] = new("Rusty DrillBit", partSprites[0], PartType.drillBit, new(1, ModifierType.damage), new(0.005f, ModifierType.partDropChance));
        drillParts[1] = new("Rusty Battery", partSprites[1], PartType.battery, new(1, ModifierType.capacity), new(1, ModifierType.efficiency));
        drillParts[2] = new("Rusty Motor", partSprites[2], PartType.motor, new(1, ModifierType.damage), new(1, ModifierType.speed));
        drillParts[3] = new("Rusty Gearbox", partSprites[3], PartType.gearbox, new(1, ModifierType.speed), new(1, ModifierType.bonusIce));
        charge = baseCharge;
        chargeCost = 100;
    }

    public void SetPartEffects()
    {
        damage = 0;
        batteryEfficiency = 0;
        batteryCapacity = 0;
        bonusIce = 0;
        speed = 0;
        partDropChance = 0;
        
        foreach (Part part in drillParts)
        {
            switch (part.implicit1.type)
            {
                case ModifierType.damage: { damage += (int)part.implicit1.value; } break;
                case ModifierType.efficiency: { batteryEfficiency += part.implicit1.value;  } break;
                case ModifierType.capacity: { batteryCapacity = (int)part.implicit1.value; } break;
                case ModifierType.bonusIce: { bonusIce += (int)part.implicit1.value; } break;
                case ModifierType.speed: { speed += part.implicit1.value; } break;
                case ModifierType.partDropChance: { partDropChance += part.implicit1.value; } break;
                default: { throw new Exception(part.name + " has an invalid type for implicit1"); }
            }
            switch (part.implicit2.type)
            {
                case ModifierType.damage: { damage += (int)part.implicit2.value; } break;
                case ModifierType.efficiency: { batteryEfficiency += part.implicit2.value; } break;
                case ModifierType.capacity: { batteryCapacity += (int)part.implicit2.value; } break;
                case ModifierType.bonusIce: { bonusIce += (int)part.implicit2.value; } break;
                case ModifierType.speed: { speed += part.implicit2.value; } break;
                case ModifierType.partDropChance: { partDropChance += part.implicit2.value; } break;
                default: { throw new Exception(part.name + " has an invalid type for implicit2"); }
            }
        }
        drillPower = damage;
        maxCharge = (int)(baseCharge * batteryCapacity);
        InvokeRepeating(nameof(iceCube.StartDrill), 0f, 1f / speed);
    }

    public void ChargeDrill()
    {
        if (GameManager.Instance.ice < chargeCost) return;
        GameManager.Instance.ice -= chargeCost;
        if (maxCharge - charge < 50) charge += 50;
        else charge = maxCharge;
    }
}
