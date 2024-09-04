using System.Collections.Generic;

namespace KemoCard.Scripts
{
    public enum RoomType
    {
        None,
        Monster,
        Treasure,
        Shop,
        Event,
        Boss,
    }
    public class Room
    {
        public RoomType Type { get; set; } = RoomType.None;
        public int Row { get; set; }
        public int Col { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public List<int> NextRooms { get; set; } = new();
        public bool Selected { get; set; } = false;
        /// <summary>
        /// 只有Type是Monster的时候才有意义
        /// </summary>
        public string RoomPresetId { get; set; }
        /// <summary>
        /// 只有Type是Event的时候才有意义
        /// </summary>
        public string RoomEventId { get; set; }
        /// <summary>
        /// 只有Type是Equip的时候才有意义
        /// </summary>
        public string RoomEquipId { get; set; }

        public override string ToString()
        {
            return $"{Col}({Type.ToString()[0]}{RoomPresetId ?? RoomEventId ?? RoomEquipId})";
        }
    }
}
