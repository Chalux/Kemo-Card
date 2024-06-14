using DialogueManagerRuntime;
using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;
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

        public static string MakeBBCodeString(string msg, string align = "center", int font_size = 36, string color = "#ffffff") => "[" + align + "][font_size=" + font_size + "][color=" + color + "]" + msg + "[/color][/font_size][/" + align + "]";

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
    }
}
