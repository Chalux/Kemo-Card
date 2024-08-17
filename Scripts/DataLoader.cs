using Godot;
using Godot.Collections;
using StaticClass;
using System.Collections.Generic;
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

        public HashSet<string> ModCache;
        public HashSet<string> errorModsSet = new();
        public System.Collections.Generic.Dictionary<string, ModInfoStruct> ModPool = new();
        public System.Collections.Generic.Dictionary<string, CardStruct> CardPool = new();
        public System.Collections.Generic.Dictionary<string, EnemyStruct> EnemyPool = new();
        public System.Collections.Generic.Dictionary<string, EquipStruct> EquipPool = new();
        public System.Collections.Generic.Dictionary<string, BuffStruct> BuffPool = new();
        public System.Collections.Generic.Dictionary<string, RoleStruct> RolePool = new();
        public System.Collections.Generic.Dictionary<string, PresetStruct> PresetPool = new();
        public System.Collections.Generic.Dictionary<string, MapStruct> MapPool = new();
        public System.Collections.Generic.Dictionary<string, EventStruct> EventPool = new();

        private static readonly string MOD_CACHE_PATH = "user://Saves/modCache.json";

        public void Init()
        {
            bool cache = FileAccess.FileExists(MOD_CACHE_PATH);
            ModCache = new();
            if (cache)
            {
                using var cachefile = FileAccess.Open(MOD_CACHE_PATH, FileAccess.ModeFlags.ReadWrite);
                try
                {
                    string content = cachefile.GetAsText();
                    var ModCacheArray = JsonSerializer.Deserialize<string[]>(content);
                    if (ModCacheArray != null)
                    {
                        foreach (string str in ModCacheArray)
                        {
                            ModCache.Add(str);
                        }
                    }
                }
                catch
                {
                    ModCache.Clear();
                    ModCache.Add("MainPackage");
                    cachefile.StoreString("[\"MainPackage\"]");
                    StaticInstance.MainRoot.ShowBanner("模组配置缓存文件不存在或格式错误，已重置文件内容");
                }
            }
            else
            {
                DirAccess.MakeDirRecursiveAbsolute("user://Saves/");
                var cachefile = FileAccess.Open(MOD_CACHE_PATH, FileAccess.ModeFlags.Write);
                if (cachefile == null)
                {
                    GD.Print(FileAccess.GetOpenError());
                    return;
                }
                string json = "[\"MainPackage\"]";
                cachefile.StoreString(json);
                var ModCacheArray = Json.ParseString(json);
                if ((ModCacheArray as object) != null)
                {
                    foreach (string str in ModCacheArray.As<string[]>())
                    {
                        ModCache.Add(str);
                    }
                }
                cachefile.Dispose();
            }
            LoadModsData();
        }

        public void LoadModsData()
        {
            ModPool.Clear();
            CardPool.Clear();
            errorModsSet.Clear();
            foreach (var name in ModCache)
            {
                var isLoaded = ProjectSettings.LoadResourcePack($"user://Mods/{name}.pck");
                if (isLoaded || name == "MainPackage")
                {
                    var res = ResourceLoader.Load<CSharpScript>($"res://Mods/{name}/Scripts/ModBoost.cs");
                    BaseModBoost modBoost = res.New().As<BaseModBoost>();
                    modBoost.OnInit();
                    string path = $"res://Mods/{name}/mod_info.json";
                    if (FileAccess.FileExists(path))
                    {
                        using var f = FileAccess.Open(path, FileAccess.ModeFlags.Read);
                        var mod_info = Json.ParseString(f.GetAsText());
                        if ((mod_info as object) != null)
                        {
                            var JsonData = new Godot.Collections.Dictionary<string, Variant>((Dictionary)mod_info);
                            if (JsonData.ContainsKey("mod_id"))
                            {
                                string currModId = JsonData["mod_id"].AsString();
                                if (ModPool.ContainsKey(currModId))
                                {
                                    string errorLog = "Mod间存在冲突。Mod的id重复。冲突的两个Mod的id分别为：" + currModId + "," + ModPool[currModId].mod_id;
                                    errorModsSet.Add(currModId);
                                    errorModsSet.Add(ModPool[currModId].mod_id);
                                    StaticInstance.MainRoot.ShowBanner(errorLog);
                                    continue;
                                }
                                ModPool.Add(currModId, new ModInfoStruct
                                {
                                    name = JsonData["name"].AsString(),
                                    mod_id = currModId,
                                    mod_version = JsonData["mod_version"].AsString(),
                                    description = JsonData["description"].AsString(),
                                    author_list = JsonData["author_list"].AsStringArray(),
                                });
                                if (JsonData.ContainsKey("card_list"))
                                {
                                    foreach (var data in JsonData["card_list"].AsGodotArray<Dictionary>())
                                    {
                                        var id = data["card_id"].AsString();
                                        if (CardPool.ContainsKey(id))
                                        {
                                            StaticInstance.MainRoot.ShowBanner("Mod间存在冲突。卡牌id冲突：" + id
                                                + ",冲突的两个Mod的id分别为：" + currModId + "," + CardPool[id].mod_id);
                                            errorModsSet.Add(currModId);
                                            errorModsSet.Add(CardPool[id].mod_id);
                                            continue;
                                        }
                                        CardPool.Add(id, new CardStruct
                                        {
                                            card_id = id,
                                            mod_id = currModId
                                        });
                                    }
                                }
                                if (JsonData.ContainsKey("enemy_list"))
                                {
                                    foreach (var data in JsonData["enemy_list"].AsGodotArray<Dictionary>())
                                    {
                                        var id = data["enemy_id"].AsString();
                                        if (EnemyPool.ContainsKey(id))
                                        {
                                            StaticInstance.MainRoot.ShowBanner("Mod间存在冲突。敌人id冲突：" + id
                                                + ",冲突的两个Mod的id分别为：" + currModId + "," + EnemyPool[id].mod_id);
                                            errorModsSet.Add(currModId);
                                            errorModsSet.Add(EnemyPool[id].mod_id);
                                            continue;
                                        }
                                        EnemyPool.Add(id, new EnemyStruct
                                        {
                                            enemy_id = id,
                                            mod_id = currModId
                                        });
                                    }
                                }
                                if (JsonData.ContainsKey("equip_list"))
                                {
                                    foreach (var data in JsonData["equip_list"].AsGodotArray<Dictionary>())
                                    {
                                        var id = data["equip_id"].AsString();
                                        if (EquipPool.ContainsKey(id))
                                        {
                                            StaticInstance.MainRoot.ShowBanner("Mod间存在冲突。装备id冲突：" + id
                                                + ",冲突的两个Mod的id分别为：" + currModId + "," + EquipPool[id].mod_id);
                                            errorModsSet.Add(currModId);
                                            errorModsSet.Add(EquipPool[id].mod_id);
                                            continue;
                                        }
                                        EquipPool.Add(id, new EquipStruct
                                        {
                                            equip_id = id,
                                            equip_type = data["equip_type"].AsUInt16(),
                                            mod_id = currModId
                                        });
                                    }
                                }
                                if (JsonData.ContainsKey("buff_list"))
                                {
                                    foreach (var data in JsonData["buff_list"].AsGodotArray<Dictionary>())
                                    {
                                        var id = data["buff_id"].AsString();
                                        if (BuffPool.ContainsKey(id))
                                        {
                                            StaticInstance.MainRoot.ShowBanner("Mod间存在冲突。buffid冲突：" + id + "，冲突的两个Mod的id分别为：" + currModId + "，" + BuffPool[id].mod_id);
                                            errorModsSet.Add(currModId);
                                            errorModsSet.Add(BuffPool[id].mod_id);
                                            continue;
                                        }
                                        BuffPool.Add(id, new BuffStruct
                                        {
                                            buff_id = id,
                                            mod_id = currModId
                                        });
                                    }
                                }
                                if (JsonData.ContainsKey("role_list"))
                                {
                                    foreach (var data in JsonData["role_list"].AsGodotArray<Dictionary>())
                                    {
                                        var id = data["role_id"].AsString();
                                        if (RolePool.ContainsKey(id))
                                        {
                                            StaticInstance.MainRoot.ShowBanner("Mod间存在冲突。roleid冲突：" + id + "，冲突的两个Mod的id分别为：" + currModId + "，" + RolePool[id].mod_id);
                                            errorModsSet.Add(currModId);
                                            errorModsSet.Add(RolePool[id].mod_id);
                                            continue;
                                        }
                                        RolePool.Add(id, new RoleStruct
                                        {
                                            role_id = id,
                                            mod_id = currModId
                                        });
                                    }
                                }
                                if (JsonData.ContainsKey("preset_list"))
                                {
                                    foreach (var data in JsonData["preset_list"].AsGodotArray<Dictionary>())
                                    {
                                        var id = data["preset_id"].AsString();
                                        if (PresetPool.ContainsKey(id))
                                        {
                                            StaticInstance.MainRoot.ShowBanner("Mod间存在冲突。presetid冲突：" + id + "，冲突的两个Mod的id分别为：" + currModId + "，" + PresetPool[id].mod_id);
                                            errorModsSet.Add(currModId);
                                            errorModsSet.Add(PresetPool[id].mod_id);
                                            continue;
                                        }
                                        PresetPool.Add(id, new PresetStruct
                                        {
                                            preset_id = id,
                                            tier = data["tier"].AsUInt32(),
                                            is_boss = data["is_boss"].AsBool(),
                                            mod_id = currModId
                                        });
                                    }
                                }
                                if (JsonData.ContainsKey("map_list"))
                                {
                                    foreach (var data in JsonData["map_list"].AsGodotArray<Dictionary>())
                                    {
                                        var id = data["map_id"].AsString();
                                        if (MapPool.ContainsKey(id))
                                        {
                                            StaticInstance.MainRoot.ShowBanner("Mod间存在冲突。mapid冲突：" + id + "，冲突的两个Mod的id分别为：" + currModId + "，" + MapPool[id].mod_id);
                                            errorModsSet.Add(currModId);
                                            errorModsSet.Add(MapPool[id].mod_id);
                                            continue;
                                        }
                                        MapPool.Add(id, new MapStruct
                                        {
                                            map_id = id,
                                            min_tier = data["min_tier"].AsUInt32(),
                                            max_tier = data["max_tier"].AsUInt32(),
                                            floor = data["floor"].AsUInt32(),
                                            map_width = data["map_width"].AsUInt32(),
                                            paths = data["paths"].AsUInt32(),
                                            show_cond = data.ContainsKey("show_cond") ? data["show_cond"].AsGodotDictionary<string, Array<Variant>>() : new(),
                                            mod_id = currModId
                                        });
                                    }
                                }
                                if (JsonData.ContainsKey("event_list"))
                                {
                                    foreach (var data in JsonData["event_list"].AsGodotArray<Dictionary>())
                                    {
                                        var id = data["event_list"].AsString();
                                        if (EventPool.ContainsKey(id))
                                        {
                                            StaticInstance.MainRoot.ShowBanner("Mod间存在冲突。eventid冲突：" + id + "，冲突的两个Mod的id分别为：" + currModId + "，" + MapPool[id].mod_id);
                                            errorModsSet.Add(currModId);
                                            errorModsSet.Add(MapPool[id].mod_id);
                                            continue;
                                        }
                                        EventPool.Add(id, new EventStruct
                                        {
                                            event_id = id,
                                            mod_id = currModId
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
            foreach (var i in ModPool)
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
        }

        public struct CardStruct
        {
            public string card_id;
            public ulong filter_flag;
            public string mod_id;
        }

        public struct ModInfoStruct
        {
            public string name;
            public string mod_id;
            public string mod_version;
            public string description;
            public string[] author_list;
        }

        public struct EnemyStruct
        {
            public string enemy_id;
            public string mod_id;
        }

        public struct EquipStruct
        {
            public string equip_id;
            public uint equip_type;
            public string mod_id;
        }

        public struct BuffStruct
        {
            public string buff_id;
            public string mod_id;
        }

        public struct RoleStruct
        {
            public string role_id;
            public string mod_id;
        }

        public struct PresetStruct
        {
            public string preset_id;
            public uint tier;
            public bool is_boss;
            public string mod_id;
        }

        public struct MapStruct
        {
            public string map_id;
            public uint min_tier;
            public uint max_tier;
            public uint floor;
            public uint map_width;
            public uint paths;
            public Godot.Collections.Dictionary<string, Array<Variant>> show_cond;
            public string mod_id;
        }

        public struct EventStruct
        {
            public string event_id;
            public string mod_id;
        }

        public static List<T> CreateTarInterface<T>(string dllpath)
        {
            List<T> rs = new();
            var dlllll = Assembly.LoadFrom(dllpath);
            foreach (var item in dlllll.GetTypes())
            {
                object objType = dlllll.CreateInstance(item.Namespace + "." + item.Name);
                if (typeof(T).IsAssignableFrom(objType.GetType()))
                    rs.Add((T)objType);
            }
            return rs;
        }
    }

    public abstract partial class ModBoostImpl
    {
        public virtual void OnModLoaded()
        {
        }
    }
}
