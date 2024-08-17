using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;

public partial class CardObject : Control
{
    public Card card;
    [Export] public RichTextLabel cardName;
    [Export] public RichTextLabel cardDesc;
    [Export] public Label cardCost;
    [Export] public ColorRect colorRect;
    [Export] public Polygon2D costBg;
    [Export] public Area2D dropPointDetector;
    [Export] public Label stateLabel;
    [Export] public CardStateMachine csm;
    [Export] public Control BezierControl;
    [Export] public SubViewportContainer SVContainer;
    private Vector2 DestinationTransform = new();
    private Vector2 DestinationScales = new();
    private float DestinationRotation;
    private float InterpotationSpeed = 0;
    private bool IsRepositioning = false;
    public Tween AnimTween;
    private bool isDraging = false;
    private Node sourceRoot;
    public int TempIndex = 0;
    [Export] public float AngleXMax = 0.15f;
    [Export] public float AngleYMax = 0.15f;
    public Tween EnterTween;
    private bool _ShowHint = true;
    public bool ShowHint
    {
        get => _ShowHint;
        set
        {
            _ShowHint = value;
            if (!value)
            {
                StaticInstance.MainRoot.HideRichHint();
            }
        }
    }

    public override void _Ready()
    {
        stateLabel.Visible = OS.IsDebugBuild();
    }

    public void InitData(Card card)
    {
        this.card = card;
        UpdateCardObject();
        csm?.Init(this);
    }

    void UpdateCardObject()
    {
        cardName.Text = StaticUtils.MakeBBCodeString(card.Alias);
        cardDesc.Text = StaticUtils.MakeBBCodeString(card.Desc);
        cardCost.Text = card.Cost.ToString();
        costBg.Color = new(StaticEnums.CostBgColor[card.CostType]);
    }

    void Reposition()
    {
        if (isDraging)
        {
            return;
        }
        if (AnimTween != null)
        {
            AnimTween.Kill();
        }
        AnimTween = CreateTween();
        AnimTween.BindNode(this).SetParallel(true);
        //tween.TweenProperty(this, "position", DestinationTransform, Math.Abs(Position.X - DestinationTransform.X) / InterpotationSpeed);
        //tween.TweenProperty(this, "rotation", DestinationRotation, Math.Abs(Rotation - DestinationRotation) / InterpotationSpeed);
        AnimTween.TweenProperty(this, "position", DestinationTransform, InterpotationSpeed);
        AnimTween.TweenProperty(this, "rotation", DestinationRotation, InterpotationSpeed);
        if (!Scale.Equals(DestinationScales)) AnimTween.TweenProperty(this, "scale", DestinationScales, 0.5f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
        AnimTween.Finished += new(() =>
        {
            RepositionCompelete();
        });
        AnimTween.Play();
    }

    public void RepositionCompelete()
    {
        IsRepositioning = false;
    }

    bool HasReachedDestination()
    {
        return Position.X == DestinationTransform.X && Position.Y == DestinationTransform.Y && Rotation == DestinationRotation;
    }

    bool HasCardSizeBeenEstablished()
    {
        return Size.X != 0 && Size.Y != 0;
    }

    public void StartReposition(Vector2 vector, float rotation, float interpotationSpeed, Vector2 scales)
    {
        DestinationTransform = vector;
        DestinationRotation = rotation;
        InterpotationSpeed = interpotationSpeed;
        DestinationScales = scales;
        Reposition();
        IsRepositioning = true;
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

    void OnMouseEnter()
    {
        object[] objects = new object[] { GetIndex() };
        csm.OnMouseEnter();
        StaticInstance.eventMgr.Dispatch("RepositionHand", objects);
        string desc = card.GetDesc;
        if (desc != "" && ShowHint)
        {
            StaticInstance.MainRoot.ShowRichHint(StaticUtils.MakeBBCodeString(desc, "left"));
        }
        if (BattleStatic.currCard == this) return;
    }

    void OnMouseExit()
    {
        object[] objects2 = new object[] { -1 };
        csm.OnMouseExit();
        StaticInstance.eventMgr.Dispatch("RepositionHand", objects2);
        StaticInstance.MainRoot.HideRichHint();
        //if (ExitTween != null && ExitTween.IsRunning()) ExitTween.Kill();
        //ExitTween = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back).SetParallel(true);
        //ExitTween.TweenProperty(this, "scale", new Vector2(1.2f, 1.2f), 0.5f);
        (SVContainer.Material as ShaderMaterial).SetShaderParameter("x_rot", 0);
        (SVContainer.Material as ShaderMaterial).SetShaderParameter("y_rot", 0);
        if (BattleStatic.currCard == this) return;
    }

    public override void _Input(InputEvent @event)
    {
        csm.OnInput(@event);
    }

    public override void _GuiInput(InputEvent @event)
    {
        csm.OnGUIInput(@event);
        if (@event is not InputEventMouseMotion) return;
        var MousePos = GetGlobalMousePosition();
        var Diff = (SVContainer.GlobalPosition + SVContainer.Size) - MousePos;
        var LerpValueX = Mathf.Remap(Diff.X, 0, Size.X, 0, 1);
        var LerpValueY = Mathf.Remap(Diff.Y, 0, Size.Y, 0, 1);
        var RotX = Mathf.RadToDeg(Mathf.LerpAngle(-AngleXMax, AngleXMax, LerpValueX));
        var RotY = Mathf.RadToDeg(Mathf.LerpAngle(-AngleYMax, AngleYMax, LerpValueY));
        //GD.Print(RotX, RotY);
        (SVContainer.Material as ShaderMaterial).SetShaderParameter("x_rot", RotX);
        (SVContainer.Material as ShaderMaterial).SetShaderParameter("y_rot", RotY);
    }

    public void BindRoot(Node root)
    {
        sourceRoot = root;
        TempIndex = GetIndex();
    }

    public void ReparentToSourceRoot()
    {
        if (sourceRoot != null)
        {
            //GD.Print(TempIndex);
            Reparent(sourceRoot);
            GetParent().MoveChild(this, TempIndex);
        }
    }

    public override void _Process(double delta)
    {
        csm.Process(delta);
    }
}
