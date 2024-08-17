using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;

public partial class CardShowObject : Control
{
    [Export] public RichTextLabel cardName;
    [Export] public RichTextLabel cardDesc;
    [Export] public Label cardCost;
    [Export] public ColorRect colorRect;
    [Export] public Polygon2D costBg;
    [Export] public SubViewportContainer SVContainer;
    [Export] public float AngleXMax = 0.15f;
    [Export] public float AngleYMax = 0.15f;
    public Tween AnimTween;
    public Card card;

    public void InitData(string id)
    {
        if (!Datas.Ins.CardPool.ContainsKey(id)) return;
        var cdata = Datas.Ins.CardPool[id];
        CSharpScript res = ResourceLoader.Load<CSharpScript>("res://Mods/" + cdata.mod_id + "/Scripts/C" + cdata.card_id + ".cs");
        Card card = new();
        if (res != null)
        {
            var cardScript = res.New().As<BaseCardScript>();
            cardScript.OnCardScriptInit(card);
        }
        this.card = card;
        UpdateCardObject();
    }

    public void InitDataByCard(Card card)
    {
        this.card = card;
        UpdateCardObject();
    }

    public override void _Ready()
    {
        MouseEntered += new(() =>
        {
            AnimTween?.Kill();
            AnimTween = CreateTween();
            AnimTween.TweenProperty(this, "scale", new Vector2(1.2f, 1.2f), 0.5f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
        });
        MouseExited += new(() =>
        {
            AnimTween?.Kill();
            AnimTween = CreateTween();
            AnimTween.TweenProperty(this, "scale", Vector2.One, 0.5f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
            (SVContainer.Material as ShaderMaterial).SetShaderParameter("x_rot", 0);
            (SVContainer.Material as ShaderMaterial).SetShaderParameter("y_rot", 0);
        });
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is not InputEventMouseMotion) return;
        var MousePos = GetGlobalMousePosition();
        var Diff = (SVContainer.GlobalPosition + SVContainer.Size) - MousePos;
        var LerpValueX = Mathf.Remap(Diff.X, 0, Size.X, 0, 1);
        var LerpValueY = Mathf.Remap(Diff.Y, 0, Size.Y, 0, 1);
        var RotX = Mathf.RadToDeg(Mathf.LerpAngle(-AngleXMax, AngleXMax, LerpValueX));
        var RotY = Mathf.RadToDeg(Mathf.LerpAngle(-AngleYMax, AngleYMax, LerpValueY));
        (SVContainer.Material as ShaderMaterial).SetShaderParameter("x_rot", RotX);
        (SVContainer.Material as ShaderMaterial).SetShaderParameter("y_rot", RotY);
    }

    void UpdateCardObject()
    {
        cardName.Text = StaticUtils.MakeBBCodeString(card.Alias);
        cardDesc.Text = StaticUtils.MakeBBCodeString(card.Desc);
        cardCost.Text = card.Cost.ToString();
        costBg.Color = new(StaticEnums.CostBgColor[card.CostType]);
    }
}
