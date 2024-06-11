using Godot;
using KemoCard.Scripts;
using StaticClass;
using System;

public partial class main_root : Control
{
    [Signal]
    public delegate void OnInitMainRootEventHandler();

    [Export] private int Vpadding = 20;
    [Export] private int Hpadding = 20;
    [Export] private ColorRect colorRect;
    [Export] private RichTextLabel banner;
    [Export] private RichTextLabel richHint;
    private Action richHintAction;
    public Boolean canPause = false;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetTree().Root.Size = new Vector2I(1280, 720);
        GetTree().Root.Position = (DisplayServer.ScreenGetSize() - GetTree().Root.Size) / 2;

        #region 静态全局初始化开始
        StaticInstance.MainRoot = this;
        StaticInstance.eventMgr = new();
        StaticInstance.windowMgr = new();

        //StaticInstance.windowMgr.ChangeScene(ResourceLoader.Load<PackedScene>("res://Pages/LoadScene.tscn").Instantiate());
        StaticInstance.windowMgr.ChangeScene(ResourceLoader.Load<PackedScene>("res://Pages/menu_scene.tscn").Instantiate());

        Datas.Ins.Init();

        StaticInstance.playerData = new();
        #endregion

        //richHint = colorRect.GetNode<RichTextLabel>("richHint");
        richHint.Position = new Vector2(Hpadding, Vpadding);

        HideRichHint();
    }

    public override void _Process(double delta)
    {
        if (colorRect.Visible == true)
        {
            float PositionX = GetLocalMousePosition().X + 50;
            float PositionY = GetLocalMousePosition().Y;
            colorRect.Position = new Vector2((PositionX + richHint.Size.X) > 1920 ? PositionX - 100 - richHint.Size.X : PositionX, (PositionY + richHint.Size.Y) > 1080 ? PositionY - richHint.Size.Y : PositionY);
        }
    }

    public void ShowRichHint(string hint)
    {
        Timer timer = GetNode<Timer>("LoadTimer");
        richHintAction?.Invoke();
        richHintAction = new Action(() =>
        {
            colorRect.Visible = true;
            richHint.Text = hint;
        });
        timer.Timeout += richHintAction;
        timer.WaitTime = 0.1f;
        timer.OneShot = true;
        timer.Start();
    }

    public void HideRichHint()
    {
        Timer timer = GetNode<Timer>("LoadTimer");
        richHintAction?.Invoke();
        timer.Stop();
        richHint.Text = "";
        colorRect.Visible = false;
    }

    public void ShowBanner(string tip, bool overrideStyle = false)
    {
        if (overrideStyle)
        {
            banner.Text = tip;
        }
        else
        {
            banner.Text = "[font_size=36][center]" + tip + "[/center][/font_size]";
        }
        string color = banner.Modulate.ToHtml(false);
        banner.Modulate = new Color(color + "ff");
        GetTree().CreateTimer(3.0f).Timeout += new(() =>
        {
            Tween tween = GetTree().CreateTween();
            tween.TweenProperty(banner, "modulate", new Color(color + "00"), 1.5f);
        });
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey eventKey)
        {
            if (eventKey.Pressed && eventKey.Keycode == Key.Escape)
            {
                if (StaticInstance.windowMgr.PopupCount() > 0)
                {
                    StaticInstance.windowMgr.RemoveTopestPopup();
                }
                else
                {
                    if (canPause) StaticInstance.windowMgr.PopScene(ResourceLoader.Load<PackedScene>("res://Pages/PauseView.tscn").Instantiate());
                }
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (colorRect.Visible)
        {
            colorRect.Size = new Vector2(richHint.Size.X + 2 * Hpadding, richHint.Size.Y + Vpadding);
        }
    }
}
