using System;
using Godot;
using KemoCard.Scripts;

namespace KemoCard.Pages;

public partial class MenuScene : BaseScene
{
    [Export] Godot.Button exit;
    [Export] Godot.Button start;
    [Export] Godot.Button load;
    [Export] Godot.Button DebugTempBtn;

    public override void _Ready()
    {
        base._Ready();
        DebugTempBtn.Visible = OS.IsDebugBuild();
        exit.Pressed += () =>
        {
            GD.Print("MenuScene退出了游戏");
            GetTree().Quit();
        };
        exit.MouseEntered += () => { StaticInstance.MainRoot.ShowRichHint("退出游戏"); };
        exit.MouseExited += () => { StaticInstance.MainRoot.HideRichHint(); };
        start.MouseEntered += () => { StaticInstance.MainRoot.ShowRichHint("开始游戏"); };
        start.MouseExited += () => { StaticInstance.MainRoot.HideRichHint(); };
        start.Pressed += () =>
        {
            StaticInstance.WindowMgr.ChangeScene(
                ResourceLoader.Load<PackedScene>("res://Pages/RoleCreateScene.tscn").Instantiate(),
                _ => { StaticInstance.MainRoot.CanPause = true; });
            StaticInstance.MainRoot.HideRichHint();
        };
        load.Pressed += () =>
        {
            StaticInstance.WindowMgr.ChangeScene(
                ResourceLoader.Load<PackedScene>("res://Pages/LoadSaveScene.tscn").Instantiate(),
                _ => { StaticInstance.MainRoot.CanPause = true; });
            StaticInstance.MainRoot.HideRichHint();
        };
        if (DebugTempBtn.Visible)
        {
            DebugTempBtn.Pressed += () =>
            {
                var res = ResourceLoader.Load<PackedScene>("res://Pages/FloatingNum.tscn");
                if (res == null) return;
                var floatingNum = res.Instantiate<FloatingNum>();
                Random r = new();
                floatingNum.Init($"+{r.Next(0, 1000000)}", Colors.Khaki, new(500, 500));
                AddChild(floatingNum);
            };
        }
    }
}