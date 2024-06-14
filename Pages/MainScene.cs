using Godot;
using KemoCard.Pages;
using StaticClass;

public partial class MainScene : BaseScene
{
    [Export] Godot.Button SaveButton;
    [Export] Godot.Button EditDeckButton;
    [Export] Godot.Button TestBattleButton;
    [Export] Godot.Button EditEquipBtn;

    public override void OnAdd(dynamic datas = null)
    {
        SaveButton.Pressed += new(() =>
        {
            var img = GetViewport().GetTexture().GetImage();
            img.Resize(256, 135);
            StaticInstance.playerData.screen_snapshot = img;
            StaticInstance.windowMgr.AddScene((BaseScene)ResourceLoader.Load<PackedScene>("res://Pages/SaveScene.tscn").Instantiate());
        });
        EditDeckButton.Pressed += new(() =>
        {
            StaticInstance.windowMgr.AddScene((BaseScene)ResourceLoader.Load<PackedScene>("res://Pages/DeckEditScene.tscn").Instantiate());
            StaticInstance.MainRoot.HideRichHint();
        });
        TestBattleButton.Pressed += new(() =>
        {
            BattleScene node = (BattleScene)ResourceLoader.Load<PackedScene>("res://Pages/BattleScene.tscn").Instantiate();
            StaticInstance.windowMgr.ChangeScene(node, new((scene) =>
            {
                node.NewBattle(StaticInstance.playerData.gsd.MajorRole, new uint[] { 1, 1 });
            }));
        });
        EditEquipBtn.Pressed += new(() =>
        {
            StaticInstance.windowMgr.AddScene((BaseScene)ResourceLoader.Load<PackedScene>("res://Pages/BagScene.tscn").Instantiate(), new[] { StaticInstance.playerData.gsd.MajorRole });
            StaticInstance.MainRoot.HideRichHint();
        });
    }
}
