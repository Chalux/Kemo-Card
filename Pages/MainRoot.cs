using System;
using Godot;
using KemoCard.Scripts;

namespace KemoCard.Pages;

public partial class MainRoot : Control
{
    [Signal]
    public delegate void OnInitMainRootEventHandler();

    [Export] private int _vPadding = 20;
    [Export] private int _hPadding = 20;
    [Export] private ColorRect _colorRect;
    [Export] private RichTextLabel _banner;
    [Export] private RichTextLabel _richHint;
    [Export] public ColorRect BlackMask;
    private Action _richHintAction;

    public bool CanPause = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetTree().Root.Size = new Vector2I(1280, 720);
        GetTree().Root.Position = (DisplayServer.ScreenGetSize() - GetTree().Root.Size) / 2;

        #region 静态全局初始化开始

        StaticInstance.MainRoot = this;
        StaticInstance.EventMgr = new EventMgr();
        StaticInstance.WindowMgr = new WindowMgr();

        Datas.Ins.Init();

        StaticInstance.PlayerData = new PlayerData();

        #endregion

        //StaticInstance.windowMgr.ChangeScene(ResourceLoader.Load<PackedScene>("res://Pages/LoadScene.tscn").Instantiate());
        StaticInstance.WindowMgr.ChangeScene(ResourceLoader.Load<PackedScene>("res://Pages/menu_scene.tscn")
            .Instantiate());
        //richHint = colorRect.GetNode<RichTextLabel>("richHint");
        _richHint.Position = new Vector2(_hPadding, _vPadding);

        HideRichHint();
    }

    public override void _Process(double delta)
    {
        if (!_colorRect.Visible) return;
        var positionX = GetLocalMousePosition().X + 50;
        var positionY = GetLocalMousePosition().Y;
        _colorRect.Position =
            new Vector2((positionX + _colorRect.Size.X) > 1920 ? positionX - 100 - _colorRect.Size.X : positionX,
                (positionY + _colorRect.Size.Y) > 1080 ? positionY - _colorRect.Size.Y : positionY);
    }

    public void ShowRichHint(string hint)
    {
        var timer = GetNode<Timer>("LoadTimer");
        _richHintAction?.Invoke();
        _richHintAction = () =>
        {
            _richHint.Size = hint.Length > 100 ? new Vector2(900, 0) : new Vector2(500, 0);
            _richHint.Text = hint;
            _colorRect.Visible = true;
        };
        timer.Timeout += _richHintAction;
        timer.WaitTime = 0.1f;
        timer.OneShot = true;
        timer.Start();
    }

    public void HideRichHint()
    {
        var timer = GetNode<Timer>("LoadTimer");
        _richHintAction?.Invoke();
        timer.Stop();
        _richHint.Text = "";
        _colorRect.Visible = false;
    }

    public void ShowBanner(string tip, bool overrideStyle = false)
    {
        if (overrideStyle)
        {
            _banner.Text = tip;
        }
        else
        {
            _banner.Text = "[font_size=36][center]" + tip + "[/center][/font_size]";
        }

        var color = _banner.Modulate;
        // _banner.Modulate = new Color(color + "ff");
        color.A = 1;
        _banner.Modulate = color;
        GetTree().CreateTimer(3.0f).Timeout += TweenBanner;
    }

    private void TweenBanner()
    {
        var color = _banner.Modulate;
        var tween = GetTree().CreateTween();
        color.A = 0;
        tween.TweenProperty(_banner, "modulate", color, 1.5f);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventKey eventKey) return;
        if (!eventKey.Pressed || eventKey.Keycode != Key.Escape) return;
        if (StaticInstance.WindowMgr.PopupCount() > 0)
        {
            StaticInstance.WindowMgr.RemoveTopestPopup();
        }
        else
        {
            if (CanPause)
                StaticInstance.WindowMgr.PopScene(ResourceLoader.Load<PackedScene>("res://Pages/PauseView.tscn")
                    .Instantiate());
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_colorRect.Visible)
        {
            _colorRect.SetSize(new Vector2(_richHint.Size.X + 2 * _hPadding, _richHint.Size.Y + 2 * _vPadding));
        }
    }
}