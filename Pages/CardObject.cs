using System;
using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;

namespace KemoCard.Pages;

public partial class CardObject : Control
{
    public Card Card;
    [Export] public RichTextLabel CardName;
    [Export] public RichTextLabel CardDesc;
    [Export] public Label CardCost;
    [Export] public ColorRect BgRect;
    [Export] public Polygon2D CostBg;
    [Export] public Area2D DropPointDetector;
    [Export] public Label StateLabel;
    [Export] public Scripts.Cards.CardStateMachine Csm;
    [Export] public Control BezierControl;
    [Export] public SubViewportContainer SvContainer;
    [Export] public ColorRect FrameRect;
    [Export] public TextureRect EfRect;
    private Vector2 _destinationTransform;
    private Vector2 _destinationScales;
    private float _destinationRotation;
    private float _interpotationSpeed;
    private bool _isRepositioning;
    public Tween AnimTween;
    private bool _isDragging;
    private Node _sourceRoot;
    private int _tempIndex;
    [Export] public float AngleXMax = 0.15f;
    [Export] public float AngleYMax = 0.15f;
    public Tween EnterTween;
    private bool _showHint = true;

    public bool ShowHint
    {
        get => _showHint;
        set
        {
            _showHint = value;
            if (!value)
            {
                StaticInstance.MainRoot.HideRichHint();
            }
        }
    }

    public override void _Ready()
    {
        StateLabel.Visible = OS.IsDebugBuild();
    }

    public void InitData(Card card)
    {
        Card = card;
        UpdateCardObject();
        Csm?.Init(this);
    }

    private void UpdateCardObject()
    {
        CardName.Text = StaticUtils.MakeBBCodeString(Card.Alias);
        CardDesc.Text = StaticUtils.MakeBBCodeString(Card.Desc);
        CardCost.Text = Card.Cost.ToString();
        CostBg.Color = new Color(StaticEnums.CostBgColor[Card.CostType]);
        SetRare();
    }

    private void Reposition()
    {
        if (_isDragging)
        {
            return;
        }

        AnimTween?.Kill();
        AnimTween = CreateTween();
        AnimTween.BindNode(this).SetParallel();
        //tween.TweenProperty(this, "position", DestinationTransform, Math.Abs(Position.X - DestinationTransform.X) / InterpotationSpeed);
        //tween.TweenProperty(this, "rotation", DestinationRotation, Math.Abs(Rotation - DestinationRotation) / InterpotationSpeed);
        AnimTween.TweenProperty(this, "position", _destinationTransform, _interpotationSpeed);
        AnimTween.TweenProperty(this, "rotation", _destinationRotation, _interpotationSpeed);
        if (!Scale.Equals(_destinationScales))
            AnimTween.TweenProperty(this, "scale", _destinationScales, 0.5f).SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Elastic);
        AnimTween.Finished += RepositionCompelete;
        AnimTween.Play();
    }

    private void RepositionCompelete()
    {
        _isRepositioning = false;
    }

    private bool HasReachedDestination()
    {
        return Math.Abs(Position.X - _destinationTransform.X) < 0.1 &&
               Math.Abs(Position.Y - _destinationTransform.Y) < 0.1 && Math.Abs(Rotation - _destinationRotation) < 0.1;
    }

    private bool HasCardSizeBeenEstablished()
    {
        return Size.X != 0 && Size.Y != 0;
    }

    public void StartReposition(Vector2 vector, float rotation, float interpotationSpeed, Vector2 scales)
    {
        _destinationTransform = vector;
        _destinationRotation = rotation;
        _interpotationSpeed = interpotationSpeed;
        _destinationScales = scales;
        Reposition();
        _isRepositioning = true;
    }

    public override void _Notification(int what)
    {
        switch (what)
        {
            case (int)NotificationMouseEnter:
                OnMouseEnter();
                break;
            case (int)NotificationMouseExit:
                OnMouseExit();
                break;
        }
    }

    private void OnMouseEnter()
    {
        var objects = new object[] { GetIndex() };
        Csm.OnMouseEnter();
        StaticInstance.EventMgr.Dispatch("RepositionHand", objects);
        var desc = Card.GetDesc;
        if (desc != "" && ShowHint)
        {
            StaticInstance.MainRoot.ShowRichHint(StaticUtils.MakeBBCodeString(desc, "left"));
        }
        // if (BattleStatic.currCard == this) return;
    }

    private void OnMouseExit()
    {
        var objects2 = new object[] { -1 };
        Csm.OnMouseExit();
        StaticInstance.EventMgr.Dispatch("RepositionHand", objects2);
        StaticInstance.MainRoot.HideRichHint();
        //if (ExitTween != null && ExitTween.IsRunning()) ExitTween.Kill();
        //ExitTween = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back).SetParallel(true);
        //ExitTween.TweenProperty(this, "scale", new Vector2(1.2f, 1.2f), 0.5f);
        if (SvContainer.Material is not ShaderMaterial sm) return;
        sm.SetShaderParameter("x_rot", 0);
        sm.SetShaderParameter("y_rot", 0);

        // if (BattleStatic.currCard == this) return;
    }

    public override void _Input(InputEvent @event)
    {
        Csm.OnInput(@event);
    }

    public override void _GuiInput(InputEvent @event)
    {
        Csm.OnGUIInput(@event);
        if (@event is not InputEventMouseMotion) return;
        var mousePos = GetGlobalMousePosition();
        var diff = (SvContainer.GlobalPosition + SvContainer.Size) - mousePos;
        var lerpValueX = Mathf.Remap(diff.X, 0, Size.X, 0, 1);
        var lerpValueY = Mathf.Remap(diff.Y, 0, Size.Y, 0, 1);
        //var RotX = Mathf.RadToDeg(Mathf.LerpAngle(-AngleXMax, AngleXMax, LerpValueX));
        //var RotY = Mathf.RadToDeg(Mathf.LerpAngle(-AngleYMax, AngleYMax, LerpValueY));
        var rotX = Mathf.RadToDeg(Mathf.LerpAngle(-AngleYMax, AngleYMax, lerpValueY));
        var rotY = Mathf.RadToDeg(Mathf.LerpAngle(AngleXMax, -AngleXMax, lerpValueX));
        //GD.Print(RotX, RotY);
        if (SvContainer.Material is not ShaderMaterial sm) return;
        sm.SetShaderParameter("x_rot", rotX);
        sm.SetShaderParameter("y_rot", rotY);
    }

    public void BindRoot(Node root)
    {
        _sourceRoot = root;
        _tempIndex = GetIndex();
    }

    public void ReparentToSourceRoot()
    {
        if (_sourceRoot == null) return;
        //GD.Print(TempIndex);
        Reparent(_sourceRoot);
        GetParent().MoveChild(this, _tempIndex);
    }

    public override void _Process(double delta)
    {
        Csm.Process(delta);
    }

    private void SetRare()
    {
        if (FrameRect.Material is ShaderMaterial fm)
            fm.SetShaderParameter("color",
                new Color(StaticUtils.GetFrameColorByRare(Card.Rare)));
    }
}