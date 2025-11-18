using System;
using System.Collections.Generic;
using Godot;

namespace KemoCard.Scripts.Map;

public static class MapFactory
{
    private static readonly Dictionary<string, Type> MapDict = new();

    public static BaseMapScript CreateMap(string mapId)
    {
        if (MapDict.TryGetValue(mapId, out var value)) return (BaseMapScript)Activator.CreateInstance(value);
        var errorLog = $"试图创建不存在的地图:{mapId}";
        GD.PrintErr(errorLog);
        StaticInstance.MainRoot.ShowBanner(errorLog);
        return null;
    }

    public static void RegisterMap(string mapId, Type type)
    {
        if (string.IsNullOrWhiteSpace(mapId))
        {
            GD.PrintErr($"试图注册空地图Id的地图:{type.Name}");
            return;
        }

        if (type == null)
        {
            GD.PrintErr($"试图注册空地图Id的地图:{mapId}");
            return;
        }

        if (MapDict.ContainsKey(mapId))
        {
            GD.PrintErr($"试图注册重复地图Id的地图:{type.Name}");
            return;
        }

        if (type.IsSubclassOf(typeof(BaseMapScript)) || type == typeof(BaseMapScript))
        {
            MapDict[mapId] = type;
        }
        else
        {
            GD.PrintErr($"试图注册非地图类:{type.Name}");
        }
    }
}