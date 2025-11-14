using System;
using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Events;

namespace KemoCard.Pages;

public partial class EventScene : BaseScene, IEvent
{
    [Export] VBoxContainer BtnContainer;
    [Export] TextureRect EventImg;
    [Export] Label EventTitle;
    private EventScript _eventScriptData;

    public override void OnAdd(params object[] datas)
    {
        _eventScriptData = datas[0] as EventScript;
        UpdateView();
    }

    public override void OnRemove()
    {
        StaticUtils.AutoSave();
    }

    public void ReceiveEvent(string @event, params object[] datas)
    {
        if (@event == "close_eventscene")
        {
            StaticInstance.WindowMgr.RemoveSceneByName("EventScene");
        }
    }

    private void UpdateView()
    {
        var maxLen = Math.Max(_eventScriptData.EventActions.Count,
            Math.Max(_eventScriptData.EventDesc.Count, _eventScriptData.EventIconPath.Count));
        foreach (var node in BtnContainer.GetChildren())
        {
            node.QueueFree();
        }

        for (var i = 0; i < maxLen; i++)
        {
            Godot.Button btn = new();
            btn.AddThemeFontSizeOverride("EventSceneFontSize", 36);
            btn.Text = _eventScriptData.EventDesc[i] ?? "无描述";
            var compressedTexture2D = ResourceLoader.Load<CompressedTexture2D>(_eventScriptData.EventIconPath[i]);
            btn.Icon = compressedTexture2D;
            if (_eventScriptData.EventActions[i] != null) btn.Pressed += _eventScriptData.EventActions[i];
            else btn.Pressed += GoAhead;
            BtnContainer.AddChild(btn);
        }

        if (_eventScriptData.EventImgPath?.Length > 0)
            EventImg.Texture = ResourceLoader.Load<CompressedTexture2D>(_eventScriptData.EventImgPath);
        EventTitle.Text = _eventScriptData.EventTitle;
    }

    private static void GoAhead()
    {
        //StaticInstance.windowMgr.RemoveSceneByName("EventScene");
        StaticUtils.CloseEvent();
    }
}