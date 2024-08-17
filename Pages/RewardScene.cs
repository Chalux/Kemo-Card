using Godot;
using KemoCard.Pages;
using StaticClass;
using System.Collections.Generic;

public partial class RewardScene : BaseScene
{
    [Export] VBoxContainer vBox;
    [Export] Godot.Button ReturnBtn;
    public override void OnAdd(dynamic datas = null)
    {
        List<RewardStruct> d = datas;
        d.ForEach(data =>
        {
            RewardItem item = (RewardItem)ResourceLoader.Load<PackedScene>("res://Pages/RewardItem.tscn").Instantiate();
            item.SetReward(data.type, data.rewards);
            vBox.AddChild(item);
        });
        ReturnBtn.Pressed += new(() =>
        {
            StaticInstance.windowMgr.RemoveScene(this);
        });
    }
}
