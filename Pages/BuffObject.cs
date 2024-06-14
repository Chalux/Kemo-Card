using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Buffs;
using StaticClass;
using System.Collections.Generic;

public partial class BuffObject : Control, IEvent
{
    [Export] public Label CountLabel;
    [Export] public TextureRect texture;
    [Export] public ColorRect color;
    public BuffImplBase data;
    public void Init(uint id)
    {
        var modInfo = Datas.Ins.BuffPool.GetValueOrDefault(id, new() { buff_id = 0, buff_name = "", mod_id = "MainPackage" });
        if (modInfo.buff_id != 0)
        {
            string spath = "res://Mods/{modInfo.mod_id}/buff/B{modInfo.buff_id}.cs";
            FileAccess script = FileAccess.Open(spath, FileAccess.ModeFlags.Read);
            data = new();
            if (script != null)
            {
                var s = ResourceLoader.Load<CSharpScript>(spath).New().As<BaseBuffScript>();
                s.OnBuffInit(data);
            }
            string path = ProjectSettings.GlobalizePath("user://Mods/" + modInfo.mod_id + "/image/B" + modInfo.buff_id + ".jpg");
            Image res = null;
            if (FileAccess.FileExists(path))
                res = Image.LoadFromFile(path);
            else
            {
                path = ProjectSettings.GlobalizePath("res://Resources/Images/SkillFrame.png");
                res = Image.LoadFromFile(path);
            }
            if (res != null)
            {
                texture.Texture = ImageTexture.CreateFromImage(res);
            }
        }
        else
        {
            data = new();
            string path = ProjectSettings.GlobalizePath("res://Resources/Images/SkillFrame.png");
            Image res = Image.LoadFromFile(path);
            texture.Texture = ImageTexture.CreateFromImage(res);
        }
        data.BuffObj = this;
        MouseEntered += new(() =>
        {
            if (data != null)
            {
                StaticInstance.MainRoot.ShowRichHint(StaticUtils.MakeBBCodeString(
                    "Buff名：" + data.BuffShowname + ",\n" +
                    "Buff剩余计数：" + (data.IsInfinite ? "永久" : data.BuffCount) + ",\n" +
                    "Buff量：" + data.BuffValue + ",\n" +
                    "效果：" + data.Desc
                    , "left"));
            }
        });
        MouseExited += new(() =>
        {
            StaticInstance.MainRoot.HideRichHint();
        });
        Update();
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

    public void ReceiveEvent(string @event, dynamic datas)
    {
        data.ReceiveEvent(@event, datas);
    }

    ~BuffObject()
    {
        data = null;
    }
}
