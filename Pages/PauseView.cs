using Godot;
using KemoCard.Pages;
using StaticClass;

public partial class PauseView : BaseScene
{
    [Export] Godot.Button Quit;
    [Export] Godot.Button Back;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Quit.Pressed += new(() =>
        {
            AlertView.PopupAlert("[font_size=50]是否退出游戏？[color=#f70101]未保存的进度将会丢失。[/color][/font_size]", true, new(() =>
            {
                GD.Print("PauseView退出了游戏");
                GetTree().Quit();
            }));
        });
        Back.Pressed += new(() => StaticInstance.windowMgr.RemoveScene(this));
    }

}
