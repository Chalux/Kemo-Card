using Godot;
using KemoCard.Pages;
using KemoCard.Scripts;
using KemoCard.Scripts.Map;
using StaticClass;

public partial class MapSelectScene : BaseScene
{
    [Export] Godot.Button ExitButton;
    [Export] FlowContainer FlowContainer;
    public override void _Ready()
    {
        ExitButton.Pressed += new(() =>
        {
            StaticInstance.windowMgr.RemoveScene(this);
        });
        foreach (var map in Datas.Ins.MapPool)
        {
            MapData m = new(map.Key);
            if (CondMgr.Ins.CheckCond(m.Cond))
            {
                MapBtn btn = (MapBtn)ResourceLoader.Load<PackedScene>("res://Pages/Map/MapBtn.tscn").Instantiate();
                btn.MapData = m;
                FlowContainer.AddChild(btn);
            }
        }
    }
}
