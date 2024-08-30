using Godot;
using KemoCard.Pages;
using KemoCard.Scripts;
using StaticClass;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ShopScene : BaseScene, IEvent
{
    [Export] FlowContainer shopContainer;
    [Export] Godot.Button LeaveBtn;
    [Export] Label GoldLabel;
    public override void OnAdd(dynamic datas = null)
    {
        foreach (ShopStruct @struct in StaticInstance.playerData.gsd.CurrShopStructs)
        {
            var obj = new ShopCardItem();
            obj.Init(@struct);
            shopContainer.AddChild(obj);
            obj.Update();
        }
        LeaveBtn.Pressed += Leave;
        GoldLabel.Text = StaticInstance.playerData.gsd.MajorRole.Gold.ToString();
    }

    public void ReceiveEvent(string @event, params object[] datas)
    {
        if (@event == "GoldChanged")
        {
            GoldLabel.Text = StaticInstance.playerData.gsd.MajorRole.Gold.ToString();
            foreach (ShopCardItem sci in shopContainer.GetChildren().Cast<ShopCardItem>())
            {
                sci.Update();
            }
        }
    }

    public void Leave()
    {
        foreach (Node node in shopContainer.GetChildren())
        {
            node.QueueFree();
        }
        StaticInstance.playerData.gsd.CurrShopStructs = new();
        StaticInstance.windowMgr.RemoveScene(this);
        StaticUtils.AutoSave();
    }
}
