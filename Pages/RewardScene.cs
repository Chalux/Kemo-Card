using Godot;
using KemoCard.Pages;
using StaticClass;
using System.Collections.Generic;

public partial class RewardScene : BaseScene
{
    [Export] VBoxContainer vBox;
    [Export] Godot.Button ReturnBtn;
    public override void OnAdd(params object[] datas)
    {
        if (StaticInstance.windowMgr.GetSceneByName("MainScene") is MainScene ms)
        {
            ms.MapView.HideMap();
        }
        List<RewardStruct> d = (List<RewardStruct>)datas[0];
        d.ForEach(data =>
        {
            RewardItem item = (RewardItem)ResourceLoader.Load<PackedScene>("res://Pages/RewardItem.tscn").Instantiate();
            item.SetReward(data.type, data.rewards);
            vBox.AddChild(item);
        });
        ReturnBtn.Pressed += new(() =>
        {
            StaticInstance.windowMgr.RemoveScene(this);
            if (StaticInstance.windowMgr.GetSceneByName("MainScene") is MainScene ms)
            {
                ms.MapView.ShowMap();
                //ms.MapView.UnlockFloor(StaticInstance.playerData.gsd.MapGenerator.FloorsClimbed);
                ms.MapView.UnlockNextRooms();
            }
            StaticUtils.AutoSave();
        });
    }
}
