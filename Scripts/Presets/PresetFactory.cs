using System;
using System.Collections.Generic;
using Godot;

namespace KemoCard.Scripts.Presets;

public static class PresetFactory
{
    private static readonly Dictionary<string, Type> PresetDict = new();

    public static BasePresetScript CreatePreset(string presetId)
    {
        if (PresetDict.TryGetValue(presetId, out var value)) return (BasePresetScript)Activator.CreateInstance(value);
        var errorLog = $"试图创建不存在的预设:{presetId}";
        GD.PrintErr(errorLog);
        StaticInstance.MainRoot.ShowBanner(errorLog);
        return null;
    }

    public static void RegisterPreset(string presetId, Type presetType)
    {
        if (string.IsNullOrWhiteSpace(presetId))
        {
            GD.PrintErr($"试图注册空预设Id的预设:{presetType.Name}");
            return;
        }

        if (presetType == null)
        {
            GD.PrintErr($"试图注册空预设:{presetId}");
            return;
        }

        if (PresetDict.ContainsKey(presetId))
        {
            GD.PrintErr($"重复注册预设Id:{presetId}");
            return;
        }

        if (!typeof(BasePresetScript).IsAssignableFrom(presetType))
        {
            GD.PrintErr($"试图注册非预设类:{presetType.Name}");
            return;
        }

        PresetDict.Add(presetId, presetType);
    }
}