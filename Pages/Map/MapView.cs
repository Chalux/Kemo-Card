using System;
using System.Linq;
using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Map;

namespace KemoCard.Pages.Map;

public partial class MapView : BaseScene, IEvent
{
    private const int ScrollSpeed = 75;
    private const string MapRoomPath = "res://Pages/Map/MapRoom.tscn";
    private const string MapLinePath = "res://Pages/Map/MapLine.tscn";

    [Export] private Node2D _lines;
    [Export] private Node2D _rooms;
    [Export] private Node2D _visual;
    [Export] private Camera2D _camera2D;
    [Export] private Button _hideBtn;
    [Export] private Button _debugBtn;
    [Export] private Button _debugBtn2;
    [Export] private Button _healBtn;
    [Export] private Button _abortBtn;
    [Export] private Label _healLabel;

    private float CameraEdgeY { get; set; }
    private bool _isDrag;

    public override void _Ready()
    {
        CameraEdgeY = StaticInstance.PlayerData.Gsd.MapGenerator.Data.YDistance *
                      (StaticInstance.PlayerData.Gsd.MapGenerator.Data.Floors - 1);

        _hideBtn.Pressed += HideMap;

        _debugBtn2.Visible = _debugBtn.Visible = OS.IsDebugBuild();
        _debugBtn.Pressed += UnlockNextRooms;
        _debugBtn2.Pressed += OnDebugBtn2OnPressed;
        _healBtn.Pressed += TryHeal;
        _abortBtn.Pressed += AbortMap;
        UpdateView();
    }

    private static void OnDebugBtn2OnPressed()
    {
        GD.Print(StaticInstance.PlayerData.Gsd.MapGenerator.ToString());
    }

    public override void _Input(InputEvent @event)
    {
        var y = _camera2D.Position.Y;
        if (@event.IsActionPressed("scroll_up"))
        {
            y -= ScrollSpeed;
            _isDrag = false;
        }
        else if (@event.IsActionPressed("scroll_down"))
        {
            y += ScrollSpeed;
            _isDrag = false;
        }
        else if (@event.IsActionPressed("left_mouse"))
            _isDrag = true;
        else if (@event.IsActionReleased("left_mouse"))
            _isDrag = false;

        if (_isDrag && @event is InputEventMouseMotion mm)
        {
            y -= mm.Relative.Y;
        }

        y = Math.Clamp(y, -CameraEdgeY, 0);
        _camera2D.Position = new Vector2(_camera2D.Position.X, y);
        //GD.Print(Camera2D.Position, Y, -CameraEdgeY);
    }

    public void GenerateNewMap(MapData mapData)
    {
        StaticInstance.PlayerData.Gsd.MapGenerator.FloorsClimbed = 0;
        StaticInstance.PlayerData.Gsd.MapGenerator.GenerateMap(mapData, true);
        CreateMap();
        if (StaticInstance.WindowMgr.GetSceneByName("MainScene") is MainScene ms)
        {
            ms.MapView.CameraEdgeY = StaticInstance.PlayerData.Gsd.MapGenerator.Data.YDistance *
                                     (StaticInstance.PlayerData.Gsd.MapGenerator.Data.Floors - 1);
        }

        var major = StaticInstance.PlayerData.Gsd.MajorRole;
        major.CurrHealth = major.CurrHpLimit;
        major.CurrMagic = major.CurrMpLimit;
        foreach (var card in major.GetDeck())
        {
            card.InRoundDict.Clear();
        }
    }

    public void CreateMap()
    {
        foreach (var child in _rooms.GetChildren())
        {
            _rooms.RemoveChild(child);
            child.QueueFree();
        }

        foreach (var child in _lines.GetChildren())
        {
            _lines.RemoveChild(child);
            child.QueueFree();
        }

        var map = StaticInstance.PlayerData.Gsd.MapGenerator.Data;
        var mapData = StaticInstance.PlayerData.Gsd.MapGenerator.MapData;
        mapData.ForEach(currentFloor =>
        {
            currentFloor.ForEach(room =>
            {
                if (room.NextRooms.Count > 0) SpawnRoom(room);
            });
        });

        var middle = (int)Math.Floor(map.MapWidth * 0.5);
        var row = (int)map.Floors - 1;
        if (mapData.Count > row && mapData[row].Count > middle) SpawnRoom(mapData[row][middle]);
        float mapWidthPixels = map.XDistance * (map.MapWidth - 1);
        var s = GetViewportRect().Size;
        _visual.Position = new Vector2((s.X - mapWidthPixels) / 2, s.Y / 2);
        _camera2D.Enabled = false;
    }

