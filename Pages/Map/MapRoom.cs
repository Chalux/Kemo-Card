using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Events;
using StaticClass;
using System;
using System.Collections.Generic;

public partial class MapRoom : Control
{
    [Export] Sprite2D Sprite;
    [Export] Line2D Line;
    [Export] AnimationPlayer Animation;
    private bool _available = false;
    public Action<Room> SelectEventHandler;
    public bool onlyRunHandlerWhenSelected = false;
    public bool Available
    {
        get => _available;
        set
        {
            _available = value;
            if (_available)
            {
                Animation.Play("highlight");
            }
            else if (!Room.Selected)
            {
                Animation.Play("RESET");
            }
        }
    }
    private Room _room;
    public Room Room
    {
        get => _room;
        set
        {
            _room = value;
            Position = new(Room.X, Room.Y);
            Random r = new();
            Line.RotationDegrees = r.NextSingle() * 360;
            var IconStruct = StaticEnums.RoomIconPath[Room.Type];
            Sprite.Texture = ResourceLoader.Load<CompressedTexture2D>(IconStruct);
        }
    }
    public void ShowSelected()
    {
        Line.Modulate = Colors.White;
    }

    /// <summary>
    /// 这个函数在AnimationPlayer中的动画轨道调用
    /// </summary>
    public void OnMapRoomSelected()
    {
        SelectEventHandler.Invoke(Room);
        if (onlyRunHandlerWhenSelected) return;
        MainScene ms = StaticInstance.windowMgr.GetSceneByName("MainScene") as MainScene;
        ms?.MapView.HideMap();
        if (Room.Type == RoomType.Monster || Room.Type == RoomType.Boss)
        {
            StaticUtils.StartNewBattleByPreset(Room.RoomPresetId);
        }
        else if (Room.Type == RoomType.Event)
        {
            PackedScene res = ResourceLoader.Load<PackedScene>("res://Pages/EventScene.tscn");
            if (res != null)
            {
                EventScene eventScene = res.Instantiate<EventScene>();
                Event e = new(Room.RoomEventId);
                StaticInstance.windowMgr.AddScene(eventScene, e);
            }
        }
        else if (Room.Type == RoomType.Treasure)
        {
            PackedScene res = ResourceLoader.Load<PackedScene>("res://Pages/RewardScene.tscn");
            if (res != null)
            {
                RewardStruct r1 = new()
                {
                    type = RewardType.Equip,
                    rewards = new() { Room.RoomEquipId }
                };
                RewardStruct r2 = new()
                {
                    type = RewardType.Card,
                    rewards = StaticUtils.GetRandomCardIdFromPool()
                };
                List<RewardStruct> datas = new() { r1 };
                RewardScene rs = res.Instantiate<RewardScene>();
                StaticInstance.windowMgr.AddScene(rs, datas);
            }
        }
        else if (Room.Type == RoomType.Shop)
        {
            PackedScene res = ResourceLoader.Load<PackedScene>("res://Pages/ShopScene.tscn");
            if (res != null)
            {
                Random rand = new();
                List<string> cardIds = new();
                List<ShopStruct> shopStructs = new();
                int ErrorCount = 0;
                var pool = StaticInstance.playerData.gsd.MapGenerator.Data.CardPool;
                for (int i = 0; i < 10; i++)
                {
                    string cid = "";
                    while ((cid == "" || (cid != "" && cardIds.IndexOf(cid) != -1)) && ErrorCount < 1000)
                    {
                        cid = pool[rand.Next(pool.Count)];
                        ErrorCount++;
                    }
                    if (cid != "")
                    {
                        ShopStruct shopStruct = new()
                        {
                            card = new(cid),
                            isBuyed = false,
                        };
                        shopStruct.price = rand.Next(50, 100) * (int)shopStruct.card.Rare;
                        shopStructs.Add(shopStruct);
                    }
                }
                StaticInstance.playerData.gsd.CurrShopStructs = shopStructs;
                ShopScene ss = res.Instantiate<ShopScene>();
                StaticInstance.windowMgr.AddScene(ss);
            }
        }
    }

    //public override void _Ready()
    //{
    //    Room r = new()
    //    {
    //        Type = RoomType.Boss,
    //        Position = new(100, 100)
    //    };
    //    Room = r;

    //    GetTree().CreateTimer(3).Timeout += new(() => { Available = true; });
    //    Connect(SignalName.Select, Callable.From(OnMapRoomSelected));
    //}

    public override void _GuiInput(InputEvent @event)
    {
        if (Available && @event.IsActionReleased("left_mouse"))
        {
            //GD.Print(GetGlobalMousePosition(), GetGlobalRect().HasPoint(GetGlobalMousePosition()));
            if (GetGlobalRect().HasPoint(GetGlobalMousePosition()))
            {
                Room.Selected = true;
                Animation.Play("select");
            }
        }
    }

}
