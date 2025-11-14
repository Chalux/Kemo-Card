using System;
using Godot;
using KemoCard.Scripts;

namespace KemoCard.Pages;

public partial class AlertView : BaseScene
{
    [Export] private RichTextLabel _msg;
    [Export] private Godot.Button _confirmBtn;
    [Export] private Godot.Button _cancelBtn;
    private Action _confirmAction;
    private Action _cancelAction;

    public static void PopupAlert(string msg, bool useCustomStyle = false, Action confirmAction = null,
        Action cancelAction = null)
    {
        if (StaticInstance.WindowMgr.IsPopupScene("AlertView"))
        {
            StaticInstance.MainRoot.ShowBanner("已有一个提示弹窗");
            return;
        }

        var alertView = (AlertView)ResourceLoader.Load<PackedScene>("res://Pages/AlertView.tscn").Instantiate();
        alertView._confirmAction = confirmAction;
        alertView._cancelAction = cancelAction;
        if (useCustomStyle) alertView._msg.Text = msg;
        else alertView._msg.Text = "[font_size=50]" + msg + "[/font_size]";
        StaticInstance.WindowMgr.PopScene(alertView);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _confirmBtn.Pressed += OnConfirmBtnOnPressed;
        _cancelBtn.Pressed += OnCancelBtnOnPressed;
    }

    private void OnCancelBtnOnPressed()
    {
        _cancelAction?.Invoke();
        StaticInstance.WindowMgr.RemoveScene(this);
    }

    private void OnConfirmBtnOnPressed()
    {
        _confirmAction?.Invoke();
        StaticInstance.WindowMgr.RemoveScene(this);
    }
}