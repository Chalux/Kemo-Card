using System.Collections.Generic;

namespace KemoCard.Scripts
{
    public class GlobalSaveData
    {
        public Dictionary<string, double> DoubleData { get; set; }
        public Dictionary<string, int> IntData { get; set; }
        public Dictionary<string, bool> BoolData { get; set; }
        public PlayerRole MajorRole { get; set; }
    }
}
