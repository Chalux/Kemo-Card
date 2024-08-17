using Godot;
using KemoCard.Scripts.Map;
using StaticClass;

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
            MapSizeLab.Text = $"{value.FLOORS}x{value.MAP_WIDTH}";
        }
    }

    public override void _Ready()
    {
        CreateBtn.Pressed += new(() =>
        {
            if (_mapData != null && !StaticInstance.playerData.gsd.MapGenerator.IsStillRunning)
            {
                //StaticInstance.playerData.gsd.MapGenerator.GenerateMap(_mapData);
                if (StaticInstance.windowMgr.GetSceneByName("MainScene") is MainScene mainScene && mainScene != null)
                {
                    mainScene.MapView.GenerateNewMap(MapData);
                    mainScene.MapView.UnlockFloor(StaticInstance.playerData.gsd.MapGenerator.FloorsClimbed);
                }
                if (StaticInstance.currWindow is MapSelectScene mss)
                {
                    StaticInstance.windowMgr.RemoveTopestPopup();
                }
            }
        });
    }
}
