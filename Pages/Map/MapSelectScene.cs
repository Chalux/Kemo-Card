using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Map;

namespace KemoCard.Pages.Map;

public partial class MapSelectScene : BaseScene
{
    [Export] private Godot.Button _exitButton;
    [Export] private FlowContainer _flowContainer;

    public override void _Ready()
    {
        _exitButton.Pressed += OnExitButtonOnPressed;
        foreach (var map in Datas.Ins.MapPool)
        {
            MapData m = new(map.Key);
            if (!CondMgr.Ins.CheckCond(m.Cond)) continue;
            var btn = (MapBtn)ResourceLoader.Load<PackedScene>("res://Pages/Map/MapBtn.tscn").Instantiate();
            btn.MapData = m;
            _flowContainer.AddChild(btn);
        }
    }

    private void OnExitButtonOnPressed()
    {
        StaticInstance.WindowMgr.RemoveScene(this);
    }
}