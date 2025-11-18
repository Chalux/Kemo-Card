using System.Collections.Generic;
using System.Linq;
using Godot;
using KemoCard.Pages.Map;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;

namespace KemoCard.Pages;

public partial class MainScene : BaseScene, IEvent
{
    [Export] private Button _saveButton;
    [Export] private Button _editDeckButton;
    [Export] private Button _testBattleButton;
    [Export] private Button _editEquipBtn;
    [Export] private Button _testPresetBtn;
    [Export] private Button _showMapBtn;
    [Export] private Button _allocBtn;
    [Export] private Label _allocLabel;
    [Export] private Label _levelLabel;
    [Export] private Label _expLabel;
    [Export] private Label _goldLabel;
    [Export] private ProgressBar _expProg;
    [Export] private TextEdit _enemiesInput;
    [Export] private TextEdit _presetInput;
    [Export] private Control _debugNode;
    [Export] public MapView MapView;
    [Export] private Label _hpLabel;
    [Export] private ProgressBar _hpProg;
    [Export] private Label _mpLabel;
    [Export] private ProgressBar _mpProg;
    [Export] private Button _testAddCardBtn;
    [Export] private TextEdit _addCardInput;
    [Export] private Button _returnMenuBtn;
    [Export] private Button _checkMajorBtn;

    public override void _Ready()
    {
        var major = StaticInstance.PlayerData.Gsd.MajorRole;
        _saveButton.Pressed += OnSaveButtonOnPressed;
        _editDeckButton.Pressed += () =>
        {
            StaticInstance.WindowMgr.AddScene(
                (BaseScene)ResourceLoader.Load<PackedScene>("res://Pages/RoleDeckView.tscn").Instantiate(), major);
            StaticInstance.MainRoot.HideRichHint();
        };
        _testBattleButton.Pressed += () =>
        {
            var strings = _enemiesInput.Text.Split(",");
            List<string> array = [];
            try
            {
                array.AddRange(strings.Where(string.IsNullOrEmpty));
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

            var node = (BattleScene)ResourceLoader.Load<PackedScene>("res://Pages/BattleScene.tscn")
                .Instantiate();
            StaticInstance.WindowMgr.AddScene(node);
            node.NewBattle(major, array.ToArray(), true);
        };
        _testPresetBtn.Pressed += () =>
        {
            var preset = _presetInput.Text;
            if (Datas.Ins.PresetPool.ContainsKey(preset))
            {
                var node = (BattleScene)ResourceLoader.Load<PackedScene>("res://Pages/BattleScene.tscn")
                    .Instantiate();
                StaticInstance.WindowMgr.AddScene(node);
                node.NewBattleByPreset(preset, true);
            }
            else
            {
                StaticInstance.MainRoot.ShowBanner("无此预设");
            }
        };
        _editEquipBtn.Pressed += () =>
        {
            StaticInstance.WindowMgr.AddScene(
                (BaseScene)ResourceLoader.Load<PackedScene>("res://Pages/BagScene.tscn").Instantiate(), major);
            StaticInstance.MainRoot.HideRichHint();
        };
        _debugNode.Visible = OS.IsDebugBuild();
        _showMapBtn.Pressed += () =>
        {
            if (StaticInstance.PlayerData.Gsd.MapGenerator.IsStillRunning)
            {
                MapView?.ShowMap();
            }
            else
            {
                StaticInstance.WindowMgr.AddScene((BaseScene)ResourceLoader
                    .Load<PackedScene>("res://Pages/Map/MapSelectScene.tscn").Instantiate());
                StaticInstance.MainRoot.HideRichHint();
            }
        };
        _allocBtn.Pressed += () =>
        {
            StaticInstance.WindowMgr.AddScene(
                (BaseScene)ResourceLoader.Load<PackedScene>("res://Pages/AllocPointScene.tscn").Instantiate(), major);
            StaticInstance.MainRoot.HideRichHint();
        };
        _testAddCardBtn.Pressed += () =>
        {
            Card c = new(_addCardInput.Text);
            if (!string.IsNullOrEmpty(c.Id))
            {
                StaticInstance.PlayerData.Gsd.MajorRole.AddCardToTempDeck(c);
                StaticInstance.MainRoot.ShowBanner($"已添加卡牌{c.Id}至牌库");
            }
            else
            {
                StaticInstance.MainRoot.ShowBanner($"卡牌{c.Id}不存在或者错误");
            }
        };
        _returnMenuBtn.Pressed += () =>
        {
            AlertView.PopupAlert("确定要返回主菜单吗？未保存的进度将丢失。", false, () =>
            {
                BattleStatic.Reset();
                StaticInstance.PlayerData.Gsd = new GlobalSaveData();
                StaticInstance.WindowMgr.RemoveAllScene();
                StaticInstance.WindowMgr.ChangeScene(ResourceLoader.Load<PackedScene>("res://Pages/menu_scene.tscn")
                    .Instantiate());
            });
        };
        _checkMajorBtn.MouseEntered += () =>
        {
            StaticInstance.MainRoot.ShowRichHint(StaticInstance.PlayerData.Gsd.MajorRole.GetRichDesc());
        };
        _checkMajorBtn.MouseExited += StaticInstance.MainRoot.HideRichHint;
        MapView?.CreateMap();
        UpdateView();
    }

    private void OnSaveButtonOnPressed()
    {
        var img = GetViewport().GetTexture().GetImage();
        img.Resize(256, 135);
        StaticInstance.PlayerData.ScreenSnapshot = img;
        StaticInstance.WindowMgr.AddScene((BaseScene)ResourceLoader.Load<PackedScene>("res://Pages/SaveScene.tscn")
            .Instantiate());
    }

    public void UpdateView()
    {
        var major = StaticInstance.PlayerData.Gsd.MajorRole;
        var maxExp = ExpCfg.CalUpgradeNeedExp(major.Level);
        _allocLabel.Text = $"剩余点数：{major.UnUsedPoints}";
        _goldLabel.Text = $"{major.Gold}";
        _expLabel.Text = $"{major.Exp}/{maxExp}";
        _expProg.MinValue = 0;
        _expProg.MaxValue = maxExp;
        _expProg.Value = major.Exp;
        _levelLabel.Text = $"{major.Level}级";
        _hpLabel.Text = $"{major.CurrHealth}/{major.CurrHpLimit}";
        _hpProg.MinValue = 0;
        _hpProg.MaxValue = major.CurrHpLimit;
        _hpProg.Value = major.CurrHealth;
        _mpLabel.Text = $"{major.CurrMagic}/{major.CurrMpLimit}";
        _mpProg.MinValue = 0;
        _mpProg.MaxValue = major.CurrMpLimit;
        _mpProg.Value = major.CurrMagic;
        //SelectMapBtn.Disabled = StaticInstance.playerData.gsd.MapGenerator.IsStillRunning;
        _showMapBtn.Text = StaticInstance.PlayerData.Gsd.MapGenerator.IsStillRunning ? "查看地图" : "选择地图";

        if (StaticInstance.PlayerData.Gsd.MapGenerator.FloorsClimbed == 0)
            MapView?.UnlockFloor(StaticInstance.PlayerData.Gsd.MapGenerator.FloorsClimbed);
        else MapView?.UnlockNextRooms();
    }

    public void ReceiveEvent(string @event, params object[] datas)
    {
        if (@event is "PropertiesChanged" or "MapStateChange")
        {
            UpdateView();
        }
    }
}