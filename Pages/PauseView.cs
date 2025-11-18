using Godot;
using KemoCard.Scripts;

namespace KemoCard.Pages;

public partial class PauseView : BaseScene
{
    [Export] private Button _quit;

    [Export] private Button _back;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _quit.Pressed += () =>
        {
            AlertView.PopupAlert("[font_size=50]是否退出游戏？[color=#f70101]未保存的进度将会丢失。[/color][/font_size]", true, () =>
            {
                GD.Print("PauseView退出了游戏");
                GetTree().Quit();
            });
        };
        _back.Pressed += () => StaticInstance.WindowMgr.RemoveScene(this);
    }
}