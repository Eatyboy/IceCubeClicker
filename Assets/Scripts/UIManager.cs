using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;
using UnityEditor.SearchService;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.Events;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine.Pool;
using Packages.Rider.Editor.UnitTesting;
using UnityEditor.ShaderKeywordFilter;
using UnityEditor.PackageManager.UI;
using UnityEngine.InputSystem.Composites;
using System.Reflection.Emit;
using Label = UnityEngine.UIElements.Label;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Upgrades upgrades;
    [SerializeField] private IceCube iceCube;
    [SerializeField] private Drill drill;
    [SerializeField] private Sprite[] pickaxeSprites;

    private VisualElement background;

    private VisualElement tooltipWindow;

    private VisualElement pickaxeWindow;

    private Label iceCount;
    private Label height;

    private VisualElement drillWindow;
    private VisualElement drillWindowContent;
    private Button drillButton;
    private Button drillClose;
    private Button upgradesButton;
    private VisualElement upgradesWindow;
    private Button upgradesClose;
    private Button liftButton;

    private Button up;
    private Button down;

    private int liftCost;

    private Label effMinNum;
    private Label strPicNum;
    private Label morCrtNum;
    private Label strCrtNum;

    private Label autMinNum;

    private Label effMinCost;
    private Label strPicCost;
    private Label morCrtCost;
    private Label strCrtCost;

    private Label autMinCost;

    private Button effMinButton;
    private Button strPicButton;
    private Button morCrtButton;
    private Button strCrtButton;

    private Button autMinButton; 

    private Button pickaxeButton;
    private VisualElement pickaxeImage;

    private readonly IDictionary<int, Button> pickSelect = new Dictionary<int, Button>();

    private Button stonePickBuy;
    private Button copperPickBuy;
    private Button bronzePickBuy;
    private Button ironPickBuy;
    private Button steelPickBuy;
    private Button titaniumPickBuy;

    private List<VisualElement> drillPartInventorySlots = new();
    private ScrollView drillPartInventory;
    private List<VisualElement> drillSlots = new();
    public Label chargeText;
    public Button chargeButton;

    private StyleLength drillSlotWidth;
    private StyleLength drillSlotHeight;

    private int stonePickCost;
    private int copperPickCost;
    private int bronzePickCost;
    private int ironPickCost;
    private int steelPickCost;
    private int titaniumPickCost;

    private bool stonePickOwned;
    private bool copperPickOwned;
    private bool bronzePickOwned;
    private bool ironPickOwned;
    private bool steelPickOwned;
    private bool titaniumPickOwned;

    // Start is called before the first frame update
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        background = root.Q<VisualElement>("Background");

        tooltipWindow = root.Q<VisualElement>("tooltipWindow");

        pickaxeWindow = root.Q<VisualElement>("pickaxeWindow");

        iceCount = root.Q<Label>("iceCount");
        height = root.Q<Label>("heightLabel");

        drillWindow = root.Q<VisualElement>("drillWindow");
        drillWindowContent = root.Q<VisualElement>("drillWindowContent");
        drillButton = root.Q<Button>("drillButton");
        drillClose = root.Q<Button>("drillExit");
        upgradesButton = root.Q<Button>("upgradesButton");
        upgradesWindow = root.Q<VisualElement>("upgradesWindow");
        upgradesClose = root.Q<Button>("upgradesExit");
        liftButton = root.Q<Button>("liftButton");

        up = root.Q<Button>("upButton");
        down = root.Q<Button>("downButton");

        effMinNum = root.Q<Label>("effMinNum");
        strPicNum = root.Q<Label>("strPicNum");
        morCrtNum = root.Q<Label>("morCrtNum");
        strCrtNum = root.Q<Label>("strCrtNum");

        autMinNum = root.Q<Label>("autMinNum");

        effMinCost = root.Q<Label>("effMinCost");
        strPicCost = root.Q<Label>("strPicCost");
        morCrtCost = root.Q<Label>("morCrtCost");
        strCrtCost = root.Q<Label>("strCrtCost");

        autMinCost = root.Q<Label>("autMinCost");

        effMinButton = root.Q<Button>("effMinButton");
        strPicButton = root.Q<Button>("strPicButton");
        morCrtButton = root.Q<Button>("morCrtButton");
        strCrtButton = root.Q<Button>("strCrtButton");

        autMinButton = root.Q<Button>("autMinButton");

        pickaxeButton = root.Q<Button>("pickaxeButton");
        pickaxeImage = pickaxeButton.ElementAt(0);

        stonePickBuy = root.Q<Button>("stonePickBuy");
        copperPickBuy = root.Q<Button>("copperPickBuy");
        bronzePickBuy = root.Q<Button>("bronzePickBuy");
        ironPickBuy = root.Q<Button>("ironPickBuy");
        steelPickBuy = root.Q<Button>("steelPickBuy");
        titaniumPickBuy = root.Q<Button>("titaniumPickBuy");

        pickSelect[1] = root.Q<Button>("stonePickSelect");
        pickSelect[2] = root.Q<Button>("copperPickSelect");
        pickSelect[3] = root.Q<Button>("bronzePickSelect");
        pickSelect[4] = root.Q<Button>("ironPickSelect");
        pickSelect[5] = root.Q<Button>("steelPickSelect");
        pickSelect[6] = root.Q<Button>("titaniumPickSelect");

        drillPartInventory = root.Q<ScrollView>("drillPartInventory");
        drillPartInventorySlots = drillPartInventory.Query<VisualElement>(className: "drillInventorySlot").ToList();

        drillSlots.Add(root.Q<VisualElement>("drillBitSlot"));
        drillSlots.Add(root.Q<VisualElement>("batterySlot"));
        drillSlots.Add(root.Q<VisualElement>("motorSlot"));
        drillSlots.Add(root.Q<VisualElement>("gearboxSlot"));

        for (int i = 0; i < drillSlots.Count; ++i)
        {
            drillSlots.ElementAt(i).style.backgroundImage = new(drill.partSprites[i]); 
        }

        chargeText = root.Q<Label>("drillChargeText");
        chargeButton = root.Q<Button>("drillChargeButton");

        drillButton.clicked += DrillWindow;
        drillClose.clicked += DrillWindow;
        chargeButton.clicked += drill.ChargeDrill;
        upgradesButton.clicked += UpgradesWindow;
        upgradesClose.clicked += UpgradesWindow;
        liftButton.clicked += LiftButton;

        up.clicked += Up;
        down.clicked += Down;

        effMinButton.clicked += upgrades.efficientMining;
        strPicButton.clicked += upgrades.strongerPick;
        morCrtButton.clicked += upgrades.critMineChance;
        strCrtButton.clicked += upgrades.critMineDamage;

        autMinButton.clicked += upgrades.autoMiner;

        pickaxeButton.clicked += PickaxeWindow;

        stonePickBuy.clicked += StonePickBuy;
        copperPickBuy.clicked += CopperPickBuy;
        bronzePickBuy.clicked += BronzePickBuy;
        ironPickBuy.clicked += IronPickBuy;
        steelPickBuy.clicked += SteelPickBuy;
        titaniumPickBuy.clicked += TitaniumPickBuy;

        pickSelect[1].clicked += StonePickSelect;
        pickSelect[2].clicked += CopperPickSelect;
        pickSelect[3].clicked += BronzePickSelect;
        pickSelect[4].clicked += IronPickSelect;
        pickSelect[5].clicked += SteelPickSelect;
        pickSelect[6].clicked += TitaniumPickSelect;

        upgradesWindow.visible = false;
        pickaxeWindow.visible = false;
        drillWindow.visible = false;
        up.visible = false;
        down.visible = false;
        drillButton.visible = false;
        //drillPopup.visible = false;

        liftCost = 25;

        stonePickCost = 50;
        copperPickCost = 200;
        bronzePickCost = 1000;
        ironPickCost = 5000;
        steelPickCost = 10000;
        titaniumPickCost = 100000;

        stonePickOwned = false;
        copperPickOwned = false;
        bronzePickOwned = false;
        ironPickOwned = false;
        steelPickOwned = false;
        titaniumPickOwned = false;

        drillButton.text = "Drill: 500 ice";

        stonePickBuy.text = stonePickCost.ToString();
        copperPickBuy.text = copperPickCost.ToString();
        bronzePickBuy.text = bronzePickCost.ToString();
        ironPickBuy.text = ironPickCost.ToString();
        steelPickBuy.text = steelPickCost.ToString();
        titaniumPickBuy.text = titaniumPickCost.ToString();
        background = root.Q<VisualElement>("Background");

        background.RegisterCallback<GeometryChangedEvent>(GeometryChangedCallback);
        drillWindowContent.RegisterCallback<MouseOutEvent>(Unhover);
        drillWindowContent.RegisterCallback<MouseOverEvent>(Hover);
        drillWindowContent.RegisterCallback<MouseDownEvent>(DrillClick);
    }

    private void GeometryChangedCallback(GeometryChangedEvent evt)
    {
        background.UnregisterCallback<GeometryChangedEvent>(GeometryChangedCallback);

        drillSlotWidth = drillPartInventorySlots[0].resolvedStyle.width;
        drillSlotHeight = drillPartInventorySlots[0].resolvedStyle.height;
    }

    private void Unhover(MouseOutEvent evt)
    {
        VisualElement target = (VisualElement)evt.target;
        if (!target.ClassListContains("hasTooltip")) return;
        tooltipWindow.visible = false;
        tooltipWindow.Clear();
        //tooltipWindow.ClearClassList();
        
    }

    private void Hover(MouseOverEvent evt)
    {
        VisualElement target = (VisualElement)evt.target;
        if (!target.ClassListContains("hasTooltip")) return;
        OpenDrillPartTooltip(target, GetPart(target));              
    }

    private void DrillClick(MouseDownEvent evt)
    {
        if (!evt.ctrlKey || evt.button != 0) return;
        VisualElement target = (VisualElement)evt.target;
        if (!target.ClassListContains("drillPart")) return;
        EquipDrillPart(target, GetPart(target));
    }

    private void OpenDrillPartTooltip(VisualElement element, Drill.Part part)
    {
        tooltipWindow.AddToClassList("drillPartTooltip");
        float width = tooltipWindow.resolvedStyle.width;
        Vector2 elementTopLeft = element.parent.LocalToWorld(new Vector2(element.resolvedStyle.left, element.resolvedStyle.top));
        Vector2 elementBottomRight = element.parent.LocalToWorld(new Vector2(element.resolvedStyle.right, element.resolvedStyle.bottom));
        tooltipWindow.style.left = elementTopLeft.x - width < 0
            ? tooltipWindow.style.left = elementBottomRight.x + element.resolvedStyle.width
            : tooltipWindow.style.left = elementTopLeft.x - width;
        tooltipWindow.style.top = elementTopLeft.y;

        Label name = new();
        name.AddToClassList("drillPartTooltipName");
        name.text = part.name;
        VisualElement image = new();
        image.AddToClassList("drillPartTooltipImage");
        image.style.backgroundImage = new(part.sprite);
        Label modifier1 = new();
        modifier1.AddToClassList("drillPartTooltipModifier");
        modifier1.text = part.implicit1.type.ToString() + ": " + part.implicit1.value.ToString();
        Label modifier2 = new();
        modifier2.AddToClassList("drillPartTooltipModifier");
        modifier2.text = part.implicit2.type.ToString() + ": " + part.implicit2.value.ToString();
        tooltipWindow.Add(name);
        tooltipWindow.Add(image);
        tooltipWindow.Add(modifier1);
        tooltipWindow.Add(modifier2);

        tooltipWindow.visible = true;
    }

    private Drill.Part GetPart(VisualElement element)
    {
        switch (element.name)
        {
            case "drillBitSlot": return drill.drillParts[0];
            case "batterySlot": return drill.drillParts[1];
            case "motorSlot": return drill.drillParts[2];
            case "gearboxSlot": return drill.drillParts[3];
        }
        return drill.partInventory[int.Parse(element.name)];
    }

    private void EquipDrillPart (VisualElement element, Drill.Part part)
    {
        int type = (int)part.type;
        int index = int.Parse(element.name);
        StyleBackground tempImage = drillSlots.ElementAt(type).style.backgroundImage;
        Drill.Part tempPart = drill.drillParts[type];
        drill.drillParts[type] = drill.partInventory[index];
        drillSlots.ElementAt(type).style.backgroundImage = element.style.backgroundImage;
        drill.partInventory[index] = tempPart;
        element.style.backgroundImage = tempImage;

        ((Label)tooltipWindow.ElementAt(0)).text = part.name;
        tooltipWindow.ElementAt(1).style.backgroundImage = new(part.sprite);
        ((Label)tooltipWindow.ElementAt(2)).text = part.implicit1.type.ToString() + ": " + part.implicit1.value.ToString();
        ((Label)tooltipWindow.ElementAt(3)).text = part.implicit2.type.ToString() + ": " + part.implicit2.value.ToString();
    }

    private void DrillWindow()
    {
        if (drill.drillOwned)
        {
            if (drillWindow.visible == false)
            {
                drill.SetPartEffects();
                chargeText.text = drill.charge + "/" + drill.maxCharge;
                chargeButton.text = "Charge Drill: " + drill.chargeCost + " ice";
                drillWindow.visible = true;
                return;
            }
            drillWindow.visible = false;
        }
        else
        {
            if (GameManager.Instance.ice >= 500)
            {
                drill.drillOwned = true;
                GameManager.Instance.ice -= 500;
                GameManager.Instance.partDropChance = 0.001f;
                drillButton.text = "Drill";
            }
        }
    }

    public void PartInventorySprite(Sprite partSprite, int index)
    {
        if (index >= drillPartInventorySlots.Count)
        {
            VisualElement newRow = new();
            newRow.AddToClassList("drillInventoryRow");
            drillPartInventory.Add(newRow);
            for (int i = 0; i < 4; ++i)
            {
                VisualElement newSlot = new();
                newSlot.AddToClassList("drillInventorySlot");
                newRow.Add(newSlot);
                drillPartInventorySlots.Add(newSlot);
            }
        }
        VisualElement part = new();
        part.style.backgroundImage = new StyleBackground(partSprite);
        part.name = index.ToString();
        part.AddToClassList("drillPart");
        part.AddToClassList("hasTooltip");
        drillPartInventory.ElementAt(Mathf.FloorToInt(index / 4)).ElementAt(index % 4).Add(part);
    }

    private void UpgradesWindow()
    {
        if (upgradesWindow.visible == false)
        {
            upgradesWindow.visible = true;
            return;
        }
        upgradesWindow.visible = false;
    }

    private void LiftButton()
    {
        if (GameManager.Instance.ice >= liftCost)
        {
            GameManager.Instance.ice -= liftCost;
            GameManager.Instance.maxHeight++;
            //Improve
            liftCost += liftCost;
            up.visible = true;
        }
    }

    private void Up()
    {
        if (GameManager.Instance.maxHeight > GameManager.Instance.height)
        {
            GameManager.Instance.height++;
            iceCube.SetCube();
            down.visible = true;
            if (GameManager.Instance.maxHeight == GameManager.Instance.height)
            {
                up.visible = false;
            }
        }
    }

    private void Down()
    {
        if (GameManager.Instance.height > 0)
        {
            GameManager.Instance.height--;
            iceCube.SetCube();
            up.visible = true;
            if (GameManager.Instance.height == 0)
            {
                down.visible = false;
            }
        }
    }

    private void pickBorder(int pick)
    {
        foreach (KeyValuePair<int, Button> pair in pickSelect)
        {
            if (pair.Key == pick)
            {
                pickSelect[pair.Key].style.borderLeftWidth = 3;
                pickSelect[pair.Key].style.borderRightWidth = 3;
                pickSelect[pair.Key].style.borderBottomWidth = 3;
                pickSelect[pair.Key].style.borderTopWidth = 3;
            }
            else
            {
                pickSelect[pair.Key].style.borderLeftWidth = 0;
                pickSelect[pair.Key].style.borderRightWidth = 0;
                pickSelect[pair.Key].style.borderBottomWidth = 0;
                pickSelect[pair.Key].style.borderTopWidth = 0;
            }
        }
    }

    private void PickaxeWindow()
    {
        if (pickaxeWindow.visible == false)
        {
            pickaxeWindow.visible = true;
            return;
        } 
        pickaxeWindow.visible = false;
    }

    private void StonePickBuy()
    {
        if (GameManager.Instance.ice >= stonePickCost)
        {
            GameManager.Instance.ice -= stonePickCost;
            stonePickOwned = true;
            StonePickSelect();
            pickSelect[1].style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
            stonePickBuy.text = "Owned";
        }
    }
    private void CopperPickBuy()
    {
        if (GameManager.Instance.ice >= copperPickCost)
        {
            GameManager.Instance.ice -= copperPickCost;
            copperPickOwned = true;
            CopperPickSelect();
            pickSelect[2].style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
            copperPickBuy.text = "Owned";
        }
    }
    private void BronzePickBuy()
    {
        if (GameManager.Instance.ice >= bronzePickCost)
        {
            GameManager.Instance.ice -= bronzePickCost;
            bronzePickOwned = true;
            BronzePickSelect();
            pickSelect[3].style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
            bronzePickBuy.text = "Owned";
        }
    }
    private void IronPickBuy()
    {
        if (GameManager.Instance.ice >= ironPickCost)
        {
            GameManager.Instance.ice -= ironPickCost;
            ironPickOwned = true;
            IronPickSelect();
            pickSelect[4].style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
            ironPickBuy.text = "Owned";
        }
    }
    private void SteelPickBuy()
    {
        if (GameManager.Instance.ice >= steelPickCost)
        {
            GameManager.Instance.ice -= steelPickCost;
            steelPickOwned = true;
            SteelPickSelect();
            pickSelect[5].style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
            steelPickBuy.text = "Owned";
        }
    }
    private void TitaniumPickBuy()
    {
        if (GameManager.Instance.ice >= titaniumPickCost)
        {
            GameManager.Instance.ice -= titaniumPickCost;
            titaniumPickOwned = true;
            TitaniumPickSelect();
            pickSelect[6].style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
            titaniumPickBuy.text = "Owned";
        }
    }

    private void StonePickSelect()
    {
        if (stonePickOwned == true)
        {
            GameManager.Instance.currentPickaxe = 1;
            GameManager.Instance.basePickPower = GameManager.Instance.pickaxePowers[1];
            pickBorder(1);
        }
    }
    private void CopperPickSelect()
    {
        if (copperPickOwned == true)
        {
            GameManager.Instance.currentPickaxe = 2;
            GameManager.Instance.basePickPower = GameManager.Instance.pickaxePowers[2];
            pickBorder(2);
        }
    }
    private void BronzePickSelect()
    {
        if (bronzePickOwned == true)
        {
            GameManager.Instance.currentPickaxe = 3;
            GameManager.Instance.basePickPower = GameManager.Instance.pickaxePowers[3];
            pickBorder(3);
        }
    }
    private void IronPickSelect()
    {
        if (ironPickOwned == true)
        {
            GameManager.Instance.currentPickaxe = 4;
            GameManager.Instance.basePickPower = GameManager.Instance.pickaxePowers[4];
            pickBorder(4);
        }
    }
    private void SteelPickSelect()
    {
        if (steelPickOwned == true)
        {
            GameManager.Instance.currentPickaxe = 5;
            GameManager.Instance.basePickPower = GameManager.Instance.pickaxePowers[5];
            pickBorder(5);
        }
    }
    private void TitaniumPickSelect()
    {
        if (titaniumPickOwned == true)
        {
            GameManager.Instance.currentPickaxe = 6;
            GameManager.Instance.basePickPower = GameManager.Instance.pickaxePowers[6];
            pickBorder(6);
        }
    }

    // Update is called once per frame
    private void OnGUI()
    {
        iceCount.text = GameManager.Instance.ice.ToString();
        height.text = "Height: "+ GameManager.Instance.height.ToString();
        liftButton.text = "Lift: " + liftCost + " ice";

        effMinNum.text = upgrades.efficientMiningLvl.ToString();
        strPicNum.text = upgrades.strongerPickLvl.ToString();
        morCrtNum.text = upgrades.critMineChanceLvl.ToString();
        strCrtNum.text = upgrades.critMineDamageLvl.ToString();

        autMinNum.text = upgrades.autoMinerLvl.ToString();

        effMinCost.text = upgrades.efficientMiningCost.ToString();
        strPicCost.text = upgrades.strongerPickCost.ToString();
        morCrtCost.text = upgrades.critMineChanceCost.ToString();
        strCrtCost.text = upgrades.critMineDamageCost.ToString();

        autMinCost.text = upgrades.autoMinerCost.ToString();

        pickaxeImage.style.backgroundImage = new StyleBackground(pickaxeSprites[GameManager.Instance.currentPickaxe]);
    }

    private void Update()
    {
        if (GameManager.Instance.ice >= 500)
        {
            drillButton.visible = true;
        }
    }
}
