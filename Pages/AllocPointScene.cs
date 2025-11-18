using Godot;
using KemoCard.Scripts;

namespace KemoCard.Pages;

public partial class AllocPointScene : BaseScene
{
    private uint _currentPoint;
    private double _speedPoint;
    private double _strengthPoint;
    private double _efficiencyPoint;
    private double _mantraPoint;
    private double _craftEquipPoint;
    private double _craftBookPoint;
    private double _criticalPoint;
    private double _dodgePoint;
    [Export] private Label _speedTxt;
    [Export] private Label _strengthTxt;
    [Export] private Label _efficiencyTxt;
    [Export] private Label _mantraTxt;
    [Export] private Label _equipTxt;
    [Export] private Label _bookTxt;
    [Export] private Label _criticalTxt;
    [Export] private Label _dodgeTxt;
    [Export] private Button _speedMinus;
    [Export] private Button _strengthMinus;
    [Export] private Button _efficiencyMinus;
    [Export] private Button _mantraMinus;
    [Export] private Button _equipMinus;
    [Export] private Button _bookMinus;
    [Export] private Button _criticalMinus;
    [Export] private Button _dodgeMinus;
    [Export] private Button _speedPlus;
    [Export] private Button _strengthPlus;
    [Export] private Button _efficiencyPlus;
    [Export] private Button _mantraPlus;
    [Export] private Button _equipPlus;
    [Export] private Button _bookPlus;
    [Export] private Button _criticalPlus;
    [Export] private Button _dodgePlus;
    [Export] private Button _confirmBtn;
    [Export] private Button _cancelBtn;
    [Export] private Button _checkDeckBtn;

    [Export] private Label _remainPoint;

    // private List<Card> _deck = [];
    private PlayerRole _player;

    public override void _Ready()
    {
        UpdateView();
        _speedPlus.Pressed += () => TryPlusProperty("SpeedPoint");
        _speedMinus.Pressed += () => TryMinusProperty("SpeedPoint");
        _strengthPlus.Pressed += () => TryPlusProperty("StrengthPoint");
        _strengthMinus.Pressed += () => TryMinusProperty("StrengthPoint");
        _efficiencyPlus.Pressed += () => TryPlusProperty("EfficiencyPoint");
        _efficiencyMinus.Pressed += () => TryMinusProperty("EfficiencyPoint");
        _mantraPlus.Pressed += () => TryPlusProperty("MantraPoint");
        _mantraMinus.Pressed += () => TryMinusProperty("MantraPoint");
        _equipPlus.Pressed += () => TryPlusProperty("CraftEquipPoint");
        _equipMinus.Pressed += () => TryMinusProperty("CraftEquipPoint");
        _bookPlus.Pressed += () => TryPlusProperty("CraftBookPoint");
        _bookMinus.Pressed += () => TryMinusProperty("CraftBookPoint");
        _criticalPlus.Pressed += () => TryPlusProperty("CriticalPoint");
        _criticalMinus.Pressed += () => TryMinusProperty("CriticalPoint");
        _dodgePlus.Pressed += () => TryPlusProperty("DodgePoint");
        _dodgeMinus.Pressed += () => TryMinusProperty("DodgePoint");
        _confirmBtn.Pressed += DoConfirm;
        _cancelBtn.Pressed += DoCancel;
    }

    public override void OnAdd(params object[] datas)
    {
        if (datas?[0] is not PlayerRole pr) return;
        _player = pr;
        // _deck = pr.Deck.ToList();
        _currentPoint = pr.UnUsedPoints;
        _speedPoint = pr.OriginSpeed;
        _strengthPoint = pr.OriginStrength;
        _efficiencyPoint = pr.OriginEffeciency;
        _mantraPoint = pr.OriginMantra;
        _craftEquipPoint = pr.OriginCraftEquip;
        _craftBookPoint = pr.OriginCraftBook;
        _criticalPoint = pr.OriginCritical;
        _dodgePoint = pr.OriginDodge;
        UpdateView();
    }

    private void UpdateView()
    {
        _speedTxt.Text = _speedPoint.ToString("N0");
        _strengthTxt.Text = _strengthPoint.ToString("N0");
        _efficiencyTxt.Text = _efficiencyPoint.ToString("N0");
        _mantraTxt.Text = _mantraPoint.ToString("N0");
        _equipTxt.Text = _craftEquipPoint.ToString("N0");
        _bookTxt.Text = _craftBookPoint.ToString("N0");
        _criticalTxt.Text = _criticalPoint.ToString("N0");
        _dodgeTxt.Text = _dodgePoint.ToString("N0");
        _remainPoint.Text = "剩余点数：" + _currentPoint;
    }

    private void TryPlusProperty(string propertyName)
    {
        if (Get(propertyName).Obj != null && _currentPoint > 0)
        {
            _currentPoint--;
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
                _currentPoint++;
            }
        }

        UpdateView();
    }

    private void DoConfirm()
    {
        AlertView.PopupAlert("是否确定修改？", false, () =>
        {
            if (_player == null) return;
            _player.OriginSpeed = _speedPoint;
            _player.OriginStrength = _strengthPoint;
            _player.OriginEffeciency = _efficiencyPoint;
            _player.OriginMantra = _mantraPoint;
            _player.OriginCraftBook = _craftBookPoint;
            _player.OriginCraftEquip = _craftEquipPoint;
            _player.OriginCritical = _criticalPoint;
            _player.OriginDodge = _dodgePoint;
            _player.UnUsedPoints = _currentPoint;
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