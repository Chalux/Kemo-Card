using System;
using System.Collections.Generic;
using Godot;
using KemoCard.Pages;
using KemoCard.Scripts.Equips;
using static KemoCard.Scripts.StaticEnums;

namespace KemoCard.Scripts
{
    public static class StaticUtils
    {
        private static readonly Random Random = new();

        public static bool ContainProperty(this Node2D instance, string propertyName)
        {
            if (instance == null || string.IsNullOrEmpty(propertyName)) return false;
            var findPropertyInfo = instance.GetScript().GetType().GetProperty(propertyName);
            return findPropertyInfo != null;
        }

        public static void ShuffleArray<T>(List<T> array)
        {
            Random random = new();

            for (var i = array.Count - 1; i > 0; i--)
            {
                var j = random.Next(i + 1);
                (array[j], array[i]) = (array[i], array[j]);
            }
        }

        public static int GetCountLimit(BuffDict buffDict)
        {
            return buffDict switch
            {
                _ => 1,
            };
        }

        // ReSharper disable once InconsistentNaming
        public static string MakeBBCodeString(string msg, string align = "center", int fontSize = 36,
            string color = "#ffffff") => "[" + align + "][font_size=" + fontSize + "][color=" + color + "]" + msg +
                                         "[/color][/font_size][/" + align + "]";

        public static string MakeColorString(string msg, string color = "#ffffff", int fontSize = 36) =>
            $"[color={color}][font_size={fontSize}]{msg}[/font_size][/color]";

        public static string GetSavePath(uint index) => ProjectSettings.LocalizePath("user://Saves/s" + index + ".sav");

        public static string GetSaveImgPath(uint index) =>
            ProjectSettings.LocalizePath("user://Saves/s" + index + ".jpg");

        public static string GetSaveDirPath() => ProjectSettings.LocalizePath("user://Saves/");
        public static string GetInternalImagePath => ProjectSettings.LocalizePath("res://Resources/Image/");

        public static DialogueScene StartDialogue(string url)
        {
            var res = ResourceLoader.Load<PackedScene>("res://Pages/DialogueScene.tscn");
            if (res == null) return null;
            var ds = res.Instantiate<DialogueScene>();
            StaticInstance.WindowMgr.AddScene(ds);
            ds.RunDialogue(url);
            StaticInstance.MainRoot.HideRichHint();
            return ds;
        }

        public static void CreateBuffAndAddToRole(string id, BaseRole role, object creator)
        {
            if (Datas.Ins.BuffPool.ContainsKey(id))
            {
                BuffImplBase data = new(id)
                {
                    Creator = creator
                };
                role.AddBuff(data);
            }
            else
            {
                var hint = $"Buff库中无id为{id}的Buff。";
                GD.PrintErr(hint);
                StaticInstance.MainRoot.ShowBanner(hint);
            }
        }

        public static void CreateEquipAndPutOn(string id, PlayerRole playerRole)
        {
            if (!Datas.Ins.EquipPool.ContainsKey(id)) return;
            var script = EquipFactory.CreateEquip(id);
            if (script == null) return;
            Equip data = new(id);
            script.OnEquipInit(data.EquipScript);
            playerRole.AddEquipToBag(data);
            playerRole.PutOnEquip(0);
        }

        public static T CreateDeepCopy<T>(T original)
        {
            if (original == null)
            {
                return default;
            }

            var type = original.GetType();
            var newObject = Activator.CreateInstance(type);

            foreach (var fieldInfo in type.GetFields())
            {
                if (fieldInfo.IsStatic)
                {
                    continue;
                }

                var value = fieldInfo.GetValue(original);
                fieldInfo.SetValue(newObject, CreateDeepCopy(value));
            }

            return (T)newObject;
        }

        /// <summary>  
        /// 根据正态分布概率随机取一个在目标点附近的偏移量为给定参数的一个值  
        /// </summary>  
        /// <param name="minRange">最小范围</param>  
        /// <param name="maxRange">最大范围</param>  
        /// <param name="target">目标点</param>  
        /// <param name="offset">偏移量</param>  
        /// <returns>一个在目标点附近的随机值</returns>  
        public static double GenerateRandomValue(double minRange, double maxRange, double target, double offset)
        {
            // 使用Box-Muller变换生成标准正态分布的随机数
            var u1 = 1.0 - Random.NextDouble(); // 随机数(0,1]
            var u2 = 1.0 - Random.NextDouble();
            var standardNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

            // 根据偏移量调整
            var randomValue = target + standardNormal * offset;

            // 约束在最小范围和最大范围之间
            if (randomValue < minRange)
            {
                randomValue = minRange;
            }
            else if (randomValue > maxRange)
            {
                randomValue = maxRange;
            }

            return randomValue;
        }

        public static void StartNewBattleByPreset(string presetId)
        {
            var bs = (BattleScene)ResourceLoader.Load<PackedScene>("res://Pages/BattleScene.tscn").Instantiate();
            //MainScene ms = (MainScene)StaticInstance.windowMgr.GetSceneByName("MainScene");
            //ms?.MapView.HideMap();
            //StaticInstance.windowMgr.AddScene(bs);
            StaticInstance.WindowMgr.ChangeScene(bs);
            bs.NewBattleByPreset(presetId);
        }

        public static void CloseEvent()
        {
            StaticInstance.EventMgr.Dispatch("close_eventscene");
            var ms = StaticInstance.WindowMgr.GetSceneByName("MainScene") as MainScene;
            ms?.MapView?.UnlockNextRooms();
        }

        public static string GetFrameColorByRare(uint rare)
        {
            return rare switch
            {
                1 => Colors.White.ToHtml(),
                2 => Colors.Green.ToHtml(),
                3 => Colors.Blue.ToHtml(),
                4 => Colors.Purple.ToHtml(),
                5 => Colors.Orange.ToHtml(),
                _ => Colors.White.ToHtml(),
            };
        }

        public static void AutoSave()
        {
            StaticInstance.PlayerData.Save(1, true);
        }

        public static List<string> GetRandomCardIdFromPool(int cardNum = 3)
        {
            List<string> res = [];
            var pool = StaticInstance.PlayerData.Gsd.MapGenerator.Data.CardPool;
            if (pool.Count <= 0) return res;
            Random r = new();
            var cid = "";
            var errorCount = 0;
            for (var i = 0; i < cardNum; i++)
            {
                while ((cid == "" || res.IndexOf(cid) != -1) && errorCount < 1000)
                {
                    cid = pool[r.Next(pool.Count)];
                    errorCount++;
                }

                if (cid != "") res.Add(cid);
            }

            return res;
        }

        public static BattleScene TryGetBattleScene()
        {
            if (!BattleStatic.IsFighting) return null;
            var bs = StaticInstance.WindowMgr.GetSceneByName("BattleScene") as BattleScene;
            return bs;
        }
    }
}