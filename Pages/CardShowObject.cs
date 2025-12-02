using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;

namespace KemoCard.Pages;

public partial class CardShowObject : Control
{
    [Export] public RichTextLabel CardName;
    [Export] public RichTextLabel CardDesc;
    [Export] public Label CardCost;
    [Export] public ColorRect BgRect;
    [Export] public Polygon2D CostBg;
    [Export] public ColorRect FrameRect;
    [Export] public SubViewportContainer SvContainer;
    [Export] public float AngleXMax = 0.15f;
    [Export] public float AngleYMax = 0.15f;
    private Tween _animTween;
    public Card Card;

    private void InitData(string id)
    {
        if (!Datas.Ins.CardPool.TryGetValue(id, out var cData)) return;
        Card card = new();
        var script = CardFactory.CreateCard(cData.CardId);
        script?.OnCardScriptInit(card);
        Card = card;
        UpdateCardObject();
    }

    public void InitDataByCard(Card card)
    {
        Card = card;
        UpdateCardObject();
    }

    public override void _Ready()
    {
        MouseEntered += OnMouseEnter;
        MouseExited += OnMouseExit;
    }

    private void OnMouseEnter()
    {
        _animTween?.Kill();
        _animTween = CreateTween();
        _animTween.TweenProperty(this, "scale", new Vector2(1.2f, 1.2f), 0.5f).SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);
        var desc = Card?.GetDesc;
        if (!string.IsNullOrEmpty(desc))
        {
            StaticInstance.MainRoot.ShowRichHint(StaticUtils.MakeBBCodeString(desc, "left"));
        }
    }

    private void OnMouseExit()
    {
        _animTween?.Kill();
        _animTween = CreateTween();
        _animTween.TweenProperty(this, "scale", Vector2.One, 0.5f).SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);
        if (SvContainer.Material is ShaderMaterial sm)
        {
            sm.SetShaderParameter("x_rot", 0);
            sm.SetShaderParameter("y_rot", 0);
        }

        StaticInstance.MainRoot?.HideRichHint();
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
        {
            var mousePos = GetGlobalMousePosition();
            var diff = (SvContainer.GlobalPosition + SvContainer.Size) - mousePos;
            var lerpValueX = Mathf.Remap(diff.X, 0, Size.X, 0, 1);
            var lerpValueY = Mathf.Remap(diff.Y, 0, Size.Y, 0, 1);
            //var RotX = Mathf.RadToDeg(Mathf.LerpAngle(-AngleXMax, AngleXMax, LerpValueX));
            //var RotY = Mathf.RadToDeg(Mathf.LerpAngle(-AngleYMax, AngleYMax, LerpValueY));
            var rotX = Mathf.RadToDeg(Mathf.LerpAngle(-AngleYMax, AngleYMax, lerpValueY));
            var rotY = Mathf.RadToDeg(Mathf.LerpAngle(AngleXMax, -AngleXMax, lerpValueX));
            if (SvContainer.Material is not ShaderMaterial sm) return;
            sm.SetShaderParameter("x_rot", rotX);
            sm.SetShaderParameter("y_rot", rotY);
        }
        else if (OS.IsDebugBuild() && @event is InputEventMouseButton imb &&
                 imb.IsCtrlPressed() &&
                 imb.IsActionPressed("left_mouse") && SvContainer.Material is ShaderMaterial sm)
        {
            _animTween?.Kill();
            _animTween = CreateTween();
            _animTween.TweenProperty(SvContainer.Material, "shader_parameter/dissolve_value",
                0.0, 1f);
            _animTween.TweenCallback(Callable.From(() => sm.SetShaderParameter("dissolve_value", 1.0f)));
        }
    }

    private void UpdateCardObject()
    {
        CardName.Text = StaticUtils.MakeBBCodeString(Card.Alias);
        CardDesc.Text = StaticUtils.MakeBBCodeString(Card.Desc);
        CardCost.Text = Card.Cost.ToString();
        CostBg.Color = new Color(StaticEnums.CostBgColor[Card.CostType]);
        SetRare();
    }

    private void SetRare()
    {
        (FrameRect.Material as ShaderMaterial)?.SetShaderParameter("color",
            new Color(StaticUtils.GetFrameColorByRare(Card.Rare)));
    }

    public override void _ExitTree()
    {
        _animTween?.Kill();
        base._ExitTree();
    }
}