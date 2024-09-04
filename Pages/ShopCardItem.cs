using Godot;
using KemoCard.Scripts.Interfaces;
using StaticClass;

public partial class ShopCardItem : Control, IUpdateAble
{
    [Export] CardShowObject ShowObject;
    [Export] Godot.Button BuyButton;
    private ShopStruct _struct;
    public void Init(ShopStruct @struct)
    {
        _struct = @struct;
        Update();
        BuyButton.Pressed += () =>
        {
            if (StaticInstance.playerData.gsd.MajorRole.Gold >= _struct.price)
            {
                StaticInstance.playerData.gsd.MajorRole.Gold -= _struct.price;
                StaticInstance.playerData.gsd.MajorRole.TempDeck.Add(_struct.card);
                _struct.isBuyed = true;
                //StaticUtils.AutoSave();
                Update();
            }
        };
    }
    public void Update()
    {
        ShowObject.InitDataByCard(_struct.card);
        BuyButton.Text = _struct.price.ToString();
        BuyButton.AddThemeColorOverride("font_color", StaticInstance.playerData.gsd.MajorRole.Gold >= _struct.price ? Colors.White : Colors.Red);
        if (_struct.isBuyed == true)
        {
            ShowObject.Visible = false;
            BuyButton.Visible = false;
            BuyButton.Disabled = true;
        }
        else
        {
            ShowObject.Visible = true;
            BuyButton.Visible = true;
            BuyButton.Disabled = false;
        }
    }
}
