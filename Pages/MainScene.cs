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
    [Export] Godot.Button ShowMapBtn;
    [Export] Godot.Button AllocBtn;
    [Export] Label AllocLabel;
    [Export] Label LevelLabel;
    [Export] Label ExpLabel;
    [Export] Label GoldLabel;
    [Export] ProgressBar ExpProg;
    [Export] TextEdit EnemiesInput;
    [Export] TextEdit PresetInput;
    [Export] Control DebugNode;
    [Export] Map MapView;

    public override void OnAdd(dynamic datas = null)
    {
        var major = StaticInstance.playerData.gsd.MajorRole;
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
                , new[] { major.Deck.ToList() });
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
                node.NewBattle(major, array.ToArray());
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
            StaticInstance.windowMgr.AddScene((BaseScene)ResourceLoader.Load<PackedScene>("res://Pages/BagScene.tscn").Instantiate(), new[] { major });
            StaticInstance.MainRoot.HideRichHint();
        });
        DebugNode.Visible = OS.IsDebugBuild();
        ShowMapBtn.Pressed += new(() =>
        {
            MapView.ShowMap();
        });
        MapView.CreateMap();
        MapView.HideMap();
        AllocBtn.Pressed += new(() =>
        {
            StaticInstance.windowMgr.AddScene((BaseScene)ResourceLoader.Load<PackedScene>("res://Pages/AllocPointScene.tscn").Instantiate(), new[] { major });
            StaticInstance.MainRoot.HideRichHint();
        });
        var maxExp = ExpCfg.CalUpgradeNeedExp(major.Level);
        AllocLabel.Text = $"剩余点数：{major.UnUsedPoints}";
        GoldLabel.Text = $"{major.Gold}";
        ExpLabel.Text = $"{major.Exp}/{maxExp}";
        ExpProg.MinValue = 0;
        ExpProg.MaxValue = maxExp;
        ExpProg.Value = major.Exp;
        LevelLabel.Text = $"{major.Level}级";
    }
}
