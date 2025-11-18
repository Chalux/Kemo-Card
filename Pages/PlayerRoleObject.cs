using System.Linq;
using Godot;
using KemoCard.Scripts;

namespace KemoCard.Pages;

public partial class PlayerRoleObject : Control, IEvent
{
    public PlayerRole Data;
    [Export] private ProgressBar _hpProgress;
    [Export] private ProgressBar _mpProgress;
    [Export] private RichTextLabel _hpLabel;
    [Export] private RichTextLabel _mpLabel;
    [Export] private ProgressBar _pbProgress;
    [Export] private ProgressBar _mbProgress;
    [Export] private RichTextLabel _pbLabel;
    [Export] private RichTextLabel _mbLabel;
    [Export] private RichTextLabel _roleName;
    [Export] public HBoxContainer BuffContainer;
    [Export] private TextureRect _selectRect;
    [Export] private Control _mainControl;

    private bool _selectingTarget;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        StaticInstance.EventMgr.RegisterIEvent(this);
        _mainControl.MouseEntered += () =>
        {
            if (_selectingTarget)
            {
                _selectRect.Visible = true;
                BattleStatic.Targets.Add(Data);
                //GD.Print(BattleStatic.Targets.Count);
            }
            else
            {
                StaticInstance.MainRoot.ShowRichHint(Data.GetRichDesc());
            }
        };
        _mainControl.MouseExited += () =>
        {
            if (_selectingTarget)
            {
                _selectRect.Visible = false;
                if (BattleStatic.Targets.Contains(Data))
                {
                    BattleStatic.Targets.Remove(Data);
                }
                //GD.Print(BattleStatic.Targets.Count);
            }

            StaticInstance.MainRoot.HideRichHint();
        };
    }

    public override void _ExitTree()
    {
        StaticInstance.EventMgr.UnregisterIEvent(this);
        base._ExitTree();
    }

    public void InitByPlayerRole(PlayerRole role)
    {
        Data = role;
        _hpProgress.MaxValue = Data.CurrHpLimit;
        _mpProgress.MaxValue = Data.CurrMpLimit;
        _pbProgress.MaxValue = _mbProgress.MaxValue = Data.CurrHpLimit;
        role.RoleObject = this;
        UpdateObject();
    }

    private void UpdateObject(PlayerRole role = null)
    {
        if (role != null)
        {
            Data = role;
        }

        if (Data == null) return;
        _hpProgress.Value = Data.CurrHealth;
        _mpProgress.Value = Data.CurrMagic;
        _pbProgress.Value = Data.CurrPBlock;
        _mbProgress.Value = Data.CurrMBlock;
        _pbLabel.Text = StaticUtils.MakeBBCodeString(Data.CurrPBlock.ToString());
        _mbLabel.Text = StaticUtils.MakeBBCodeString(Data.CurrMBlock.ToString());
        _hpLabel.Text = StaticUtils.MakeBBCodeString(Data.CurrHealth + "/" + Data.CurrHpLimit);
        _mpLabel.Text = StaticUtils.MakeBBCodeString(Data.CurrMagic + "/" + Data.CurrMpLimit);
        _roleName.Text = StaticUtils.MakeBBCodeString(Data.Name);
    }

    public void ReceiveEvent(string @event, params object[] datas)
    {
        switch (@event)
        {
            case "PropertiesChanged":
                UpdateObject();
                break;
            case "StartSelectTarget":
            {
                var type = (StaticEnums.TargetType)datas[0];
                if (type is StaticEnums.TargetType.AllSingle or StaticEnums.TargetType.TeamSingle)
                    _selectingTarget = true;
                break;
            }
            case "SelectTargetOwner":
            {
                var target = (BaseRole)datas[0];
                if (target != null && target == Data)
                {
                    _selectingTarget = false;
                    _selectRect.Visible = true;
                }

                break;
            }
            case "EndSelectTarget":
                _selectingTarget = false;
                _selectRect.Visible = false;
                break;
            case "SelectTargetAll":
            {
                var type = (StaticEnums.TargetType)datas[0];
                if (type is StaticEnums.TargetType.All or StaticEnums.TargetType.TeamAll)
                {
                    _selectingTarget = false;
                    _selectRect.Visible = true;
                }

                break;
            }
        }

        //data?.ReceiveEvent(@event, datas);
        foreach (var buffObject in BuffContainer?.GetChildren().Cast<BuffObject>() ?? [])
        {
            buffObject.ReceiveEvent(@event, datas);
        }

        if (Data == null) return;
        foreach (var equip in Data.EquipDic.Values)
        {
            equip?.EquipScript.ReceiveEvent(@event, datas);
        }
    }

    public void AddBuff(string id, object creator)
    {
        var buffObject =
            (BuffObject)ResourceLoader.Load<PackedScene>("res://Pages/BuffObject.tscn").Instantiate();
        buffObject.Init(id, creator);
        if (buffObject.Data == null) return;
        BuffContainer?.AddChild(buffObject);
        buffObject.Data.Binder = Data;
    }

    public void AddBuff(BuffImplBase buff)
    {
        var buffObject =
            (BuffObject)ResourceLoader.Load<PackedScene>("res://Pages/BuffObject.tscn").Instantiate();
        buffObject.Init(buff);
        if (buffObject.Data == null) return;
        BuffContainer?.AddChild(buffObject);
        buffObject.Data.Binder = Data;
    }

    private void MinusBuffCountAndValue(string id, int count = 0, int value = 0)
    {
        var hasBuff = false;
        BuffImplBase temp = null;
        foreach (var buff in Data.InFightBuffs.Where(buff => buff.BuffId == id))
        {
            hasBuff = true;
            temp = buff;
            break;
        }

        if (!hasBuff) return;
        if (count > 0)
            temp.BuffCount -= count;
        if (value != 0)
        {
            temp.BuffValue -= value;
        }
    }
}