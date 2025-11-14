using System.Linq;
using Godot;
using KemoCard.Scripts;

namespace KemoCard.Pages;

public partial class ShopScene : BaseScene, IEvent
{
    [Export] FlowContainer shopContainer;
    [Export] Godot.Button LeaveBtn;
    [Export] Label GoldLabel;

    public override void OnAdd(params object[] datas)
    {
        foreach (var @struct in StaticInstance.PlayerData.Gsd.CurrShopStructs)
        {
            var res = ResourceLoader.Load<PackedScene>("res://Pages/ShopCardItem.tscn");
            if (res == null) continue;
            var obj = res.Instantiate<ShopCardItem>();
            obj.Init(@struct);
            shopContainer.AddChild(obj);
            obj.Update();
        }

        LeaveBtn.Pressed += Leave;
        GoldLabel.Text = StaticInstance.PlayerData.Gsd.MajorRole.Gold.ToString();
    }

    public void ReceiveEvent(string @event, params object[] datas)
    {
        if (@event != "GoldChanged") return;
        GoldLabel.Text = StaticInstance.PlayerData.Gsd.MajorRole.Gold.ToString();
        foreach (var sci in shopContainer.GetChildren().Cast<ShopCardItem>())
        {
            sci.Update();
        }
    }

    public void Leave()
    {
        foreach (var node in shopContainer.GetChildren())
        {
            node.QueueFree();
        }

        StaticInstance.PlayerData.Gsd.CurrShopStructs = [];
        var ms = (MainScene)StaticInstance.WindowMgr.GetSceneByName("MainScene");
        //ms?.MapView.UnlockFloor(StaticInstance.playerData.gsd.MapGenerator.FloorsClimbed);
        ms?.MapView.UnlockNextRooms();
        StaticInstance.WindowMgr.RemoveScene(this);
        StaticUtils.AutoSave();
    }
}