using System;
using System.Collections.Generic;
using Godot;

namespace KemoCard.Scripts.Buffs;

public static class BuffFactory
{
    private static readonly Dictionary<string, Type> BuffMap = new();

    public static BaseBuffScript CreateBuff(string buffId)
    {
        if (BuffMap.TryGetValue(buffId, out var value)) return (BaseBuffScript)Activator.CreateInstance(value);
        var errorLog = $"试图创建不存在的buff:{buffId}";
        GD.PrintErr(errorLog);
        StaticInstance.MainRoot.ShowBanner(errorLog);
        return null;
    }

    public static void RegisterBuff(string buffId, Type buffType)
    {
        if (string.IsNullOrWhiteSpace(buffId))
        {
            GD.PrintErr($"试图注册空buffId的buff:{buffType.Name}");
            return;
        }

        if (buffType == null)
        {
            GD.PrintErr($"试图注册空类型的buff:{buffId}");
            return;
        }

        if (BuffMap.ContainsKey(buffId))
        {
            GD.PrintErr($"试图注册已存在的buff:{buffId}");
            return;
        }

        if (buffType.IsSubclassOf(typeof(BaseBuffScript)) || buffType == typeof(BaseBuffScript))
        {
            BuffMap[buffId] = buffType;
        }
        else
        {
            GD.PrintErr($"试图注册非buff的类:{buffType.Name}, Id为{buffId}");
        }
    }
}