    private void SpawnRoom(Room room)
    {
        var newRoom = ResourceLoader.Load<PackedScene>(MapRoomPath).Instantiate() as MapRoom;
        if (newRoom == null) return;
        _rooms.AddChild(newRoom);
        newRoom.Room = room;
        newRoom.SelectEventHandler += OnMapRoomSelected;
        ConnectLines(room);

        if (room.Selected && room.Row < StaticInstance.PlayerData.Gsd.MapGenerator.FloorsClimbed)
        {
            newRoom.ShowSelected();
        }
    }

    private void ConnectLines(Room room)
    {
        if (room.NextRooms.Count == 0) return;
        foreach (var nextRoom in room.NextRooms)
        {
            var newLine = ResourceLoader.Load<PackedScene>(MapLinePath).Instantiate() as Line2D;
            if (newLine == null) continue;
            newLine.AddPoint(new Vector2(room.X, room.Y));
            var r = StaticInstance.PlayerData.Gsd.MapGenerator.MapData[nextRoom / 100][nextRoom % 100];
            newLine.AddPoint(new Vector2(r.X, r.Y));
            _lines.AddChild(newLine);
        }
    }

    private void OnMapRoomSelected(Room room)
    {
        foreach (var roomNode in _rooms.GetChildren().Cast<MapRoom>())
        {
            if (roomNode.Room.Row == room.Row || !StaticInstance.PlayerData.Gsd.MapGenerator.IsStillRunning)
            {
                roomNode.Available = false;
            }
        }

        StaticInstance.PlayerData.Gsd.MapGenerator.LastRoom = room;
        StaticInstance.PlayerData.Gsd.MapGenerator.FloorsClimbed += 1;
        StaticInstance.EventMgr.Dispatch("MapExited", room);
    }

    public void UnlockFloor(int floor)
    {
        foreach (var roomNode in _rooms.GetChildren().Cast<MapRoom>())
        {
            if (roomNode.Room.Row == floor && StaticInstance.PlayerData.Gsd.MapGenerator.IsStillRunning)
                roomNode.Available = true;
        }
    }

    public void UnlockNextRooms()
    {
        foreach (var roomNode in _rooms.GetChildren().Cast<MapRoom>())
        {
            if (StaticInstance.PlayerData.Gsd.MapGenerator.IsStillRunning &&
                StaticInstance.PlayerData.Gsd.MapGenerator.LastRoom.NextRooms.Contains(roomNode.Room.Row * 100 +
                    roomNode.Room.Col)) roomNode.Available = true;
        }
    }

    public void ShowMap()
    {
        Show();
        UpdateView();
        _camera2D.Enabled = true;
    }

    public void HideMap()
    {
        Hide();
        _camera2D.Enabled = false;
    }

    private void TryHeal()
    {
        if (!StaticInstance.PlayerData.Gsd.MapGenerator.IsStillRunning) return;
        ResetCamera();
        var major = StaticInstance.PlayerData.Gsd.MajorRole;
        AlertView.PopupAlert($"确定要休息吗？将回复生命上限一半的血量。\n当前血量：{major.CurrHealth}/{major.CurrHpLimit}", false,
            () => StaticInstance.PlayerData.Gsd.MapGenerator.Data?.TryHeal());
    }

    private void UpdateView()
    {
        var mapGeneration = StaticInstance.PlayerData.Gsd.MapGenerator;
        _healLabel.Text = $"休息次数：{mapGeneration.Data?.HealTimes ?? 0}";
        _abortBtn.Disabled = mapGeneration.Data is not { CanAbort: true };
    }

    public void ReceiveEvent(string @event, params object[] datas)
    {
        if (@event == "AfterHeal")
        {
            UpdateView();
        }
    }

    private void AbortMap()
    {
        AlertView.PopupAlert("是否放弃此次地图？", false, () =>
        {
            StaticInstance.PlayerData.Gsd.MapGenerator.EndMap();
            HideMap();
        });
    }

    private void ResetCamera()
    {
        _camera2D.Position = new Vector2(_camera2D.Position.X, -CameraEdgeY);
    }
}