using System.Collections.Generic;
using System.Linq;
using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;

namespace KemoCard.Pages;

public partial class AllocPointScene : BaseScene
{
    public uint CurrentPoint = 0;
    public double SpeedPoint = 0;
    public double StrengthPoint = 0;
    public double EfficiencyPoint = 0;
    public double MantraPoint = 0;
    public double CraftEquipPoint = 0;
    public double CraftBookPoint = 0;
    public double CriticalPoint = 0;
    public double DodgePoint = 0;
    [Export] private Label SpeedTxt;
    [Export] private Label StrengthTxt;
    [Export] private Label EfficiencyTxt;
    [Export] private Label MantraTxt;
    [Export] private Label EquipTxt;
    [Export] private Label BookTxt;
    [Export] private Label CriticalTxt;
    [Export] private Label DodgeTxt;
    [Export] private Godot.Button SpeedMinus;
    [Export] private Godot.Button StrengthMinus;
    [Export] private Godot.Button EfficiencyMinus;
    [Export] private Godot.Button MantraMinus;
    [Export] private Godot.Button EquipMinus;
    [Export] private Godot.Button BookMinus;
    [Export] private Godot.Button CriticalMinus;
    [Export] private Godot.Button DodgeMinus;
    [Export] private Godot.Button SpeedPlus;
    [Export] private Godot.Button StrengthPlus;
    [Export] private Godot.Button EfficiencyPlus;
    [Export] private Godot.Button MantraPlus;
    [Export] private Godot.Button EquipPlus;
    [Export] private Godot.Button BookPlus;
    [Export] private Godot.Button CriticalPlus;
    [Export] private Godot.Button DodgePlus;
    [Export] private Godot.Button ConfirmBtn;
    [Export] private Godot.Button CancelBtn;
    [Export] private Godot.Button CheckDeckBtn;
    [Export] private Label RemainPoint;
    private List<Card> Deck = new();
    private PlayerRole player;

    public override void _Ready()
    {
        UpdateView();
        SpeedPlus.Pressed += () => TryPlusProperty("SpeedPoint");
        SpeedMinus.Pressed += () => TryMinusProperty("SpeedPoint");
        StrengthPlus.Pressed += () => TryPlusProperty("StrengthPoint");
        StrengthMinus.Pressed += () => TryMinusProperty("StrengthPoint");
        EfficiencyPlus.Pressed += () => TryPlusProperty("EfficiencyPoint");
        EfficiencyMinus.Pressed += () => TryMinusProperty("EfficiencyPoint");
        MantraPlus.Pressed += () => TryPlusProperty("MantraPoint");
        MantraMinus.Pressed += () => TryMinusProperty("MantraPoint");
        EquipPlus.Pressed += () => TryPlusProperty("CraftEquipPoint");
        EquipMinus.Pressed += () => TryMinusProperty("CraftEquipPoint");
        BookPlus.Pressed += () => TryPlusProperty("CraftBookPoint");
        BookMinus.Pressed += () => TryMinusProperty("CraftBookPoint");
        CriticalPlus.Pressed += () => TryPlusProperty("CriticalPoint");
        CriticalMinus.Pressed += () => TryMinusProperty("CriticalPoint");
        DodgePlus.Pressed += () => TryPlusProperty("DodgePoint");
        DodgeMinus.Pressed += () => TryMinusProperty("DodgePoint");
        ConfirmBtn.Pressed += DoConfirm;
        CancelBtn.Pressed += DoCancel;
    }

    public override void OnAdd(params object[] datas)
    {
        if (datas?[0] is not PlayerRole pr) return;
        player = pr;
        Deck = pr.Deck.ToList();
        CurrentPoint = pr.UnUsedPoints;
        SpeedPoint = pr.OriginSpeed;
        StrengthPoint = pr.OriginStrength;
        EfficiencyPoint = pr.OriginEffeciency;
        MantraPoint = pr.OriginMantra;
        CraftEquipPoint = pr.OriginCraftEquip;
        CraftBookPoint = pr.OriginCraftBook;
        CriticalPoint = pr.OriginCritical;
        DodgePoint = pr.OriginDodge;
        UpdateView();
    }

    private void UpdateView()
    {
        SpeedTxt.Text = SpeedPoint.ToString("N0");
        StrengthTxt.Text = StrengthPoint.ToString("N0");
        EfficiencyTxt.Text = EfficiencyPoint.ToString("N0");
        MantraTxt.Text = MantraPoint.ToString("N0");
        EquipTxt.Text = CraftEquipPoint.ToString("N0");
        BookTxt.Text = CraftBookPoint.ToString("N0");
        CriticalTxt.Text = CriticalPoint.ToString("N0");
        DodgeTxt.Text = DodgePoint.ToString("N0");
        RemainPoint.Text = "剩余点数：" + CurrentPoint.ToString();
    }

    private void TryPlusProperty(string propertyName)
    {
        if (Get(propertyName).Obj != null && CurrentPoint > 0)
        {
            CurrentPoint--;
            var obj = Get(propertyName).Obj;
            if (obj != null) Set(propertyName, (double)obj + 1);
        }

        UpdateView();
    }

    private void TryMinusProperty(string protertyName)
    {
        var v = Get(protertyName);
        if (v.Obj != null)
        {
            if (((double)v) > 0)
            {
                Set(protertyName, ((double)v) - 1);
                CurrentPoint++;
            }
        }

        UpdateView();
    }

    private void DoConfirm()
    {
        AlertView.PopupAlert("是否确定修改？", false, () =>
        {
            if (player == null) return;
            player.OriginSpeed = SpeedPoint;
            player.OriginStrength = StrengthPoint;
            player.OriginEffeciency = EfficiencyPoint;
            player.OriginMantra = MantraPoint;
            player.OriginCraftBook = CraftBookPoint;
            player.OriginCraftEquip = CraftEquipPoint;
            player.OriginCritical = CriticalPoint;
            player.OriginDodge = DodgePoint;
            player.UnUsedPoints = CurrentPoint;
            var node = (MainScene)ResourceLoader.Load<PackedScene>("res://Pages/MainScene.tscn").Instantiate();
            StaticInstance.WindowMgr.ChangeScene(node);
            node.UpdateView();
        });
    }

    private static void DoCancel()
    {
        AlertView.PopupAlert("是否取消修改？未保存的修改不会生效。", false, () =>
        {
            var node = (MainScene)ResourceLoader.Load<PackedScene>("res://Pages/MainScene.tscn").Instantiate();
            StaticInstance.WindowMgr.ChangeScene(node);
        });
    }
}