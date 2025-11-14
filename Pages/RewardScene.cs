using System.Collections.Generic;
using Godot;
using KemoCard.Scripts;

namespace KemoCard.Pages;

public partial class RewardScene : BaseScene
{
    [Export] VBoxContainer vBox;
    [Export] Godot.Button ReturnBtn;

    public override void OnAdd(params object[] datas)
    {
        if (StaticInstance.WindowMgr.GetSceneByName("MainScene") is MainScene ms)
        {
            ms.MapView.HideMap();
        }

        var d = (List<RewardStruct>)datas[0];
        d.ForEach(data =>
        {
            var item = (RewardItem)ResourceLoader.Load<PackedScene>("res://Pages/RewardItem.tscn").Instantiate();
            item.SetReward(data.Type, data.Rewards);
            vBox.AddChild(item);
        });
        ReturnBtn.Pressed += () =>
        {
            StaticInstance.WindowMgr.RemoveScene(this);
            if (StaticInstance.WindowMgr.GetSceneByName("MainScene") is MainScene mainScene)
            {
                mainScene.MapView.ShowMap();
                //ms.MapView.UnlockFloor(StaticInstance.playerData.gsd.MapGenerator.FloorsClimbed);
                mainScene.MapView.UnlockNextRooms();
            }

            StaticUtils.AutoSave();
        };
    }
}