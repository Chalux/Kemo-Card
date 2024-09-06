using Godot;
using KemoCard.Pages;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;
using System.Collections.Generic;
using System.Linq;

public partial class MainScene : BaseScene, IEvent
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
    [Export] Godot.Button SelectMapBtn;
    [Export] public Map MapView;
    [Export] Label HpLabel;
    [Export] ProgressBar HpProg;
    [Export] Label MpLabel;
    [Export] ProgressBar MpProg;
    [Export] Godot.Button TestAddCardBtn;
    [Export] TextEdit AddCardInput;
    [Export] Godot.Button ReturnMenuBtn;

    public override void _Ready()
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
            //StaticInstance.windowMgr.AddScene((BaseScene)ResourceLoader.Load<PackedScene>("res://Pages/DeckView.tscn").Instantiate()
            //    , new[] { major.Deck.ToList().Concat(major.TempDeck.ToList()).ToList() });
            StaticInstance.windowMgr.AddScene((BaseScene)ResourceLoader.Load<PackedScene>("res://Pages/RoleDeckView.tscn").Instantiate()
                , new[] { major });
            StaticInstance.MainRoot.HideRichHint();
        });
        TestBattleButton.Pressed += new(() =>
        {
            string[] strings = EnemiesInput.Text.Split(",");
            List<string> array = new();
            try
            {
                foreach (var s in strings)
                {
                    if (s != "") array.Add(s);
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
            StaticInstance.windowMgr.AddScene(node);
            node.NewBattle(major, array.ToArray(), true);
        });
        TestPresetBtn.Pressed += new(() =>
        {
            string preset = PresetInput.Text;
            if (Datas.Ins.PresetPool.ContainsKey(preset))
            {
                BattleScene node = (BattleScene)ResourceLoader.Load<PackedScene>("res://Pages/BattleScene.tscn").Instantiate();
                StaticInstance.windowMgr.AddScene(node);
                node.NewBattleByPreset(preset, true);
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
            MapView?.ShowMap();
        });
        AllocBtn.Pressed += new(() =>
        {
            StaticInstance.windowMgr.AddScene((BaseScene)ResourceLoader.Load<PackedScene>("res://Pages/AllocPointScene.tscn").Instantiate(), new[] { major });
            StaticInstance.MainRoot.HideRichHint();
        });
        SelectMapBtn.Pressed += new(() =>
        {
            StaticInstance.windowMgr.AddScene((BaseScene)ResourceLoader.Load<PackedScene>("res://Pages/Map/MapSelectScene.tscn").Instantiate());
            StaticInstance.MainRoot.HideRichHint();
        });
        TestAddCardBtn.Pressed += new(() =>
        {
            Card c = new(AddCardInput.Text);
            if (c != null && c.Id != "")
            {
                StaticInstance.playerData.gsd.MajorRole.AddCardToTempDeck(c);
                StaticInstance.MainRoot.ShowBanner($"已添加卡牌{c.Id}至牌库");
            }
            else
            {
                StaticInstance.MainRoot.ShowBanner($"卡牌{c.Id}不存在或者错误");
            }
        });
        ReturnMenuBtn.Pressed += new(() =>
        {
            AlertView.PopupAlert("确定要返回主菜单吗？未保存的进度将丢失。", false, new(() =>
            {
                BattleStatic.Reset();
                StaticInstance.playerData.gsd = new();
                StaticInstance.windowMgr.RemoveAllScene();
                StaticInstance.windowMgr.ChangeScene(ResourceLoader.Load<PackedScene>("res://Pages/menu_scene.tscn").Instantiate());
            }));
        });
        MapView?.CreateMap();
        UpdateView();
    }

    public void UpdateView()
    {
        var major = StaticInstance.playerData.gsd.MajorRole;
        var maxExp = ExpCfg.CalUpgradeNeedExp(major.Level);
        AllocLabel.Text = $"剩余点数：{major.UnUsedPoints}";
        GoldLabel.Text = $"{major.Gold}";
        ExpLabel.Text = $"{major.Exp}/{maxExp}";
        ExpProg.MinValue = 0;
        ExpProg.MaxValue = maxExp;
        ExpProg.Value = major.Exp;
        LevelLabel.Text = $"{major.Level}级";
        HpLabel.Text = $"{major.CurrHealth}/{major.CurrHpLimit}";
        HpProg.MinValue = 0;
        HpProg.MaxValue = major.CurrHpLimit;
        HpProg.Value = major.CurrHealth;
        MpLabel.Text = $"{major.CurrMagic}/{major.CurrMpLimit}";
        MpProg.MinValue = 0;
        MpProg.MaxValue = major.CurrMpLimit;
        MpProg.Value = major.CurrMagic;
        SelectMapBtn.Disabled = StaticInstance.playerData.gsd.MapGenerator.IsStillRunning;
        if (StaticInstance.playerData.gsd.MapGenerator.FloorsClimbed == 0) MapView?.UnlockFloor(StaticInstance.playerData.gsd.MapGenerator.FloorsClimbed);
        else MapView?.UnlockNextRooms();
    }

    public void ReceiveEvent(string @event, params object[] datas)
    {
        if (@event == "PropertiesChanged")
        {
            UpdateView();
        }
    }
}
