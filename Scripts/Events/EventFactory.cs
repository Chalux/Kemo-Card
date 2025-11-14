using System;
using System.Collections.Generic;
using Godot;

namespace KemoCard.Scripts.Events;

public static class EventFactory
{
    private static readonly Dictionary<string, Type> EventDict = new();

    public static BaseEventScript CreateEvent(string eventId)
    {
        if (EventDict.TryGetValue(eventId, out var value)) return (BaseEventScript)Activator.CreateInstance(value);
        var errorLog = $"试图创建不存在的事件:{eventId}";
        GD.PrintErr(errorLog);
        StaticInstance.MainRoot.ShowBanner(errorLog);
        return null;
    }

    public static void RegisterEvent(string eventId, Type type)
    {
        if (string.IsNullOrWhiteSpace(eventId))
        {
            GD.PrintErr($"试图注册空事件Id的事件:{type.Name}");
            return;
        }

        if (type == null)
        {
            GD.PrintErr($"试图注册空事件:{eventId}");
            return;
        }

        if (EventDict.ContainsKey(eventId))
        {
            GD.PrintErr($"试图注册重复的事件Id:{eventId}");
            return;
        }

        if (!type.IsSubclassOf(typeof(BaseEventScript)) || type == typeof(BaseEventScript))
        {
            GD.PrintErr($"试图注册非事件类:{type.Name}");
            return;
        }

        EventDict[eventId] = type;
    }
}