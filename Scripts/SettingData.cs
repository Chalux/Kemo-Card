using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

namespace KemoCard.Scripts;

public class SettingData
{
    private const string SettingFilePath = "user://setting.json";

    private readonly JsonSerializerOptions _options = new()
        { ReferenceHandler = ReferenceHandler.Preserve, IncludeFields = true };

    private SettingObj _settings;

    public SettingData()
    {
        Load();
    }

    public void Save()
    {
        if (_settings == null) return;
        using var save = FileAccess.Open(SettingFilePath, FileAccess.ModeFlags.Write);
        var jsonString = JsonSerializer.Serialize(_settings, _options);
        save.StoreString(jsonString);
    }

    private void Load()
    {
        if (!FileAccess.FileExists(SettingFilePath)) return;
        using var load = FileAccess.Open(SettingFilePath, FileAccess.ModeFlags.Read);
        try
        {
            _settings = JsonSerializer.Deserialize<SettingObj>(load.GetAsText(), _options);
        }
        catch (Exception e)
        {
            GD.PrintErr($"系统设置读取失败，重置为默认设置，错误信息:{e}");
            _settings = new SettingObj();
        }
    }

    public Vector2I GetScreenResolution()
    {
        return _settings.ScreenResolution;
    }

    public void SetScreenResolution(Vector2I resolution)
    {
        _settings.ScreenResolution = resolution;
        Save();
    }
}

internal class SettingObj
{
    public Vector2I ScreenResolution = new(1920, 1080);
}