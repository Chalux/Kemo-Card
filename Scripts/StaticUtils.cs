using DialogueManagerRuntime;
using Godot;
using KemoCard.Scripts;
using NLua;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using static StaticClass.StaticEnums;

namespace StaticClass
{
    static class StaticUtils
    {
        public static bool ContainProperty(this Node2D instance, string propertyName)
        {
            if (instance != null && !string.IsNullOrEmpty(propertyName))
            {
                PropertyInfo _findedPropertyInfo = instance.GetScript().GetType().GetProperty(propertyName);
                return (_findedPropertyInfo != null);
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

        public static BuffImplBase RegisteBuff(object Binder)
        {
            BuffImplBase buffImplBase = new();
            buffImplBase.Binder = Binder;
            return buffImplBase;
        }

        /// <summary>
        /// 获取一个临时的lua虚拟机，使用完后记得手动销毁掉
        /// </summary>
        /// <returns>临时的lua虚拟机，使用完后记得手动销毁掉</returns>
        public static Lua GetOneTempLua()
        {
            Lua tempLua = new();
            InitLuaEnv(tempLua);
            return tempLua;
        }

        public static void ExecuteInSandBox(string script)
        {
            Lua tempLua = new();
            InitLuaEnv(tempLua);
            tempLua.DoString(script);
        }

        public static string MakeBBCodeString(string msg, string align = "center", int font_size = 36, string color = "#ffffff") => "[" + align + "][font_size=" + font_size + "][color=" + color + "]" + msg + "[/color][/font_size][/" + align + "]";

        public static string GetSavePath(uint index) => ProjectSettings.LocalizePath("user://Saves/s" + index + ".sav");
        public static string GetSaveImgPath(uint index) => ProjectSettings.LocalizePath("user://Saves/s" + index + ".jpg");
        public static string GetSaveDirPath() => ProjectSettings.LocalizePath("user://Saves/");
        public static void InitLuaEnv(Lua lua)
        {
            lua.LoadCLRPackage();
            lua.State.Encoding = Encoding.UTF8;
            lua.DoString($"import('{typeof(StaticEnums).Assembly}','{typeof(StaticEnums).Namespace}')");
            lua.DoString($"import('{typeof(StaticUtils).Assembly}','{typeof(StaticUtils).Namespace}')");
            lua.DoString($"import('{typeof(StaticInstance).Assembly}','{typeof(StaticInstance).Namespace}')");
            lua.DoString($"import('{typeof(Console).Namespace}')");
            lua.DoString($"import('{typeof(DialogueManager).Assembly}','{typeof(DialogueManager).Namespace}')");
            lua["ShowBanner"] = new Action<string, bool>(StaticInstance.MainRoot.ShowBanner);
            lua.DoString($"import = function() end");
        }

        public static List<BaseRole> CreateBaseRoleList()
        {
            return new List<BaseRole>();
        }
    }
}
