using Godot;
using Godot.Collections;
using System;

namespace KemoCard.Scripts
{
    internal class CondMgr
    {
        private readonly System.Collections.Generic.Dictionary<string, Func<Array<Variant>, bool>> CondActions = new();

        public bool CheckCond(System.Collections.Generic.Dictionary<string, Array<Variant>> CondList)
        {
            if (CondList == null) return true;
            foreach (var Cond in CondList)
            {
                if (CondActions.ContainsKey(Cond.Key))
                {
                    if (!CondActions[Cond.Key].Invoke(Cond.Value))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool CheckCond(Dictionary<string, Array<Variant>> CondList)
        {
            if (CondList == null) return true;
            foreach (var Cond in CondList)
            {
                if (CondActions.ContainsKey(Cond.Key))
                {
                    if (!CondActions[Cond.Key].Invoke(Cond.Value))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public CondMgr()
        {
            CondActions.Add("Map", (Array<Variant> variants) =>
            {
                bool res = false;
                foreach (var id in variants)
                {
                    string key = $"Map{variants[0].AsString()}Passed";
                    if (StaticInstance.PlayerData.Gsd.IntData.ContainsKey(key) && StaticInstance.PlayerData.Gsd.IntData[key] > 0)
                    {
                        res = true;
                        break;
                    }
                }
                return res;
            });
            CondActions.Add("Level", (Array<Variant> variants) =>
            {
                return StaticInstance.PlayerData.Gsd.MajorRole.Level >= variants[0].AsInt32();
            });
        }

        private static CondMgr _ins;
        public static CondMgr Ins
        {
            get
            {
                _ins ??= new CondMgr();
                return _ins;
            }
        }
    }
}
