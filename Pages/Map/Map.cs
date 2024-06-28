using Godot;
using KemoCard.Pages;
using KemoCard.Scripts;
using KemoCard.Scripts.Map;
using StaticClass;
using System;
using System.Linq;

public partial class Map : BaseScene
{
    private const int SCROLL_SPEED = 75;
    private const string MapRoomPath = $"res://Pages/Map/MapRoom.tscn";
    private const string MapLinePath = $"res://Pages/Map/MapLine.tscn";

    [Export] Node2D Lines;
    [Export] Node2D Rooms;
    [Export] Node2D Visual;
    [Export] Camera2D Camera2D;
    [Export] Godot.Button HideBtn;

    private float CameraEdgeY { get; set; } = 0;
    private bool isDrag = false;

    public override void _Ready()
    {
        CameraEdgeY = StaticInstance.playerData.gsd.MapGenerator.Data.Y_DISTANCE * (StaticInstance.playerData.gsd.MapGenerator.Data.FLOORS - 1);

        HideBtn.Pressed += HideMap;
    }

    public override void _Input(InputEvent @event)
    {
        float Y = Camera2D.Position.Y;
        if (@event.IsActionPressed("scroll_up"))
        {
            Y -= SCROLL_SPEED;
            isDrag = false;
        }
        else if (@event.IsActionPressed("scroll_down"))
        {
            Y += SCROLL_SPEED;
            isDrag = false;
        }
        else if (@event.IsActionPressed("left_mouse"))
            isDrag = true;
        else if (@event.IsActionReleased("left_mouse"))
            isDrag = false;
        if (isDrag && @event is InputEventMouseMotion mm)
        {
            Y -= mm.Relative.Y;
        }
        Y = Math.Clamp(Y, -CameraEdgeY, 0);
        Camera2D.Position = new(Camera2D.Position.X, Y);
    }

    public void GenerateNewMap(MapData mapData)
    {
        StaticInstance.playerData.gsd.MapGenerator.FloorsClimbed = 0;
        StaticInstance.playerData.gsd.MapGenerator.GenerateMap(mapData);
        CreateMap();
    }

    public void CreateMap()
    {
        var map = StaticInstance.playerData.gsd.MapGenerator.Data;
        StaticInstance.playerData.gsd.MapGenerator.MapData.ForEach(CurrentFloor =>
        {
            CurrentFloor.ForEach(Room =>
            {
                if (Room.NextRooms.Count > 0) SpawnRoom(Room);
            });
        });

        int Middle = (int)Math.Floor(map.MAP_WIDTH * 0.5);
        SpawnRoom(StaticInstance.playerData.gsd.MapGenerator.MapData[map.FLOORS - 1][Middle]);
        float MapWidthPixels = map.X_DISTANCE * (map.MAP_WIDTH - 1);
        var s = GetViewportRect().Size;
        Visual.Position = new((s.X - MapWidthPixels) / 2, s.Y / 2);
    }

    private void SpawnRoom(Room room)
    {
        MapRoom NewRoom = ResourceLoader.Load<PackedScene>(MapRoomPath).Instantiate() as MapRoom;
        Rooms.AddChild(NewRoom);
        NewRoom.Room = room;
        NewRoom.SelectEventHandler += OnMapRoomSelected;
        ConnectLines(room);

        if (room.Selected && room.Row < StaticInstance.playerData.gsd.MapGenerator.FloorsClimbed)
        {
            NewRoom.ShowSelected();
        }
    }

    private void ConnectLines(Room room)
    {
        if (room.NextRooms.Count == 0) return;
        foreach (var NextRoom in room.NextRooms)
        {
            Line2D NewLine = ResourceLoader.Load<PackedScene>(MapLinePath).Instantiate() as Line2D;
            NewLine.AddPoint(new(room.X, room.Y));
            var r = StaticInstance.playerData.gsd.MapGenerator.MapData[NextRoom / 100][NextRoom % 100];
            NewLine.AddPoint(new(r.X, r.Y));
            Lines.AddChild(NewLine);
        }
    }

    private void OnMapRoomSelected(Room room)
    {
        foreach (var RoomNode in Rooms.GetChildren().Cast<MapRoom>())
        {
            if (RoomNode.Room.Row == room.Row)
            {
                RoomNode.Available = false;
            }
        }
        StaticInstance.playerData.gsd.MapGenerator.LastRoom = room;
        StaticInstance.playerData.gsd.MapGenerator.FloorsClimbed += 1;
        StaticInstance.eventMgr.Dispatch("MapExited", new dynamic[] { room });
    }

    public void UnlockFloor(int floor)
    {
        foreach (MapRoom RoomNode in Rooms.GetChildren().Cast<MapRoom>())
        {
            if (RoomNode.Room.Row == floor) RoomNode.Available = true;
        }
    }

    public void UnlockNextRooms()
    {
        foreach (MapRoom RoomNode in Rooms.GetChildren().Cast<MapRoom>())
        {
            if (StaticInstance.playerData.gsd.MapGenerator.LastRoom.NextRooms.Contains(RoomNode.Room.Row * 100 + RoomNode.Room.Col)) RoomNode.Available = true;
        }
    }

    public void ShowMap()
    {
        Show();
        Camera2D.Enabled = true;
    }

    public void HideMap()
    {
        Hide();
        Camera2D.Enabled = false;
    }
}
