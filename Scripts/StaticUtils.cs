using DialogueManagerRuntime;
using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Buffs;
using System;
using System.Collections.Generic;
using System.Reflection;
using static StaticClass.StaticEnums;

namespace StaticClass
{
    static class StaticUtils
    {
        private static readonly Random _random = new();

        public static bool ContainProperty(this Node2D instance, string propertyName)
        {
            if (instance != null && !string.IsNullOrEmpty(propertyName))
            {
                PropertyInfo _findedPropertyInfo = instance.GetScript().GetType().GetProperty(propertyName);
                return _findedPropertyInfo != null;
            }
            return false;
        }

        public static void ShuffleArray<T>(List<T> array)
        {
            Random random = new();

            for (int i = array.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (array[j], array[i]) = (array[i], array[j]);
            }
        }

        public static int GetCountLimit(BuffDict buffDict)
        {
            return buffDict switch
            {
                BuffDict.None => 1,
                _ => 1,
            };
        }

        public static string MakeBBCodeString(string msg, string align = "center", int font_size = 36, string color = "#ffffff") => "[" + align + "][font_size=" + font_size + "][color=" + color + "]" + msg + "[/color][/font_size][/" + align + "]";
        public static string MakeColorString(string msg, string color = "#ffffff", int font_size = 36) => $"[color={color}][font_size={font_size}]{msg}[/font_size][/color]";

        public static string GetSavePath(uint index) => ProjectSettings.LocalizePath("user://Saves/s" + index + ".sav");
        public static string GetSaveImgPath(uint index) => ProjectSettings.LocalizePath("user://Saves/s" + index + ".jpg");
        public static string GetSaveDirPath() => ProjectSettings.LocalizePath("user://Saves/");
        public static string GetInternalImagePath => ProjectSettings.LocalizePath("res://Resources/Image/");

        public static void StartDialogue(string url)
        {
            StaticInstance.windowMgr.ChangeScene(ResourceLoader.Load<PackedScene>("res://Pages/DialogueScene.tscn").Instantiate(), new((scene) =>
            {
                StaticInstance.MainRoot.canPause = true;
                DialogueManager.ShowDialogueBalloon(ResourceLoader.Load(url), "Begin", new() { scene });
            }));
            StaticInstance.MainRoot.HideRichHint();
        }

        public static void CreateBuffAndAddToRole(string id, BaseRole role)
        {
            if (Datas.Ins.BuffPool.TryGetValue(id, out Datas.BuffStruct modInfo))
            {
                FileAccess script = FileAccess.Open($"res://Mods/{modInfo.mod_id}/Scripts/Buffs/B{modInfo.buff_id}.cs", FileAccess.ModeFlags.Read);
                if (script != null)
                {
                    BuffImplBase data = new();
                    var res = ResourceLoader.Load<CSharpScript>($"res://Mods/{modInfo.mod_id}/Scripts/Buffs/B{modInfo.buff_id}.cs");
                    if (res != null)
                    {
                        BaseBuffScript @base = res.New().As<BaseBuffScript>();
                        @base.OnBuffInit(data);
                    }
                    role.AddBuff(data);
                }
                else
                {
                    string hint = $"读取id为{id}的Buff时出错。";
                    GD.PrintErr(hint);
                    StaticInstance.MainRoot.ShowBanner(hint);
                }
            }
            else
            {
                string hint = $"Buff库中无id为{id}的Buff。";
                GD.PrintErr(hint);
                StaticInstance.MainRoot.ShowBanner(hint);
            }
        }

        public static void CreateEquipAndPutOn(string id, PlayerRole playerRole)
        {
            if (Datas.Ins.EquipPool.TryGetValue(id, out Datas.EquipStruct modInfo))
            {
                string path = $"res://Mods/{modInfo.mod_id}/Scripts/Equips/EQ{modInfo.equip_id}.cs";
                if (FileAccess.FileExists(path))
                {
                    Equip data = new(id);
                    //var res = ResourceLoader.Load<CSharpScript>(path);
                    //if (res != null)
                    //{
                    //    BaseEquipScript @base = res.New().As<BaseEquipScript>();
                    //    @base.OnEquipInit(data.EquipScript);
                    //}
                    playerRole.AddEquipToBag(data);
                    playerRole.PutOnEquip(0);
                }
            }
        }

        public static T CreateDeepCopy<T>(T original)
        {
            if (original == null)
            {
                return default(T);
            }

            Type type = original.GetType();
            object newObject = Activator.CreateInstance(type);

            foreach (FieldInfo fieldInfo in type.GetFields())
            {
                if (fieldInfo.IsStatic)
                {
                    continue;
                }

                object value = fieldInfo.GetValue(original);
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
            double u1 = 1.0 - _random.NextDouble(); // 随机数(0,1]
            double u2 = 1.0 - _random.NextDouble();
            double standardNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

            // 根据偏移量调整
            double randomValue = target + standardNormal * offset;

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

        public static void StartNewBattleByPreset(string PresetId)
        {
            var bs = (BattleScene)ResourceLoader.Load<PackedScene>("res://Pages/BattleScene.tscn").Instantiate();
            //MainScene ms = (MainScene)StaticInstance.windowMgr.GetSceneByName("MainScene");
            //ms?.MapView.HideMap();
            //StaticInstance.windowMgr.AddScene(bs);
            StaticInstance.windowMgr.ChangeScene(bs);
            bs.NewBattleByPreset(PresetId);
        }
    }
}
