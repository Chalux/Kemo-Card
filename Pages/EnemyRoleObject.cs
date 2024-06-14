using Godot;
using KemoCard.Scripts;
using StaticClass;
using System.Linq;
using static StaticClass.StaticEnums;

public partial class EnemyRoleObject : Control, IEvent
{
    public EnemyRole data;
    [Export] private ProgressBar hpProgress;
    [Export] private ProgressBar mpProgress;
    [Export] private RichTextLabel hpLabel;
    [Export] private RichTextLabel mpLabel;
    [Export] private ProgressBar PBProgress;
    [Export] private ProgressBar MBProgress;
    [Export] private RichTextLabel PBLabel;
    [Export] private RichTextLabel MBLabel;
    [Export] public AnimationPlayer hitFlashAnimationPlayer;
    [Export] private RichTextLabel monsterName;
    [Export] private AnimatedSprite2D animation;
    [Export] public HBoxContainer buffContainer;
    [Export] private TextureRect SelectRect;
    [Export] private Control mainControl;
    [Export] public RichTextLabel IntentRichLabel;

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
        });
    }

    public override void _ExitTree()
    {
        StaticInstance.eventMgr.UnregistIEvent(this);
        base._ExitTree();
    }

    public void Init(uint id)
    {
        data = new(id);
        monsterName.Text = StaticUtils.MakeBBCodeString(data.name);
        hpProgress.MaxValue = data.CurrHpLimit;
        mpProgress.MaxValue = data.CurrMpLimit;
        PBProgress.MaxValue = MBProgress.MaxValue = data.CurrHpLimit;
        data.roleObject = this;
        var res = ResourceLoader.Load<SpriteFrames>(data.script.AnimationResourcePath);
        if (res != null)
        {
            animation.SpriteFrames = res;
        }
        UpdateObject();
    }

    public void Init(EnemyRole enemyRole)
    {
        data = enemyRole;
        monsterName.Text = StaticUtils.MakeBBCodeString(data.name);
        hpProgress.MaxValue = data.CurrHpLimit;
        mpProgress.MaxValue = data.CurrMpLimit;
        PBProgress.MaxValue = MBProgress.MaxValue = data.CurrHpLimit;
        enemyRole.roleObject = this;
        var res = ResourceLoader.Load<SpriteFrames>(data.script.AnimationResourcePath);
        if (res != null)
        {
            animation.SpriteFrames = res;
        }
        UpdateObject();
    }

    public void UpdateObject(EnemyRole role = null)
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
        mpLabel.Text = StaticUtils.MakeBBCodeString(data.CurrMagic + "/" + data.CurrHpLimit);
    }

    public void ReceiveEvent(string @event, dynamic datas)
    {
        if (@event == "PropertiesChanged")
        {
            UpdateObject();
        }
        else if (@event == "StartSelectTarget")
        {
            var type = (TargetType)datas;
            if (type == TargetType.ALL_SINGLE || type == TargetType.ENEMY_SINGLE)
                SelectingTarget = true;
        }
        else if (@event == "EndSelectTarget")
        {
            SelectingTarget = false;
            SelectRect.Visible = false;
        }
        else if (@event == "SelectTargetAll")
        {
            var type = (TargetType)datas;
            if (type == TargetType.ENEMY_ALL || type == TargetType.ALL)
            {
                SelectingTarget = false;
                SelectRect.Visible = true;
            }
        }
        data?.ReceiveEvent(@event, datas);
        foreach (BuffObject buffObject in buffContainer.GetChildren().Cast<BuffObject>())
        {
            buffObject.ReceiveEvent(@event, datas);
        }
    }

    public void AddBuff(uint id)
    {
        BuffObject bobj = (BuffObject)ResourceLoader.Load<PackedScene>("res://Pages/BuffObject.tscn").Instantiate();
        bobj.Init(id);
        if (bobj.data != null)
        {
            bobj.data.Binder = data;
            bobj.data.BuffObj = bobj;
            bobj.data.OnBuffAdded?.Invoke(bobj.data);
            data.buffs.Add(bobj.data);
            data.InFightBuffs.Add(bobj.data);
            buffContainer.AddChild(bobj);
        }
    }
}
