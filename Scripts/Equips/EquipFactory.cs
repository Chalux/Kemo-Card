using System;
using System.Collections.Generic;
using Godot;

namespace KemoCard.Scripts.Equips;

public static class EquipFactory
{
    private static readonly Dictionary<string, Type> EquipDict = new();

    public static BaseEquipScript CreateEquip(string equipId)
    {
        if (EquipDict.TryGetValue(equipId, out var value)) return (BaseEquipScript)Activator.CreateInstance(value);
        var errorLog = $"试图创建不存在的装备:{equipId}";
        StaticInstance.MainRoot.ShowBanner(errorLog);
        GD.PrintErr(errorLog);
        return null;
    }

    public static void RegisterEquip(string equipId, Type type)
    {
        if (string.IsNullOrWhiteSpace(equipId))
        {
            GD.PrintErr($"试图注册空装备Id的装备:{type.Name}");
            return;
        }

        if (type == null)
        {
            GD.PrintErr($"试图注册空装备:{equipId}");
            return;
        }

        if (EquipDict.ContainsKey(equipId))
        {
            GD.PrintErr($"重复注册装备Id:{equipId}");
            return;
        }

        if (!typeof(BaseEquipScript).IsAssignableFrom(type))
        {
            GD.PrintErr($"试图注册非装备类:{type.Name}");
            return;
        }

        EquipDict.Add(equipId, type);
    }
}