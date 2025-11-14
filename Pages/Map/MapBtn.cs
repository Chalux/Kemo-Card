using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Map;

public partial class MapBtn : Control
{
    private MapData _mapData;
    [Export] Godot.Button CreateBtn;
    [Export] Label ShowNameLab;
    [Export] Label TierRangeLab;
    [Export] Label MapSizeLab;

    public MapData MapData
    {
        get => _mapData;
        set
        {
            _mapData = value;
            ShowNameLab.Text = value.ShowName;
            TierRangeLab.Text = $"{value.MinTier}~{value.MaxTier}";
            MapSizeLab.Text = $"{value.Floors}x{value.MapWidth}";
        }
    }

    public override void _Ready()
    {
        CreateBtn.Pressed += new(() =>
        {
            if (_mapData != null && !StaticInstance.PlayerData.Gsd.MapGenerator.IsStillRunning)
            {
                //StaticInstance.playerData.gsd.MapGenerator.GenerateMap(_mapData);
                if (StaticInstance.WindowMgr.GetSceneByName("MainScene") is KemoCard.Pages.MainScene mainScene && mainScene != null)
                {
                    mainScene.MapView.GenerateNewMap(MapData);
                    mainScene.MapView.UnlockFloor(StaticInstance.PlayerData.Gsd.MapGenerator.FloorsClimbed);
                }
                var mss = StaticInstance.WindowMgr.GetSceneByName("MapSelectScene");
                if (mss != null)
                {
                    StaticInstance.WindowMgr.RemoveScene(mss);
                }
            }
        });
    }
}
