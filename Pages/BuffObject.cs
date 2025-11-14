using Godot;
using KemoCard.Scripts;
using System.Collections.Generic;

public partial class BuffObject : Control, IEvent
{
    [Export] public Label CountLabel;
    [Export] public TextureRect texture;
    [Export] public ColorRect color;
    public BuffImplBase data;
    public void Init(string id, object Creator)
    {
        data = new(id)
        {
            Creator = Creator
        };
        CompressedTexture2D res = ResourceLoader.Load<CompressedTexture2D>(data.IconPath);
        if (res != null)
        {
            texture.Texture = res;
        }
        InitObj();
    }

    public void Init(BuffImplBase buff)
    {
        data = buff;
        CompressedTexture2D res = ResourceLoader.Load<CompressedTexture2D>(data.IconPath);
        if (res != null)
        {
            texture.Texture = res;
        }
        InitObj();
    }

    public void Update()
    {
        if (data != null)
        {
            if (data.BuffCount < 99)
                CountLabel.Text = data.BuffCount.ToString();
            else
                CountLabel.Text = "99+";
        }
    }

    public void ReceiveEvent(string @event, params object[] datas)
    {
        data?.ReceiveEvent(@event, datas);
    }

    private void InitObj()
    {
        data.BuffObj = this;
        MouseEntered += new(() =>
        {
            if (data != null)
            {
                string result = "Buff名：" + data.BuffShowname + ",\n" +
                    "Buff剩余计数：" + (data.IsInfinite ? "永久" : data.BuffCount) + ",\n" +
                    "Buff量：" + data.BuffValue + ",\n" +
                    "效果：" + data.Desc;
                HashSet<string> list = new();
                foreach (var key in data.tags)
                {
                    if (StaticEnums.HintDictionary.ContainsKey(key))
                    {
                        var data = StaticEnums.HintDictionary[key];
                        result += $"\n{data.Alias}:{data.Desc}";
                        list.Add(key);
                    }
                }
                StaticInstance.MainRoot.ShowRichHint(StaticUtils.MakeBBCodeString(result, "left"));
            }
        });
        MouseExited += new(() =>
        {
            StaticInstance.MainRoot.HideRichHint();
        });
        Update();
    }

    ~BuffObject()
    {
        data = null;
    }
}
