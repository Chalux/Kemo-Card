using Godot;
using KemoCard.Pages;
using KemoCard.Scripts;
using KemoCard.Scripts.Interfaces;

public partial class ShopCardItem : Control, IUpdateAble
{
    [Export] KemoCard.Pages.CardShowObject ShowObject;
    [Export] Godot.Button BuyButton;
    private ShopStruct _struct;
    public void Init(ShopStruct @struct)
    {
        _struct = @struct;
        Update();
        BuyButton.Pressed += () =>
        {
            if (StaticInstance.PlayerData.Gsd.MajorRole.Gold >= _struct.Price)
            {
                StaticInstance.PlayerData.Gsd.MajorRole.Gold -= _struct.Price;
                StaticInstance.PlayerData.Gsd.MajorRole.TempDeck.Add(_struct.Card);
                _struct.IsBought = true;
                //StaticUtils.AutoSave();
                Update();
            }
        };
    }
    public void Update()
    {
        ShowObject.InitDataByCard(_struct.Card);
        BuyButton.Text = _struct.Price.ToString();
        BuyButton.AddThemeColorOverride("font_color", StaticInstance.PlayerData.Gsd.MajorRole.Gold >= _struct.Price ? Colors.White : Colors.Red);
        if (_struct.IsBought == true)
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
