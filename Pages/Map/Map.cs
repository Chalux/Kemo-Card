using Godot;
using KemoCard.Pages;
using KemoCard.Scripts;
using KemoCard.Scripts.Map;
using StaticClass;
using System;
using System.Linq;

public partial class Map : BaseScene, IEvent
{
    private const int SCROLL_SPEED = 75;
    private const string MapRoomPath = $"res://Pages/Map/MapRoom.tscn";
    private const string MapLinePath = $"res://Pages/Map/MapLine.tscn";

    [Export] Node2D Lines;
    [Export] Node2D Rooms;
    [Export] Node2D Visual;
    [Export] Camera2D Camera2D;
    [Export] Godot.Button HideBtn;
    [Export] Godot.Button DebugBtn;
    [Export] Godot.Button DebugBtn2;
    [Export] Godot.Button HealBtn;
    [Export] Label HealLabel;

    public float CameraEdgeY { get; set; } = 0;
    private bool isDrag = false;

    public override void _Ready()
    {
        CameraEdgeY = StaticInstance.playerData.gsd.MapGenerator.Data.Y_DISTANCE * (StaticInstance.playerData.gsd.MapGenerator.Data.FLOORS - 1);

        HideBtn.Pressed += HideMap;

        DebugBtn2.Visible = DebugBtn.Visible = OS.IsDebugBuild();
        DebugBtn.Pressed += UnlockNextRooms;
        DebugBtn2.Pressed += new(() => GD.Print(StaticInstance.playerData.gsd.MapGenerator.ToString()));
        HealBtn.Pressed += TryHeal;
        UpdateView();
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
        //GD.Print(Camera2D.Position, Y, -CameraEdgeY);
    }

    public void GenerateNewMap(MapData mapData)
    {
        StaticInstance.playerData.gsd.MapGenerator.FloorsClimbed = 0;
        StaticInstance.playerData.gsd.MapGenerator.GenerateMap(mapData, true);
        CreateMap();
        if (StaticInstance.windowMgr.GetSceneByName("MainScene") is MainScene ms)
        {
            ms.MapView.CameraEdgeY = StaticInstance.playerData.gsd.MapGenerator.Data.Y_DISTANCE * (StaticInstance.playerData.gsd.MapGenerator.Data.FLOORS - 1);
        }
        var major = StaticInstance.playerData.gsd.MajorRole;
        major.CurrHealth = major.CurrHpLimit;
        major.CurrMagic = major.CurrMpLimit;
        foreach (var card in major.GetDeck())
        {
            card.InRoundDict.Clear();
        }
    }

    public void CreateMap()
    {
        foreach (var child in Rooms.GetChildren())
        {
            Rooms.RemoveChild(child);
            child.QueueFree();
        }
        foreach (var child in Lines.GetChildren())
        {
            Lines.RemoveChild(child);
            child.QueueFree();
        }
        var map = StaticInstance.playerData.gsd.MapGenerator.Data;
        var mapdata = StaticInstance.playerData.gsd.MapGenerator.MapData;
        mapdata.ForEach(CurrentFloor =>
        {
            CurrentFloor.ForEach(Room =>
            {
                if (Room.NextRooms.Count > 0) SpawnRoom(Room);
            });
        });

        int Middle = (int)Math.Floor(map.MAP_WIDTH * 0.5);
        var row = (int)map.FLOORS - 1;
        if (mapdata.Count > row && mapdata[row].Count > Middle) SpawnRoom(mapdata[row][Middle]);
        float MapWidthPixels = map.X_DISTANCE * (map.MAP_WIDTH - 1);
        var s = GetViewportRect().Size;
        Visual.Position = new((s.X - MapWidthPixels) / 2, s.Y / 2);
        Camera2D.Enabled = false;
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
            if (RoomNode.Room.Row == room.Row || !StaticInstance.playerData.gsd.MapGenerator.IsStillRunning)
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
            if (RoomNode.Room.Row == floor && StaticInstance.playerData.gsd.MapGenerator.IsStillRunning)
                RoomNode.Available = true;
        }
    }

    public void UnlockNextRooms()
    {
        foreach (MapRoom RoomNode in Rooms.GetChildren().Cast<MapRoom>())
        {
            if (StaticInstance.playerData.gsd.MapGenerator.IsStillRunning && StaticInstance.playerData.gsd.MapGenerator.LastRoom.NextRooms.Contains(RoomNode.Room.Row * 100 + RoomNode.Room.Col)) RoomNode.Available = true;
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

    private void TryHeal()
    {
        var major = StaticInstance.playerData.gsd.MajorRole;
        AlertView.PopupAlert($"确定要休息吗？将回复生命上限一半的血量。\n当前血量：{major.CurrHealth}/{major.CurrHpLimit}", false, new(() => StaticInstance.playerData.gsd.MapGenerator.Data?.TryHeal()));
    }

    private void UpdateView()
    {
        HealLabel.Text = $"休息次数：{StaticInstance.playerData.gsd.MapGenerator.Data?.HealTimes ?? 0}";
    }

    public void ReceiveEvent(string @event, params object[] datas)
    {
        if (@event == "AfterHeal")
        {
            UpdateView();
        }
    }
}
