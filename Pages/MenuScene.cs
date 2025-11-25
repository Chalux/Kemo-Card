using System;
using Godot;
using KemoCard.Scripts;

namespace KemoCard.Pages;

public partial class MenuScene : BaseScene
{
    [Export] private Button _exit;
    [Export] private Button _start;
    [Export] private Button _load;
    [Export] private Button _debugTempBtn;
    [Export] private Button _setting;

    public override void _Ready()
    {
        base._Ready();
        _debugTempBtn.Visible = OS.IsDebugBuild();
        _exit.Pressed += () =>
        {
            GD.Print("MenuScene退出了游戏");
            StaticInstance.SettingData.Save();
            GetTree().Quit();
        };
        _exit.MouseEntered += () => { StaticInstance.MainRoot.ShowRichHint("退出游戏"); };
        _exit.MouseExited += () => { StaticInstance.MainRoot.HideRichHint(); };
        _start.MouseEntered += () => { StaticInstance.MainRoot.ShowRichHint("开始游戏"); };
        _start.MouseExited += () => { StaticInstance.MainRoot.HideRichHint(); };
        _start.Pressed += () =>
        {
            StaticInstance.WindowMgr.ChangeScene(
                ResourceLoader.Load<PackedScene>("res://Pages/RoleCreateScene.tscn").Instantiate(),
                _ => { StaticInstance.MainRoot.CanPause = true; });
            StaticInstance.MainRoot.HideRichHint();
        };
        _load.Pressed += () =>
        {
            StaticInstance.WindowMgr.ChangeScene(
                ResourceLoader.Load<PackedScene>("res://Pages/LoadSaveScene.tscn").Instantiate(),
                _ => { StaticInstance.MainRoot.CanPause = true; });
            StaticInstance.MainRoot.HideRichHint();
        };
        if (_debugTempBtn.Visible)
        {
            _debugTempBtn.Pressed += () =>
            {
                var res = ResourceLoader.Load<PackedScene>("res://Pages/FloatingNum.tscn");
                if (res == null) return;
                var floatingNum = res.Instantiate<FloatingNum>();
                Random r = new();
                floatingNum.Init($"+{r.Next(0, 1000000)}", Colors.Khaki, new Vector2(500, 500));
                AddChild(floatingNum);
            };
        }

        _setting.Pressed += () =>
        {
            // StaticInstance.WindowMgr.ChangeScene(
            //     ResourceLoader.Load<PackedScene>("res://Pages/SettingScene.tscn").Instantiate(),
            //     _ => { StaticInstance.MainRoot.CanPause = true; });
            var res = ResourceLoader.Load<PackedScene>("res://Pages/SettingScene.tscn");
            if (res == null) return;
            StaticInstance.WindowMgr.AddScene(res.Instantiate<SettingScene>());
            StaticInstance.MainRoot.HideRichHint();
        };
    }
}