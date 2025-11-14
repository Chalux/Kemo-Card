using Godot;
using KemoCard.Scripts.Buffs;
using KemoCard.Scripts.Cards;
using KemoCard.Scripts.Equips;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KemoCard.Scripts
{
    public class PlayerData
    {
        public Image ScreenSnapshot;

        public GlobalSaveData Gsd = new();

        private readonly JsonSerializerOptions _options = new()
            { ReferenceHandler = ReferenceHandler.Preserve, IncludeFields = true };

        public void Save(uint index, bool isQuick = false)
        {
            if (index == 1 && !isQuick)
            {
                Pages.AlertView.PopupAlert("选择的是自动存档位，是否继续？");
            }

            var path = StaticUtils.GetSavePath(index);
            DirAccess.MakeDirRecursiveAbsolute(StaticUtils.GetSaveDirPath());
            using var sav = FileAccess.OpenEncryptedWithPass(path, FileAccess.ModeFlags.Write, "UNDERSTROKE");
            if (sav == null)
            {
                FileAccess.GetOpenError();
                return;
            }

            //gsd.MajorRole = gsd.MajorRole;
            var json = JsonSerializer.Serialize(Gsd, _options);
            //GD.Print(sav, json);
            sav.StoreString(json);
            var imgPath = StaticUtils.GetSaveImgPath(index);
            if (ScreenSnapshot == null)
            {
                var img = StaticInstance.MainRoot.GetViewport().GetTexture().GetImage();
                img.Resize(256, 135);
                ScreenSnapshot = img;
            }

            ScreenSnapshot.SaveJpg(imgPath);
        }

        public void Load(uint index)
        {
            var path = StaticUtils.GetSavePath(index);
            DirAccess.MakeDirRecursiveAbsolute(StaticUtils.GetSaveDirPath());
            if (!FileAccess.FileExists(path)) return;
            using var save = FileAccess.OpenEncryptedWithPass(path, FileAccess.ModeFlags.Read, "UNDERSTROKE");
            var jsonString = Encoding.UTF8.GetString(save.GetBuffer((long)save.GetLength()));
            if (jsonString.Equals(""))
            {
                StaticInstance.MainRoot.ShowBanner("存档损坏");
                return;
            }

            GlobalSaveData obj;
            if (OS.IsDebugBuild())
            {
                obj = JsonSerializer.Deserialize<GlobalSaveData>(jsonString, _options);
            }
            else
            {
                try
                {
                    obj = JsonSerializer.Deserialize<GlobalSaveData>(jsonString, _options);
                }
                catch
                {
                    StaticInstance.MainRoot.ShowBanner("存档损坏");
                    return;
                }
            }

            Gsd.MajorRole = obj.MajorRole;
            Gsd.MapGenerator = obj.MapGenerator;
            Gsd.DoubleData = obj.DoubleData;
            Gsd.IntData = obj.IntData;
            Gsd.BoolData = obj.BoolData;
            Gsd.CurrShopStructs = obj.CurrShopStructs;
            Gsd.MapGenerator.Data.ReloadPools();
            Gsd.MajorRole.Buffs.ForEach(buff =>
            {
                if (Datas.Ins.BuffPool.ContainsKey(buff.BuffId))
                {
                    var buffScript = BuffFactory.CreateBuff(buff.BuffId);
                    buffScript?.OnBuffInit(buff);
                }

                buff.Binder = Gsd.MajorRole;
                if (buff.CreatorId == Gsd.MajorRole.Id)
                {
                    buff.Creator = Gsd.MajorRole;
                }
            });
            Gsd.MajorRole.Deck.ForEach(LoadCardScript);
            Gsd.MajorRole.TempDeck.ForEach(LoadCardScript);
            foreach (var equip in Gsd.MajorRole.EquipList)
            {
                if (equip == null) continue;
                equip.Owner = Gsd.MajorRole;
                equip.EquipScript.Binder = equip;
                if (!Datas.Ins.EquipPool.ContainsKey(equip.Id)) continue;
                var bes = EquipFactory.CreateEquip(equip.Id);
                bes?.UpdateAction(equip.EquipScript);
            }

            foreach (var equip in Gsd.MajorRole.EquipDic.Values.Where(equip => equip != null))
            {
                equip.Owner = Gsd.MajorRole;
                equip.EquipScript.Binder = equip;
                if (!Datas.Ins.EquipPool.ContainsKey(equip.Id)) continue;
                var bes = EquipFactory.CreateEquip(equip.Id);
                bes?.UpdateAction(equip.EquipScript);
            }

            //GD.Print(MajorRole);
            if (Gsd.MajorRole == null) return;
            Gsd.MajorRole.BuildDeckIdxDic();
            var node = (Pages.MainScene)ResourceLoader.Load<PackedScene>("res://Pages/MainScene.tscn").Instantiate();
            StaticInstance.WindowMgr.ChangeScene(node);
            if (!StaticInstance.PlayerData.Gsd.MapGenerator.IsStillRunning) return;
            if (StaticInstance.PlayerData.Gsd.MapGenerator.FloorsClimbed == 0)
                node?.MapView?.UnlockFloor(0);
            else
                node?.MapView?.UnlockNextRooms();
        }

        private void LoadCardScript(Card card)
        {
            card.Owner = Gsd.MajorRole;
            if (!Datas.Ins.CardPool.TryGetValue(card.Id, out var value))
            {
                var errorLog = "未在脚本库中找到卡牌对应id，请检查Mod配置。id:" + card.Id;
                StaticInstance.MainRoot.ShowBanner(errorLog);
                throw new Exception(errorLog);
            }

            var script = CardFactory.CreateCard(card.Id);
            script?.OnCardScriptInit(card);
        }
    }
}