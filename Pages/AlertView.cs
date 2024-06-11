using Godot;
using KemoCard.Pages;
using StaticClass;
using System;

public partial class AlertView : BaseScene
{
    [Export] RichTextLabel msg;
    [Export] Godot.Button ConfirmBtn;
    [Export] Godot.Button CancelBtn;
    private Action _ConfirmAction;
    private Action _CancelAction;
    public static void PopupAlert(string msg, bool useCustomStyle = false, Action ConfirmAction = null, Action CancelAction = null)
    {
        if (StaticInstance.windowMgr.IsPopupScene("AlertView"))
        {
            StaticInstance.MainRoot.ShowBanner("已有一个提示弹窗");
            return;
        }
        AlertView alertView = (AlertView)ResourceLoader.Load<PackedScene>("res://Pages/AlertView.tscn").Instantiate();
        alertView._ConfirmAction = ConfirmAction;
        alertView._CancelAction = CancelAction;
        if (useCustomStyle) alertView.msg.Text = msg;
        else alertView.msg.Text = "[font_size=50]" + msg + "[/font_size]";
        StaticInstance.windowMgr.PopScene(alertView);
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ConfirmBtn.Pressed += new(() =>
        {
            _ConfirmAction?.Invoke();
            StaticInstance.windowMgr.RemoveScene(this);
        });
        CancelBtn.Pressed += new(() =>
        {
            _CancelAction?.Invoke();
            StaticInstance.windowMgr.RemoveScene(this);
        });
    }
}
