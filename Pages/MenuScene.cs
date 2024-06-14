using DialogueManagerRuntime;
using Godot;
using KemoCard.Pages;
using StaticClass;

public partial class MenuScene : BaseScene
{
    [Export] Godot.Button exit;
    [Export] Godot.Button start;
    [Export] Godot.Button load;
    public override void _Ready()
    {
        base._Ready();
        exit.Pressed += new(() =>
        {
            GD.Print("MenuScene退出了游戏");
            GetTree().Quit();
        });
        exit.MouseEntered += new(() =>
        {
            StaticInstance.MainRoot.ShowRichHint("退出游戏");
        });
        exit.MouseExited += new(() =>
        {
            StaticInstance.MainRoot.HideRichHint();
        });
        start.MouseEntered += new(() =>
        {
            StaticInstance.MainRoot.ShowRichHint("开始游戏");
        });
        start.MouseExited += new(() =>
        {
            StaticInstance.MainRoot.HideRichHint();
        });
        start.Pressed += new(() =>
        {
            StaticInstance.windowMgr.ChangeScene(ResourceLoader.Load<PackedScene>("res://Pages/RoleCreateScene.tscn").Instantiate(), new((scene) => { StaticInstance.MainRoot.canPause = true; }));
            StaticInstance.MainRoot.HideRichHint();
        });
        load.Pressed += new(() =>
        {
            StaticInstance.windowMgr.ChangeScene(ResourceLoader.Load<PackedScene>("res://Pages/LoadSaveScene.tscn").Instantiate(), new((scene) => { StaticInstance.MainRoot.canPause = true; }));
            StaticInstance.MainRoot.HideRichHint();
        });
    }
}
