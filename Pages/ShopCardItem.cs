using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Interfaces;

namespace KemoCard.Pages;

public partial class ShopCardItem : Control, IUpdateAble
{
    [Export] private CardShowObject _showObject;
    [Export] private Button _buyButton;
    private ShopStruct _struct;

    public void Init(ShopStruct @struct)
    {
        _struct = @struct;
        Update();
        _buyButton.Pressed += () =>
        {
            if (StaticInstance.PlayerData.Gsd.MajorRole.Gold < _struct.Price) return;
            StaticInstance.PlayerData.Gsd.MajorRole.Gold -= _struct.Price;
            StaticInstance.PlayerData.Gsd.MajorRole.TempDeck.Add(_struct.Card);
            _struct.IsBought = true;
            //StaticUtils.AutoSave();
            Update();
        };
    }

    public void Update()
    {
        _showObject.InitDataByCard(_struct.Card);
        _buyButton.Text = _struct.Price.ToString();
        _buyButton.AddThemeColorOverride("font_color",
            StaticInstance.PlayerData.Gsd.MajorRole.Gold >= _struct.Price ? Colors.White : Colors.Red);
        if (_struct.IsBought)
        {
            _showObject.Visible = false;
            _buyButton.Visible = false;
            _buyButton.Disabled = true;
        }
        else
        {
            _showObject.Visible = true;
            _buyButton.Visible = true;
            _buyButton.Disabled = false;
        }
    }
}