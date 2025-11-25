using System;
using Godot;
using KemoCard.Scripts;

namespace KemoCard.Pages;

public partial class SettingScene : BaseScene
{
    [Export] private OptionButton _screenResolution;

    [Export] private Button _applyResolution;
    [Export] private Button _closeBtn;

    // Called when the node enters the scene tree for the first time.
    public override void OnAdd(params object[] data)
    {
        var screenResolution = GetTree().Root.Size;
        _screenResolution.Selected = screenResolution.X switch
        {
            1280 when screenResolution.Y == 720 => 0,
            1920 when screenResolution.Y == 1080 => 1,
            2560 when screenResolution.Y == 1440 => 2,
            _ => -1
        };

        _applyResolution.Pressed += () =>
        {
            var oldResolution = GetTree().Root.Size;
            switch (_screenResolution.Selected)
            {
                case 0:
                    GetTree().Root.SetSize(new Vector2I(1280, 720));
                    break;
                case 1:
                    GetTree().Root.SetSize(new Vector2I(1920, 1080));
                    break;
                case 2:
                    GetTree().Root.SetSize(new Vector2I(2560, 1440));
                    break;
            }

            AlertView.PopupAlert("是否确定应用此分辨率？({time}秒后回复原分辨率)", false, () =>
            {
                var resolution = _screenResolution.Selected switch
                {
                    0 => new Vector2I(1280, 720),
                    1 => new Vector2I(1920, 1080),
                    2 => new Vector2I(2560, 1440),
                    _ => GetTree().Root.Size
                };
                StaticInstance.SettingData.SetScreenResolution(resolution);
            }, () => { GetTree().Root.SetSize(oldResolution); }, 10);
        };

        _closeBtn.Pressed += CloseBtnOnPressed;
    }

    private void CloseBtnOnPressed()
    {
        StaticInstance.WindowMgr.RemoveScene(this);
    }
}