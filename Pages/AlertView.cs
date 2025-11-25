using System;
using Godot;
using KemoCard.Scripts;

namespace KemoCard.Pages;

public partial class AlertView : BaseScene
{
    [Export] private RichTextLabel _msg;
    [Export] private Button _confirmBtn;
    [Export] private Button _cancelBtn;
    [Export] private Timer _cdTimer;
    private Action _confirmAction;
    private Action _cancelAction;
    private int _cd = -1;
    private string _msgValue = "";
    private bool _useCustomStyle;

    public static void PopupAlert(string msg, bool useCustomStyle = false, Action confirmAction = null,
        Action cancelAction = null, int cd = -1)
    {
        if (StaticInstance.WindowMgr.IsPopupScene("AlertView"))
        {
            StaticInstance.MainRoot.ShowBanner("已有一个提示弹窗");
            return;
        }

        var alertView = (AlertView)ResourceLoader.Load<PackedScene>("res://Pages/AlertView.tscn").Instantiate();
        alertView._confirmAction = confirmAction;
        alertView._cancelAction = cancelAction;
        alertView._msgValue = msg;
        alertView._useCustomStyle = useCustomStyle;
        alertView._cd = cd;
        StaticInstance.WindowMgr.PopScene(alertView);
    }

    // Called when the node enters the scene tree for the first time.
    public override void OnAdd(params object[] datas)
    {
        _confirmBtn.Pressed += OnConfirmBtnOnPressed;
        _cancelBtn.Pressed += OnCancelBtnOnPressed;
        _cdTimer.Timeout += CdTimerOnTimeout;
        if (_cd > 0)
        {
            _cdTimer.Start();
        }

        UpdateMsgValue();
    }

    private void CdTimerOnTimeout()
    {
        if (_cd <= 0)
        {
            _cancelAction?.Invoke();
            StaticInstance.WindowMgr.RemoveScene(this);
            return;
        }

        UpdateMsgValue();
        _cd--;
    }

    private void UpdateMsgValue()
    {
        if (_cd > 0)
        {
            if (_useCustomStyle) _msg.Text = _msgValue.ReplaceN("{time}", _cd.ToString());
            else
            {
                var str = "[font_size=50]" + _msgValue + "[/font_size]";
                _msg.Text = str.ReplaceN("{time}", _cd.ToString());
            }
        }
        else
        {
            if (_useCustomStyle) _msg.Text = _msgValue;
            else _msg.Text = "[font_size=50]" + _msgValue + "[/font_size]";
        }
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

    public override void OnRemove()
    {
        _confirmAction = null;
        _cancelAction = null;
        _cd = -1;
        _cdTimer.Stop();
        _cdTimer.Timeout -= CdTimerOnTimeout;
        _cancelBtn.Pressed -= OnCancelBtnOnPressed;
        _confirmBtn.Pressed -= OnConfirmBtnOnPressed;
    }
}