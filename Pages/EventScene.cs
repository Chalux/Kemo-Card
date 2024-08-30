using Godot;
using KemoCard.Pages;
using KemoCard.Scripts;
using KemoCard.Scripts.Events;
using StaticClass;
using System;

public partial class EventScene : BaseScene, IEvent
{
    [Export] VBoxContainer BtnContainer;
    [Export] TextureRect EventImg;
    [Export] Label EventTitle;
    private Event eventData;
    public override void OnAdd(dynamic datas = null)
    {
        eventData = datas as Event;
        int maxLen = Math.Max(eventData.EventActions.Count, Math.Max(eventData.EventDesc.Count, eventData.EventIconPath.Count));
        foreach (var node in BtnContainer.GetChildren())
        {
            node.QueueFree();
        }
        for (int i = 0; i < maxLen; i++)
        {
            Godot.Button btn = new();
            btn.AddThemeFontSizeOverride("EventSceneFontSize", 36);
            if (eventData.EventDesc[i] != null) btn.Text = eventData.EventDesc[i];
            else btn.Text = "无描述";
            CompressedTexture2D compressedTexture2D = ResourceLoader.Load<CompressedTexture2D>(eventData.EventIconPath[i]);
            if (compressedTexture2D != null) btn.Icon = compressedTexture2D;
            else btn.Icon = null;
            if (eventData.EventActions[i] != null) btn.Pressed += eventData.EventActions[i];
            BtnContainer.AddChild(btn);
        }
        if (eventData.EventImgPath?.Length > 0) EventImg.Texture = ResourceLoader.Load<CompressedTexture2D>(eventData.EventImgPath);
        EventTitle.Text = eventData.EventTitle;
    }

    public void ReceiveEvent(string @event, params object[] datas)
    {
        if(@event == "close_eventscene")
        {
            StaticInstance.windowMgr.RemoveScene(this);
        }
    }
}
