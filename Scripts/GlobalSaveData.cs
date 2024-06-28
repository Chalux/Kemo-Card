using System.Collections.Generic;

namespace KemoCard.Scripts
{
    public class GlobalSaveData
    {
        public Dictionary<string, double> DoubleData { get; set; } = new();
        public Dictionary<string, int> IntData { get; set; } = new();
        public Dictionary<string, bool> BoolData { get; set; } = new();
        public PlayerRole MajorRole { get; set; }
        public MapGeneration MapGenerator { get; set; } = new();
    }
}
