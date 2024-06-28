using Godot;
using KemoCard.Scripts;
using StaticClass;
using System;
using System.Collections.Generic;

public partial class MapRoom : Area2D
{
    [Export] Sprite2D Sprite;
    [Export] Line2D Line;
    [Export] AnimationPlayer Animation;
    private bool _available = false;
    public Action<Room> SelectEventHandler;
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
    public void CollInput(Node viewport, InputEvent @event, int shape_idx)
    {
        if (!Available || !@event.IsActionPressed("left_mouse")) return;
        Room.Selected = true;
        Animation.Play("select");
    }

    /// <summary>
    /// 这个函数在AnimationPlayer中的动画轨道调用
    /// </summary>
    public void OnMapRoomSelected()
    {
        //EmitSignal(SignalName.Select, Room);
        SelectEventHandler.Invoke(Room);
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
}
