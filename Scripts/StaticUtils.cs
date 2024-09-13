using Godot;
using KemoCard.Scripts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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

        public static DialogueScene StartDialogue(string url)
        {
            PackedScene res = ResourceLoader.Load("res://Pages/DialogueScene.tscn") as PackedScene;
            if (res == null) return null;
            DialogueScene ds = res.Instantiate<DialogueScene>();
            StaticInstance.windowMgr.AddScene(ds);
            ds.RunDialogue(url);
            StaticInstance.MainRoot.HideRichHint();
            return ds;
        }

        public static void CreateBuffAndAddToRole(string id, BaseRole role, object Creator)
        {
            if (Datas.Ins.BuffPool.TryGetValue(id, out Datas.BuffStruct modInfo))
            {
                BuffImplBase data = new(id)
                {
                    Creator = Creator
                };
                role.AddBuff(data);
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
                return default;
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

        public static void CloseEvent()
        {
            StaticInstance.eventMgr.Dispatch("close_eventscene");
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
            StaticInstance.playerData.Save(1, true);
        }

        public static List<string> GetRandomCardIdFromPool(int cardNum = 3)
        {
            List<string> res = new();
            var pool = StaticInstance.playerData.gsd.MapGenerator.Data.CardPool;
            if (pool.Count > 0)
            {
                Random r = new();
                string cid = "";
                int ErrorCount = 0;
                for (int i = 0; i < cardNum; i++)
                {
                    while ((cid == "" || res.IndexOf(cid) != -1) && ErrorCount < 1000)
                    {
                        cid = pool[r.Next(pool.Count)];
                    }
                    if (cid != "") res.Add(cid);
                }
            }
            return res;
        }

        public static BattleScene TryGetBattleScene()
        {
            if (!BattleStatic.isFighting) return null;
            BattleScene bs = StaticInstance.windowMgr.GetSceneByName("BattleScene") as BattleScene;
            return bs;
        }

        public static class TransExp<TIn, TOut>
        {
            private static readonly Func<TIn, TOut> cache = GetFunc();
            private static Func<TIn, TOut> GetFunc()
            {
                ParameterExpression parameterExpression = System.Linq.Expressions.Expression.Parameter(typeof(TIn), "p");
                List<MemberBinding> memberBindingList = new();

                foreach (var item in typeof(TOut).GetProperties())
                {
                    if (!item.CanWrite) continue;
                    MemberExpression property = System.Linq.Expressions.Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name));
                    MemberBinding memberBinding = System.Linq.Expressions.Expression.Bind(item, property);
                    memberBindingList.Add(memberBinding);
                }

                MemberInitExpression memberInitExpression = System.Linq.Expressions.Expression.MemberInit(System.Linq.Expressions.Expression.New(typeof(TOut)), memberBindingList.ToArray());
                Expression<Func<TIn, TOut>> lambda = System.Linq.Expressions.Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

                return lambda.Compile();
            }

            public static TOut Trans(TIn tIn)
            {
                return cache(tIn);
            }
        }

    }
}
