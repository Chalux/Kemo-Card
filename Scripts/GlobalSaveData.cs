using System;
using System.Collections.Generic;
using KemoCard.Pages;
using MemoryPack;

namespace KemoCard.Scripts
{
    [MemoryPackable(GenerateType.NoGenerate)]
    [Serializable]
    public partial class GlobalSaveData
    {
        public Dictionary<string, double> DoubleData { get; set; } = new();
        public Dictionary<string, int> IntData { get; set; } = new();
        public Dictionary<string, bool> BoolData { get; set; } = new();
        public PlayerRole MajorRole { get; set; }
        public MapGeneration MapGenerator { get; set; } = new();
        public List<ShopStruct> CurrShopStructs = [];
    }
}