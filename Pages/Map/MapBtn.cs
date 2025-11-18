using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Map;

namespace KemoCard.Pages.Map;

public partial class MapBtn : Control
{
    private MapData _mapData;
    [Export] private Button _createBtn;
    [Export] private Label _showNameLab;
    [Export] private Label _tierRangeLab;
    [Export] private Label _mapSizeLab;

    public MapData MapData
    {
        get => _mapData;
        set
        {
            _mapData = value;
            _showNameLab.Text = value.ShowName;
            _tierRangeLab.Text = $"{value.MinTier}~{value.MaxTier}";
            _mapSizeLab.Text = $"{value.Floors}x{value.MapWidth}";
        }
    }

    public override void _Ready()
    {
        _createBtn.Pressed += () =>
        {
            if (_mapData == null || StaticInstance.PlayerData.Gsd.MapGenerator.IsStillRunning) return;
            //StaticInstance.playerData.gsd.MapGenerator.GenerateMap(_mapData);
            if (StaticInstance.WindowMgr.GetSceneByName("MainScene") is MainScene mainScene)
            {
                mainScene.MapView.GenerateNewMap(MapData);
                mainScene.MapView.UnlockFloor(StaticInstance.PlayerData.Gsd.MapGenerator.FloorsClimbed);
            }

            var mss = StaticInstance.WindowMgr.GetSceneByName("MapSelectScene");
            if (mss != null)
            {
                StaticInstance.WindowMgr.RemoveScene(mss);
            }
        };
    }
}