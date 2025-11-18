using System;
using System.Linq;
using Godot;
using Godot.Collections;

namespace KemoCard.Scripts
{
    internal class CondMgr
    {
        private readonly System.Collections.Generic.Dictionary<string, Func<Array<Variant>, bool>> _condActions = new();

        public bool CheckCond(System.Collections.Generic.Dictionary<string, Array<Variant>> condList)
        {
            return condList == null || condList.Where(cond => _condActions.ContainsKey(cond.Key))
                .All(cond => _condActions[cond.Key].Invoke(cond.Value));
        }

        public bool CheckCond(Dictionary<string, Array<Variant>> condList)
        {
            return condList == null || condList.Where(cond => _condActions.ContainsKey(cond.Key))
                .All(cond => _condActions[cond.Key].Invoke(cond.Value));
        }

        private CondMgr()
        {
            _condActions.Add("Map",
                variants =>
                {
                    return variants.Select(_ => $"Map{variants[0].AsString()}Passed").Any(key =>
                        StaticInstance.PlayerData.Gsd.IntData.ContainsKey(key) &&
                        StaticInstance.PlayerData.Gsd.IntData[key] > 0);
                });
            _condActions.Add("Level",
                variants => StaticInstance.PlayerData.Gsd.MajorRole.Level >= variants[0].AsInt32());
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