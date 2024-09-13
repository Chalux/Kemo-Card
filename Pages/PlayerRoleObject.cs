using Godot;
using KemoCard.Scripts;
using StaticClass;
using System.Linq;

public partial class PlayerRoleObject : Control, IEvent
{
    public PlayerRole data;
    [Export] private ProgressBar hpProgress;
    [Export] private ProgressBar mpProgress;
    [Export] private RichTextLabel hpLabel;
    [Export] private RichTextLabel mpLabel;
    [Export] private ProgressBar PBProgress;
    [Export] private ProgressBar MBProgress;
    [Export] private RichTextLabel PBLabel;
    [Export] private RichTextLabel MBLabel;
    [Export] private RichTextLabel roleName;
    [Export] public HBoxContainer buffContainer;
    [Export] private TextureRect SelectRect;
    [Export] private Control mainControl;

    private bool SelectingTarget = false;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        StaticInstance.eventMgr.RegistIEvent(this);
        mainControl.MouseEntered += new(() =>
        {
            if (SelectingTarget)
            {
                SelectRect.Visible = true;
                BattleStatic.Targets.Add(data);
                //GD.Print(BattleStatic.Targets.Count);
            }
            else
            {
                StaticInstance.MainRoot.ShowRichHint(data.GetRichDesc());
            }
        });
        mainControl.MouseExited += new(() =>
        {
            if (SelectingTarget)
            {
                SelectRect.Visible = false;
                if (BattleStatic.Targets.Contains(data))
                {
                    BattleStatic.Targets.Remove(data);
                }
                //GD.Print(BattleStatic.Targets.Count);
            }
            StaticInstance.MainRoot.HideRichHint();
        });
    }

    public override void _ExitTree()
    {
        StaticInstance.eventMgr.UnregistIEvent(this);
        base._ExitTree();
    }

    public void InitByPlayerRole(PlayerRole role)
    {
        data = role;
        hpProgress.MaxValue = data.CurrHpLimit;
        mpProgress.MaxValue = data.CurrMpLimit;
        PBProgress.MaxValue = MBProgress.MaxValue = data.CurrHpLimit;
        role.roleObject = this;
        UpdateObject();
    }

    public void UpdateObject(PlayerRole role = null)
    {
        if (role != null)
        {
            data = role;
        }
        if (data == null) return;
        hpProgress.Value = data.CurrHealth;
        mpProgress.Value = data.CurrMagic;
        PBProgress.Value = data.CurrPBlock;
        MBProgress.Value = data.CurrMBlock;
        PBLabel.Text = StaticUtils.MakeBBCodeString(data.CurrPBlock.ToString());
        MBLabel.Text = StaticUtils.MakeBBCodeString(data.CurrMBlock.ToString());
        hpLabel.Text = StaticUtils.MakeBBCodeString(data.CurrHealth + "/" + data.CurrHpLimit);
        mpLabel.Text = StaticUtils.MakeBBCodeString(data.CurrMagic + "/" + data.CurrMpLimit);
        roleName.Text = StaticUtils.MakeBBCodeString(data.name);
    }

    public void ReceiveEvent(string @event, params object[] datas)
    {
        if (@event == "PropertiesChanged")
        {
            UpdateObject();
        }
        else if (@event == "StartSelectTarget")
        {
            var type = (StaticEnums.TargetType)datas[0];
            if (type == StaticEnums.TargetType.ALL_SINGLE || type == StaticEnums.TargetType.TEAM_SINGLE)
                SelectingTarget = true;
        }
        else if (@event == "SelectTargetOwner")
        {
            var target = (BaseRole)datas[0];
            if (target != null && target == data)
            {
                SelectingTarget = false;
                SelectRect.Visible = true;
            }
        }
        else if (@event == "EndSelectTarget")
        {
            SelectingTarget = false;
            SelectRect.Visible = false;
        }
        else if (@event == "SelectTargetAll")
        {
            var type = (StaticEnums.TargetType)datas[0];
            if (type == StaticEnums.TargetType.ALL || type == StaticEnums.TargetType.TEAM_ALL)
            {
                SelectingTarget = false;
                SelectRect.Visible = true;
            }
        }
        //data?.ReceiveEvent(@event, datas);
        foreach (BuffObject buffObject in buffContainer?.GetChildren().Cast<BuffObject>())
        {
            buffObject.ReceiveEvent(@event, datas);
        }
        if (data != null)
        {
            foreach (Equip equip in data.EquipDic.Values)
            {
                equip?.EquipScript.ReceiveEvent(@event, datas);
            }
        }
    }

    public void AddBuff(string id, object Creator)
    {
        BuffObject buffObject = (BuffObject)ResourceLoader.Load<PackedScene>("res://Pages/BuffObject.tscn").Instantiate();
        buffObject.Init(id, Creator);
        if (buffObject.data != null)
        {
            buffContainer?.AddChild(buffObject);
            buffObject.data.Binder = data;
        }
    }

    public void AddBuff(BuffImplBase buff)
    {
        BuffObject buffObject = (BuffObject)ResourceLoader.Load<PackedScene>("res://Pages/BuffObject.tscn").Instantiate();
        buffObject.Init(buff);
        if (buffObject.data != null)
        {
            buffContainer?.AddChild(buffObject);
            buffObject.data.Binder = data;
        }
    }

    public void MinusBuffCountAndValue(string id, int count = 0, int value = 0)
    {
        bool HasBuff = false;
        BuffImplBase temp = null;
        foreach (var buff in data.InFightBuffs)
        {
            if (buff.BuffId == id)
            {
                HasBuff = true;
                temp = buff;
                break;
            }
        }
        if (HasBuff)
        {
            if (count > 0)
                temp.BuffCount -= count;
            if (value != 0)
            {
                temp.BuffValue -= value;
            }
        }
    }
}
