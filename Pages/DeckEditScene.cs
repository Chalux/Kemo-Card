using Godot;
using KemoCard.Pages;
using System.Collections.Generic;
using System.Linq;

public partial class DeckEditScene : BaseScene
{
    [Export] MenuButton flagFilter;
    [Export] TextEdit IDFilter;
    [Export] TextEdit NameFilter;
    [Export] Godot.Button clearBtn;
    private int flags;
    private readonly Dictionary<int, string> flagList = new()
    {
        { 0x1 , "攻击" },
        { 0x2 , "护甲" },
        { 0x4 , "治疗" },
        { 0x8 , "无属性" },
        { 0x10 , "水属性" },
        { 0x20 , "火属性" },
        { 0x40 , "地属性" },
        { 0x80 , "风属性" },
        { 0x100 , "光属性" },
        { 0x200 , "暗属性" },
        { 0x400 , "物理" },
        { 0x800 , "魔法" },
        { 0x1000 , "抽卡" }
    };
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var popup = flagFilter.GetPopup();
        foreach (var flag in flagList.Values)
        {
            popup.AddCheckItem(flag);
        }
        popup.IndexPressed += new((index) =>
        {
            var text = popup.GetItemText((int)index);
            int value = flagList.ElementAt((int)index).Key;
            if ((flags & value) > 0)
            {
                flags &= ~value;
            }
            else
            {
                flags |= value;
            }
            ReDrawMenu();
        });
        clearBtn.Pressed += new(() =>
        {
            flags = 0;
            ReDrawMenu();
        });
    }

    void ReDrawMenu()
    {
        if (flagFilter != null)
        {
            for (int i = 0; i < flagFilter.GetPopup().ItemCount; i++)
            {
                flagFilter.GetPopup().SetItemChecked(i, (flags & flagList.ElementAt(i).Key) > 0);
            }
        }
    }
}
