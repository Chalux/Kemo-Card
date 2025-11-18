using Godot;
using KemoCard.Scripts;

namespace KemoCard.Pages;

public partial class BuffObject : Control, IEvent
{
    [Export] public Label CountLabel;
    [Export] public TextureRect Texture;
    [Export] public ColorRect Color;
    public BuffImplBase Data;

    public void Init(string id, object creator)
    {
        Data = new BuffImplBase(id)
        {
            Creator = creator
        };
        var res = ResourceLoader.Load<CompressedTexture2D>(Data.IconPath);
        if (res != null)
        {
            Texture.Texture = res;
        }

        InitObj();
    }

    public void Init(BuffImplBase buff)
    {
        Data = buff;
        var res = ResourceLoader.Load<CompressedTexture2D>(Data.IconPath);
        if (res != null)
        {
            Texture.Texture = res;
        }

        InitObj();
    }

    public void Update()
    {
        if (Data != null)
        {
            CountLabel.Text = Data.BuffCount < 99 ? Data.BuffCount.ToString() : "99+";
        }
    }

    public void ReceiveEvent(string @event, params object[] datas)
    {
        Data?.ReceiveEvent(@event, datas);
    }

    private void InitObj()
    {
        Data.BuffObj = this;
        MouseEntered += () =>
        {
            if (Data == null) return;
            var result = "Buff名：" + Data.BuffShowname + ",\n" +
                         "Buff剩余计数：" + (Data.IsInfinite ? "永久" : Data.BuffCount) + ",\n" +
                         "Buff量：" + Data.BuffValue + ",\n" +
                         "效果：" + Data.Desc;
            // HashSet<string> list = [];
            foreach (var key in Data.tags)
            {
                if (!StaticEnums.HintDictionary.TryGetValue(key, out var data1)) continue;
                result += $"\n{data1.Alias}:{data1.Desc}";
                // list.Add(key);
            }

            StaticInstance.MainRoot.ShowRichHint(StaticUtils.MakeBBCodeString(result, "left"));
        };
        MouseExited += () => { StaticInstance.MainRoot.HideRichHint(); };
        Update();
    }

    ~BuffObject()
    {
        Data = null;
    }
}