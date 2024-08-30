using Godot;
using KemoCard.Scripts.Buffs;
using KemoCard.Scripts.Cards;
using StaticClass;
using System;
using System.Text;
using System.Text.Json;

namespace KemoCard.Scripts
{
    public class PlayerData
    {
        public Image screen_snapshot;

        public GlobalSaveData gsd = new();

        public void Save(uint index, bool isquick = false)
        {
            if (index == 1 && !isquick)
            {
                AlertView.PopupAlert("选择的是自动存档位，是否继续？", false, null, new(() => { return; }));
            }
            string path = StaticUtils.GetSavePath(index);
            DirAccess.MakeDirRecursiveAbsolute(StaticUtils.GetSaveDirPath());
            using var sav = FileAccess.OpenEncryptedWithPass(path, FileAccess.ModeFlags.Write, "UNDERSTROKE");
            if (sav == null)
            {
                FileAccess.GetOpenError();
                return;
            }
            //gsd.MajorRole = gsd.MajorRole;
            var json = JsonSerializer.Serialize(gsd);
            //GD.Print(sav, json);
            sav.StoreString(json);
            string imgPath = StaticUtils.GetSaveImgPath(index);
            if (screen_snapshot == null)
            {
                var img = StaticInstance.MainRoot.GetViewport().GetTexture().GetImage();
                img.Resize(256, 135);
                screen_snapshot = img;
            }
            screen_snapshot.SaveJpg(imgPath);
        }

        public void Load(uint index)
        {
            string path = StaticUtils.GetSavePath(index);
            DirAccess.MakeDirRecursiveAbsolute(StaticUtils.GetSaveDirPath());
            if (FileAccess.FileExists(path))
            {
                using var save = FileAccess.OpenEncryptedWithPass(path, FileAccess.ModeFlags.Read, "UNDERSTROKE");
                var jsonstring = Encoding.UTF8.GetString(save.GetBuffer((long)save.GetLength()));
                if (jsonstring.Equals(""))
                {
                    StaticInstance.MainRoot.ShowBanner("存档损坏");
                    return;
                }
                GlobalSaveData obj;
                try
                {
                    obj = JsonSerializer.Deserialize<GlobalSaveData>(jsonstring);
                }
                catch
                {
                    StaticInstance.MainRoot.ShowBanner("存档损坏");
                    return;
                }
                //obj = JsonSerializer.Deserialize<GlobalSaveData>(jsonstring);
                gsd.MajorRole = obj.MajorRole;
                gsd.MapGenerator = obj.MapGenerator;
                gsd.DoubleData = obj.DoubleData;
                gsd.IntData = obj.IntData;
                gsd.BoolData = obj.BoolData;
                gsd.CurrShopStructs = obj.CurrShopStructs;
                gsd.MapGenerator.Data.ReloadPools();
                gsd.MajorRole.Buffs.ForEach(buff =>
                {
                    if (Datas.Ins.BuffPool.ContainsKey(buff.BuffId))
                    {
                        var modinfo = Datas.Ins.BuffPool[buff.BuffId];
                        var res = ResourceLoader.Load<CSharpScript>($"res://Mods/{modinfo.mod_id}/Scripts/Buffs/B{buff.BuffId}.cs");
                        if (res != null)
                        {
                            BaseBuffScript script = res.New().As<BaseBuffScript>();
                            script.OnBuffInit(buff);
                        }
                    }
                    buff.Binder = gsd.MajorRole;
                });
                gsd.MajorRole.Deck.ForEach(LoadCardScript);
                gsd.MajorRole.TempDeck.ForEach(LoadCardScript);
                foreach (var equip in gsd.MajorRole.EquipList)
                {
                    if (equip != null)
                    {
                        equip.owner = gsd.MajorRole;
                        equip.EquipScript.Binder = equip;
                    }
                }
                foreach (var equip in gsd.MajorRole.EquipDic.Values)
                {
                    if (equip != null)
                    {
                        equip.owner = gsd.MajorRole;
                        equip.EquipScript.Binder = equip;
                    }
                }
                //GD.Print(MajorRole);
                if (gsd.MajorRole != null)
                {
                    gsd.MajorRole.BuildDeckIdxDic();
                    MainScene node = (MainScene)ResourceLoader.Load<PackedScene>("res://Pages/MainScene.tscn").Instantiate();
                    StaticInstance.windowMgr.ChangeScene(node);
                    if (StaticInstance.playerData.gsd.MapGenerator.IsStillRunning)
                    {
                        if (StaticInstance.playerData.gsd.MapGenerator.FloorsClimbed == 0)
                            node?.MapView.UnlockFloor(0);
                        else
                            node?.MapView.UnlockNextRooms();
                    }
                }
            }
        }

        private void LoadCardScript(Card card)
        {
            card.owner = gsd.MajorRole;
            if (!Datas.Ins.CardPool.ContainsKey(card.Id))
            {
                string errorLog = "未在脚本库中找到卡牌对应id，请检查Mod配置。id:" + card.Id;
                StaticInstance.MainRoot.ShowBanner(errorLog);
                throw new Exception(errorLog);
            }
            var cfg = Datas.Ins.CardPool[card.Id];
            string path = $"res://Mods/{cfg.mod_id}/Scripts/Cards/C{card.Id}.cs";
            var res = FileAccess.Open(path, FileAccess.ModeFlags.Read);
            if (res == null)
            {
                string errorLog = "未找到卡牌脚本资源,id:" + card.Id;
                StaticInstance.MainRoot.ShowBanner(errorLog);
                throw new Exception(errorLog);
            }
            else
            {
                var s = ResourceLoader.Load<CSharpScript>(path).New().As<BaseCardScript>();
                s.OnCardScriptInit(card);
            }
        }
    }
}
