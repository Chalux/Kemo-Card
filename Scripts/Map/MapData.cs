using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KemoCard.Scripts.Map
{
    public class MapData
    {
        /// <summary>
        /// x轴两个点的间距
        /// </summary>
        public int X_DISTANCE { get; set; } = 180;
        /// <summary>
        /// y轴两个点的间距
        /// </summary>
        public int Y_DISTANCE { get; set; } = 150;
        /// <summary>
        /// 每个点随机偏移多少像素
        /// </summary>
        public int PLACEMENT_RANDOMNESS { get; set; } = 25;
        /// <summary>
        /// 一共多少层
        /// </summary>
        public int FLOORS { get; set; } = 15;
        /// <summary>
        /// 一层有多少格
        /// </summary>
        public int MAP_WIDTH { get; set; } = 7;
        /// <summary>
        /// 代码会生成多少条路径
        /// </summary>
        public int PATHS { get; set; } = 6;
        /// <summary>
        /// 生成怪物房间的权重
        /// </summary>
        public double MONSTER_ROOM_WEIGHT { get; set; } = 10.0;
        /// <summary>
        /// 生成商店房间的权重
        /// </summary>
        public double SHOP_ROOM_WEIGHT { get; set; } = 2.5;
        /// <summary>
        /// 生成事件房间的权重
        /// </summary>
        public double EVENT_ROOM_WEIGHT { get; set; } = 4.0;
        public List<uint> PresetPool { get; set; } = new();
        public uint MinTier { get; set; } = 1;
        public uint MaxTier { get; set; } = 1;
    }
}
