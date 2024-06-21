using Godot;
using KemoCard.Pages;
using KemoCard.Scripts;
using StaticClass;
using System.Collections.Generic;
using System.Linq;

public partial class MainScene : BaseScene
{
    [Export] Godot.Button SaveButton;
    [Export] Godot.Button EditDeckButton;
    [Export] Godot.Button TestBattleButton;
    [Export] Godot.Button EditEquipBtn;
    [Export] Godot.Button TestPresetBtn;
    [Export] TextEdit EnemiesInput;
    [Export] TextEdit PresetInput;
    [Export] Node2D DebugNode;

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
            StaticInstance.windowMgr.AddScene((BaseScene)ResourceLoader.Load<PackedScene>("res://Pages/DeckView.tscn").Instantiate()
                , new[] { StaticInstance.playerData.gsd.MajorRole.Deck.ToList() });
            StaticInstance.MainRoot.HideRichHint();
        });
        TestBattleButton.Pressed += new(() =>
        {
            string[] strings = EnemiesInput.Text.Split(",");
            List<uint> array = new();
            try
            {
                foreach (var s in strings)
                {
                    if (s != "") array.Add((uint)s.ToInt());
                }
            }
            catch
            {
                StaticInstance.MainRoot.ShowBanner("输入的格式不对");
                return;
            }
            if (array.Count == 0)
            {
                StaticInstance.MainRoot.ShowBanner("未取得任何数据");
                return;
            }
            BattleScene node = (BattleScene)ResourceLoader.Load<PackedScene>("res://Pages/BattleScene.tscn").Instantiate();
            StaticInstance.windowMgr.ChangeScene(node, new((scene) =>
            {
                node.NewBattle(StaticInstance.playerData.gsd.MajorRole, array.ToArray());
            }));
        });
        TestPresetBtn.Pressed += new(() =>
        {
            uint preset = (uint)PresetInput.Text.ToInt();
            if (Datas.Ins.PresetPool.ContainsKey(preset))
            {
                BattleScene node = (BattleScene)ResourceLoader.Load<PackedScene>("res://Pages/BattleScene.tscn").Instantiate();
                StaticInstance.windowMgr.ChangeScene(node, new((scene) =>
                {
                    node.NewBattleByPreset(preset);
                }));
            }
            else
            {
                StaticInstance.MainRoot.ShowBanner("无此预设");
            }
        });
        EditEquipBtn.Pressed += new(() =>
        {
            StaticInstance.windowMgr.AddScene((BaseScene)ResourceLoader.Load<PackedScene>("res://Pages/BagScene.tscn").Instantiate(), new[] { StaticInstance.playerData.gsd.MajorRole });
            StaticInstance.MainRoot.HideRichHint();
        });
        DebugNode.Visible = OS.IsDebugBuild();
    }
}
