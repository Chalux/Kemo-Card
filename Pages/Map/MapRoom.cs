using System;
using System.Collections.Generic;
using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using KemoCard.Scripts.Events;

namespace KemoCard.Pages.Map;

public partial class MapRoom : Control
{
    [Export] private Sprite2D _sprite;
    [Export] private Line2D _line;
    [Export] private AnimationPlayer _animation;
    private bool _available;
    public Action<Room> SelectEventHandler;
    private bool _onlyRunHandlerWhenSelected;

    public bool Available
    {
        get => _available;
        set
        {
            _available = value;
            if (_available)
            {
                _animation.Play("highlight");
            }
            else if (!Room.Selected)
            {
                _animation.Play("RESET");
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
            Position = new Vector2(Room.X, Room.Y);
            Random r = new();
            _line.RotationDegrees = r.NextSingle() * 360;
            var iconStruct = StaticEnums.RoomIconPath[Room.Type];
            _sprite.Texture = ResourceLoader.Load<CompressedTexture2D>(iconStruct);
        }
    }

    public void ShowSelected()
    {
        _line.Modulate = Colors.White;
    }

    /// <summary>
    /// 这个函数在AnimationPlayer中的动画轨道调用
    /// </summary>
    public void OnMapRoomSelected()
    {
        SelectEventHandler.Invoke(Room);
        if (_onlyRunHandlerWhenSelected) return;
        var ms = StaticInstance.WindowMgr.GetSceneByName("MainScene") as MainScene;
        ms?.MapView.HideMap();
        // var data = StaticInstance.PlayerData.Gsd.MapGenerator.Data;
        switch (Room.Type)
        {
            case RoomType.Monster or RoomType.Boss:
            {
                if (Room.RoomPresetId == null)
                {
                    MapGeneration.SetMonsterRoom(Room, Room.Type == RoomType.Boss);
                }

                StaticUtils.StartNewBattleByPreset(Room.RoomPresetId);
                break;
            }
            case RoomType.Event:
            {
                if (Room.RoomEventId == null)
                {
                    MapGeneration.SetEventRoom(Room);
                }

                var res = ResourceLoader.Load<PackedScene>("res://Pages/EventScene.tscn");
                if (res != null)
                {
                    var eventScene = res.Instantiate<EventScene>();
                    EventScript e = new(Room.RoomEventId);
                    StaticInstance.WindowMgr.AddScene(eventScene, e);
                }

                break;
            }
            case RoomType.Treasure:
            {
                if (Room.RoomEquipId == null)
                {
                    MapGeneration.SetTreasureRoom(Room);
                }

                var res = ResourceLoader.Load<PackedScene>("res://Pages/RewardScene.tscn");
                if (res != null)
                {
                    RewardStruct r1 = new()
                    {
                        Type = RewardType.Equip,
                        Rewards = [Room.RoomEquipId]
                    };
                    // RewardStruct r2 = new()
                    // {
                    //     Type = RewardType.Card,
                    //     Rewards = StaticUtils.GetRandomCardIdFromPool()
                    // };
                    List<RewardStruct> datas = [r1];
                    var rs = res.Instantiate<RewardScene>();
                    StaticInstance.WindowMgr.AddScene(rs, datas);
                }

                break;
            }
            case RoomType.Shop:
            {
                var res = ResourceLoader.Load<PackedScene>("res://Pages/ShopScene.tscn");
                if (res != null)
                {
                    Random rand = new();
                    List<string> cardIds = [];
                    List<ShopStruct> shopStructs = [];
                    var errorCount = 0;
                    var pool = StaticInstance.PlayerData.Gsd.MapGenerator.Data.CardPool;
                    for (var i = 0; i < 10; i++)
                    {
                        var cid = "";
                        while ((cid == "" || (cid != "" && cardIds.IndexOf(cid) != -1)) && errorCount < 1000)
                        {
                            cid = pool[rand.Next(pool.Count)];
                            errorCount++;
                        }

                        if (cid == "") continue;
                        ShopStruct shopStruct = new()
                        {
                            Card = new Card(cid),
                            IsBought = false,
                        };
                        shopStruct.Price = rand.Next(50, 100) * (int)shopStruct.Card.Rare;
                        shopStructs.Add(shopStruct);
                        cardIds.Add(cid);
                    }

                    StaticInstance.PlayerData.Gsd.CurrShopStructs = shopStructs;
                    var ss = res.Instantiate<ShopScene>();
                    StaticInstance.WindowMgr.AddScene(ss);
                }

                break;
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
        if (!Available || !@event.IsActionReleased("left_mouse")) return;
        //GD.Print(GetGlobalMousePosition(), GetGlobalRect().HasPoint(GetGlobalMousePosition()));
        if (!GetGlobalRect().HasPoint(GetGlobalMousePosition())) return;
        Room.Selected = true;
        _animation.Play("select");
    }
}