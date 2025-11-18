using System.Linq;
using Godot;
using KemoCard.Scripts;

namespace KemoCard.Pages;

public partial class ShopScene : BaseScene, IEvent
{
    [Export] private FlowContainer _shopContainer;
    [Export] private Button _leaveBtn;
    [Export] private Label _goldLabel;

    public override void OnAdd(params object[] datas)
    {
        foreach (var @struct in StaticInstance.PlayerData.Gsd.CurrShopStructs)
        {
            var res = ResourceLoader.Load<PackedScene>("res://Pages/ShopCardItem.tscn");
            if (res == null) continue;
            var obj = res.Instantiate<ShopCardItem>();
            obj.Init(@struct);
            _shopContainer.AddChild(obj);
            obj.Update();
        }

        _leaveBtn.Pressed += Leave;
        _goldLabel.Text = StaticInstance.PlayerData.Gsd.MajorRole.Gold.ToString();
    }

    public void ReceiveEvent(string @event, params object[] datas)
    {
        if (@event != "GoldChanged") return;
        _goldLabel.Text = StaticInstance.PlayerData.Gsd.MajorRole.Gold.ToString();
        foreach (var sci in _shopContainer.GetChildren().Cast<ShopCardItem>())
        {
            sci.Update();
        }
    }

    private void Leave()
    {
        foreach (var node in _shopContainer.GetChildren())
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