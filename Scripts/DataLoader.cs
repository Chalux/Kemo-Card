using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace KemoCard.Scripts
{
    // 数据中心单例
    public class Datas
    {
        private static Datas _ins;

        public static Datas Ins
        {
            get
            {
                _ins ??= new Datas();
                return _ins;
            }
        }

        private HashSet<string> _modCache;
        private readonly HashSet<string> _errorModsSet = [];
        private readonly System.Collections.Generic.Dictionary<string, ModInfoStruct> _modPool = new();
        public readonly System.Collections.Generic.Dictionary<string, CardStruct> CardPool = new();
        public readonly System.Collections.Generic.Dictionary<string, EnemyStruct> EnemyPool = new();
        public readonly System.Collections.Generic.Dictionary<string, EquipStruct> EquipPool = new();
        public readonly System.Collections.Generic.Dictionary<string, BuffStruct> BuffPool = new();
        public readonly System.Collections.Generic.Dictionary<string, RoleStruct> RolePool = new();
        public readonly System.Collections.Generic.Dictionary<string, PresetStruct> PresetPool = new();
        public readonly System.Collections.Generic.Dictionary<string, MapStruct> MapPool = new();
        public readonly System.Collections.Generic.Dictionary<string, EventStruct> EventPool = new();

        private const string ModCachePath = "user://Saves/modCache.json";

        public void Init()
        {
            var cache = FileAccess.FileExists(ModCachePath);
            _modCache = [];
            if (cache)
            {
                using var cacheFile = FileAccess.Open(ModCachePath, FileAccess.ModeFlags.ReadWrite);
                try
                {
                    var content = cacheFile.GetAsText();
                    var modCacheArray = JsonSerializer.Deserialize<string[]>(content);
                    if (modCacheArray != null)
                    {
                        foreach (var str in modCacheArray)
                        {
                            _modCache.Add(str);
                        }
                    }
                }
                catch
                {
                    _modCache.Clear();
                    _modCache.Add("MainPackage");
                    cacheFile.StoreString("[\"MainPackage\"]");
                    StaticInstance.MainRoot.ShowBanner("模组配置缓存文件不存在或格式错误，已重置文件内容");
                }
            }
            else
            {
                DirAccess.MakeDirRecursiveAbsolute("user://Saves/");
                using var cachefile = FileAccess.Open(ModCachePath, FileAccess.ModeFlags.Write);
                if (cachefile == null)
                {
                    GD.Print(FileAccess.GetOpenError());
                    return;
                }

                const string json = "[\"MainPackage\"]";
                cachefile.StoreString(json);
                var modCacheArray = Json.ParseString(json);
                foreach (var str in modCacheArray.As<string[]>())
                {
                    _modCache.Add(str);
                }
            }

            LoadModsData();
        }

        private void LoadModsData()
        {
            _modPool.Clear();
            CardPool.Clear();
            _errorModsSet.Clear();
            foreach (var name in _modCache)
            {
                var isLoaded = ProjectSettings.LoadResourcePack($"user://Mods/{name}.pck");
                if (isLoaded || name == "MainPackage")
                {
                    var res = ResourceLoader.Load<CSharpScript>($"res://Mods/{name}/Scripts/ModBoost.cs");
                    if (res == null) continue;
                    var modBoost = res.New().As<BaseModBoost>();
                    modBoost.OnInit();
                    var path = $"res://Mods/{name}/mod_info.json";
                    if (FileAccess.FileExists(path))
                    {
                        using var f = FileAccess.Open(path, FileAccess.ModeFlags.Read);
                        var modInfo = Json.ParseString(f.GetAsText());
                        if ((modInfo as object) != null)
                        {
                            var jsonData = new Godot.Collections.Dictionary<string, Variant>((Dictionary)modInfo);
                            if (jsonData.TryGetValue("mod_id", out var modId))
                            {
                                var currModId = modId.AsString();
                                if (_modPool.TryGetValue(currModId, out var modInfoStruct))
                                {
                                    var errorLog = "Mod间存在冲突。Mod的id重复。冲突的两个Mod的id分别为：" + currModId + "," +
                                                   modInfoStruct.ModId;
                                    _errorModsSet.Add(currModId);
                                    _errorModsSet.Add(modInfoStruct.ModId);
                                    StaticInstance.MainRoot.ShowBanner(errorLog);
                                    continue;
                                }

                                _modPool.Add(currModId, new ModInfoStruct
                                {
                                    Name = jsonData["name"].AsString(),
                                    ModId = currModId,
                                    ModVersion = jsonData["mod_version"].AsString(),
                                    Description = jsonData["description"].AsString(),
                                    AuthorList = jsonData["author_list"].AsStringArray(),
                                });
                                if (jsonData.TryGetValue("card_list", out var cardList))
                                {
                                    foreach (var data in cardList.AsGodotArray<Dictionary>())
                                    {
                                        var id = data["card_id"].AsString();
                                        if (CardPool.TryGetValue(id, out var cardStruct))
                                        {
                                            StaticInstance.MainRoot.ShowBanner("Mod间存在冲突。卡牌id冲突：" + id
                                                + ",冲突的两个Mod的id分别为：" + currModId + "," + cardStruct.ModId);
                                            _errorModsSet.Add(currModId);
                                            _errorModsSet.Add(cardStruct.ModId);
                                            continue;
                                        }

                                        CardPool.Add(id, new CardStruct
                                        {
                                            CardId = id,
                                            FilterFlag = data["filter_flag"].AsUInt64(),
                                            Rare = data["rare"].AsUInt16(),
                                            IsSpecial = data["is_special"].AsBool(),
                                            Alias = data["alias"].AsString(),
                                            Desc = data["desc"].AsString(),
                                            Cost = data["cost"].AsUInt32(),
                                            CostType = data["cost_type"].AsUInt32(),
                                            TargetType = data["target_type"].AsUInt32(),
                                            ModId = currModId
                                        });
                                    }
                                }

                                if (jsonData.TryGetValue("enemy_list", out var variant))
                                {
                                    foreach (var data in variant.AsGodotArray<Dictionary>())
                                    {
                                        var id = data["enemy_id"].AsString();
                                        if (EnemyPool.TryGetValue(id, out var enemyStruct))
                                        {
                                            StaticInstance.MainRoot.ShowBanner("Mod间存在冲突。敌人id冲突：" + id
                                                + ",冲突的两个Mod的id分别为：" + currModId + "," + enemyStruct.ModId);
                                            _errorModsSet.Add(currModId);
                                            _errorModsSet.Add(enemyStruct.ModId);
                                            continue;
                                        }

                                        EnemyPool.Add(id, new EnemyStruct
                                        {
                                            EnemyId = id,
                                            ModId = currModId
                                        });
                                    }
                                }

                                if (jsonData.TryGetValue("equip_list", out var elValue))
                                {
                                    foreach (var data in elValue.AsGodotArray<Dictionary>())
                                    {
                                        var id = data["equip_id"].AsString();
                                        if (EquipPool.TryGetValue(id, out var value))
                                        {
                                            StaticInstance.MainRoot.ShowBanner("Mod间存在冲突。装备id冲突：" + id
                                                + ",冲突的两个Mod的id分别为：" + currModId + "," + value.ModId);
                                            _errorModsSet.Add(currModId);
                                            _errorModsSet.Add(value.ModId);
                                            continue;
                                        }

                                        EquipPool.Add(id, new EquipStruct
                                        {
                                            EquipId = id,
                                            EquipType = data["equip_type"].AsUInt16(),
                                            IsSpecial = data["is_special"].AsBool(),
                                            ModId = currModId
                                        });
                                    }
                                }

                                if (jsonData.TryGetValue("buff_list", out var buffValue))
                                {
                                    foreach (var data in buffValue.AsGodotArray<Dictionary>())
                                    {
                                        var id = data["buff_id"].AsString();
                                        if (BuffPool.TryGetValue(id, out var value))
                                        {
                                            StaticInstance.MainRoot.ShowBanner("Mod间存在冲突。buffId冲突：" + id +
                                                                               "，冲突的两个Mod的id分别为：" + currModId + "，" +
                                                                               value.ModId);
                                            _errorModsSet.Add(currModId);
                                            _errorModsSet.Add(value.ModId);
                                            continue;
                                        }

                                        BuffPool.Add(id, new BuffStruct
                                        {
                                            BuffId = id,
                                            IsDebuff = data["is_debuff"].AsBool(),
                                            // ReSharper disable once StringLiteralTypo
                                            ShowName = data["showname"].AsString(),
                                            Desc = data["desc"].AsString(),
                                            IconPath = data["icon_path"].AsString(),
                                            IsInfinite = data["is_infinite"].AsBool(),
                                            BuffCount = data["buff_count"].AsInt32(),
                                            BuffValue = data["buff_value"].AsDouble(),
                                            ModId = currModId
                                        });
                                    }
                                }

                                if (jsonData.TryGetValue("role_list", out var roleValue))
                                {
                                    foreach (var data in roleValue.AsGodotArray<Dictionary>())
                                    {
                                        var id = data["role_id"].AsString();
                                        if (RolePool.TryGetValue(id, out var value))
                                        {
                                            StaticInstance.MainRoot.ShowBanner("Mod间存在冲突。roleId冲突：" + id +
                                                                               "，冲突的两个Mod的id分别为：" + currModId + "，" +
                                                                               value.ModId);
                                            _errorModsSet.Add(currModId);
                                            _errorModsSet.Add(value.ModId);
                                            continue;
                                        }

                                        RolePool.Add(id, new RoleStruct
                                        {
                                            RoleId = id,
                                            ModId = currModId
                                        });
                                    }
                                }

                                if (jsonData.TryGetValue("preset_list", out var presetValue))
                                {
                                    foreach (var data in presetValue.AsGodotArray<Dictionary>())
                                    {
                                        var id = data["preset_id"].AsString();
                                        if (PresetPool.TryGetValue(id, out var value))
                                        {
                                            StaticInstance.MainRoot.ShowBanner("Mod间存在冲突。presetId冲突：" + id +
                                                                               "，冲突的两个Mod的id分别为：" + currModId + "，" +
                                                                               value.ModId);
                                            _errorModsSet.Add(currModId);
                                            _errorModsSet.Add(value.ModId);
                                            continue;
                                        }

                                        PresetPool.Add(id, new PresetStruct
                                        {
                                            PresetId = id,
                                            Tier = data["tier"].AsUInt32(),
                                            IsBoss = data["is_boss"].AsBool(),
                                            IsSpecial = data["is_special"].AsBool(),
                                            ModId = currModId
                                        });
                                    }
                                }

                                if (jsonData.TryGetValue("map_list", out var mapValue))
                                {
                                    foreach (var data in mapValue.AsGodotArray<Dictionary>())
                                    {
                                        var id = data["map_id"].AsString();
                                        if (MapPool.TryGetValue(id, out var value))
                                        {
                                            StaticInstance.MainRoot.ShowBanner("Mod间存在冲突。mapId冲突：" + id +
                                                                               "，冲突的两个Mod的id分别为：" + currModId + "，" +
                                                                               value.ModId);
                                            _errorModsSet.Add(currModId);
                                            _errorModsSet.Add(value.ModId);
                                            continue;
                                        }

                                        MapPool.Add(id, new MapStruct
                                        {
                                            MapId = id,
                                            MinTier = data["min_tier"].AsUInt32(),
                                            MaxTier = data["max_tier"].AsUInt32(),
                                            Floor = data["floor"].AsUInt32(),
                                            MapWidth = data["map_width"].AsUInt32(),
                                            Paths = data["paths"].AsUInt32(),
                                            ShowCond = data.ContainsKey("show_cond")
                                                ? data["show_cond"].AsGodotDictionary<string, Array<Variant>>()
                                                : new Godot.Collections.Dictionary<string, Array<Variant>>(),
                                            HealTimes = data["heal_times"].AsUInt32(),
                                            CanAbort = data["can_abort"].AsBool(),
                                            ModId = currModId
                                        });
                                    }
                                }

                                if (jsonData.TryGetValue("event_list", out var eventValue))
                                {
                                    foreach (var data in eventValue.AsGodotArray<Dictionary>())
                                    {
                                        var id = data["event_id"].AsString();
                                        if (EventPool.ContainsKey(id))
                                        {
                                            StaticInstance.MainRoot.ShowBanner("Mod间存在冲突。eventId冲突：" + id +
                                                                               "，冲突的两个Mod的id分别为：" + currModId + "，" +
                                                                               MapPool[id].ModId);
                                            _errorModsSet.Add(currModId);
                                            _errorModsSet.Add(MapPool[id].ModId);
                                            continue;
                                        }

                                        EventPool.Add(id, new EventStruct
                                        {
                                            EventId = id,
                                            IsSpecial = data["is_special"].AsBool(),
                                            ModId = currModId
                                        });
                                    }
                                }
                            }
                        }
                    }

                    modBoost.OnInitEnd();
                }
                else
                {
                    StaticInstance.MainRoot.ShowBanner($"Mod加载出错，Mod名为：{name}");
                }
            }

            foreach (var i in _modPool)
            {
                GD.Print(i.ToString());
            }
            //foreach (var i in CardPool)
            //{
            //    GD.Print(i.ToString());
            //}
            //foreach (var i in EnemyPool)
            //{
            //    GD.Print(i.ToString());
            //}
            //foreach (var i in EquipPool)
            //{
            //    GD.Print(i.ToString());
            //}
            //foreach (var i in BuffPool)
            //{
            //    GD.Print(i.ToString());
            //}
            //foreach (var i in RolePool)
            //{
            //    GD.Print(i.ToString());
            //}
            //foreach (var i in PresetPool)
            //{
            //    GD.Print(i.ToString());
            //}

            if (_errorModsSet.Count > 0) GD.PrintErr($"发现Mod冲突，冲突的ModId列表为{_errorModsSet}");
        }

        public struct CardStruct
        {
            public string CardId;
            public ulong FilterFlag;
            public uint Rare;
            public bool IsSpecial;
            public string Alias;
            public string Desc;
            public uint Cost;
            public uint TargetType;
            public uint CostType;
            public string ModId;
        }

        public struct ModInfoStruct
        {
            public string Name;
            public string ModId;
            public string ModVersion;
            public string Description;
            public string[] AuthorList;
        }

        public struct EnemyStruct
        {
            public string EnemyId;
            public string ModId;
        }

        public struct EquipStruct
        {
            public string EquipId;
            public uint EquipType;
            public bool IsSpecial;
            public string ModId;
        }

        public struct BuffStruct
        {
            public string BuffId;
            public bool IsDebuff;
            public string ShowName;
            public string Desc;
            public string IconPath;
            public bool IsInfinite;
            public int BuffCount;
            public double BuffValue;
            public string ModId;
        }

        public struct RoleStruct
        {
            public string RoleId;
            public string ModId;
        }

        public struct PresetStruct
        {
            public string PresetId;
            public uint Tier;
            public bool IsBoss;

            /// <summary>
            /// 特殊的预设不会进到随机池子里
            /// </summary>
            public bool IsSpecial;

            public string ModId;
        }

        public struct MapStruct
        {
            public string MapId;
            public uint MinTier;
            public uint MaxTier;
            public uint Floor;
            public uint MapWidth;
            public uint Paths;
            public Godot.Collections.Dictionary<string, Array<Variant>> ShowCond;
            public uint HealTimes;
            public bool CanAbort;
            public string ModId;
        }

        public struct EventStruct
        {
            public string EventId;

            /// <summary>
            /// 特殊的预设不会进到随机池子里
            /// </summary>
            public bool IsSpecial;

            public string ModId;
        }

        public static List<T> CreateTarInterface<T>(string dllPath)
        {
            List<T> rs = [];
            var dll = Assembly.LoadFrom(dllPath);
            rs.AddRange(dll.GetTypes().Select(item => dll.CreateInstance(item.Namespace + "." + item.Name))
                .Where(objType => objType != null && typeof(T).IsAssignableFrom(objType.GetType())).Cast<T>());

            return rs;
        }
    }

    public abstract class ModBoostImpl
    {
        public virtual void OnModLoaded()
        {
        }
    }
}