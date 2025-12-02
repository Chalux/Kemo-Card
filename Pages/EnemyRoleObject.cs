using System.Linq;
using Godot;
using KemoCard.Scripts;
using static KemoCard.Scripts.StaticEnums;

namespace KemoCard.Pages;

public partial class EnemyRoleObject : Control, IEvent
{
    public EnemyRole Data;
    [Export] private ProgressBar _hpProgress;
    [Export] private ProgressBar _mpProgress;
    [Export] private RichTextLabel _hpLabel;
    [Export] private RichTextLabel _mpLabel;
    [Export] private ProgressBar _pbProgress;
    [Export] private ProgressBar _mbProgress;
    [Export] private RichTextLabel _pbLabel;
    [Export] private RichTextLabel _mbLabel;
    [Export] public AnimationPlayer HitFlashAnimationPlayer;
    [Export] private RichTextLabel _monsterName;
    [Export] private AnimatedSprite2D _animation;
    [Export] public HBoxContainer BuffContainer;
    [Export] private TextureRect _selectRect;
    [Export] private Control _mainControl;
    [Export] public RichTextLabel IntentRichLabel;

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
            }
            else
            {
                StaticInstance.MainRoot.ShowRichHint(Data.GetRichDesc() + Data.Script.Intent);
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

    private void Init(string id)
    {
        Data = new EnemyRole(id);
        _monsterName.Text = StaticUtils.MakeBBCodeString(Data.Name);
        _hpProgress.MaxValue = Data.CurrHpLimit;
        _mpProgress.MaxValue = Data.CurrMpLimit;
        _pbProgress.MaxValue = _mbProgress.MaxValue = Data.CurrHpLimit;
        Data.RoleObject = this;
        var res = ResourceLoader.Load<SpriteFrames>(Data.Script.AnimationResourcePath);
        if (res != null)
        {
            _animation.SpriteFrames = res;
        }

        UpdateObject();
    }

    public void Init(EnemyRole enemyRole)
    {
        Data = enemyRole;
        _monsterName.Text = StaticUtils.MakeBBCodeString(Data.Name);
        _hpProgress.MaxValue = Data.CurrHpLimit;
        _mpProgress.MaxValue = Data.CurrMpLimit;
        _pbProgress.MaxValue = _mbProgress.MaxValue = Data.CurrHpLimit;
        enemyRole.RoleObject = this;
        var res = ResourceLoader.Load<SpriteFrames>(Data.Script.AnimationResourcePath);
        if (res != null)
        {
            _animation.SpriteFrames = res;
        }

        enemyRole.Buffs.ForEach(AddBuff);
        UpdateObject();
    }

    private void UpdateObject(EnemyRole role = null)
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
                var type = (TargetType)datas[0];
                if (type is TargetType.AllSingle or TargetType.EnemySingle)
                    _selectingTarget = true;
                break;
            }
            case "EndSelectTarget":
                _selectingTarget = false;
                _selectRect.Visible = false;
                break;
            case "SelectTargetAll":
            {
                var type = (TargetType)datas[0];
                if (type is TargetType.EnemyAll or TargetType.All)
                {
                    _selectingTarget = false;
                    _selectRect.Visible = true;
                }

                break;
            }
        }

        Data?.ReceiveEvent(@event, datas);
        foreach (var buffObject in BuffContainer?.GetChildren().Cast<BuffObject>() ?? [])
        {
            buffObject.ReceiveEvent(@event, datas);
        }
    }

    public void AddBuff(string id, object creator)
    {
        var buffObj = (BuffObject)ResourceLoader.Load<PackedScene>("res://Pages/BuffObject.tscn").Instantiate();
        buffObj.Init(id, creator);
        if (buffObj.Data == null) return;
        buffObj.Data.Binder = Data;
        buffObj.Data.BuffObj = buffObj;
        buffObj.Data.OnBuffAdded?.Invoke(buffObj.Data);
        BuffContainer?.AddChild(buffObj);
    }

    public void AddBuff(BuffImplBase buff)
    {
        var buffObj = (BuffObject)ResourceLoader.Load<PackedScene>("res://Pages/BuffObject.tscn").Instantiate();
        buffObj.Init(buff);
        if (buffObj.Data == null) return;
        buffObj.Data.Binder = Data;
        buffObj.Data.BuffObj = buffObj;
        buffObj.Data.OnBuffAdded?.Invoke(buffObj.Data);
        BuffContainer?.AddChild(buffObj);
    }

    ~EnemyRoleObject()
    {
        StaticInstance.EventMgr.UnregisterIEvent(this);
    }
}