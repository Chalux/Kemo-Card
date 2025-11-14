using System;
using System.Collections.Generic;
using Godot;

namespace KemoCard.Scripts.Cards;

public static class CardFactory
{
    private static readonly Dictionary<string, Type> CardTypeMap = new();

    public static BaseCardScript CreateCard(string cardId)
    {
        if (CardTypeMap.TryGetValue(cardId, out var value)) return (BaseCardScript)Activator.CreateInstance(value);
        var errorLog = $"试图创建不存在的卡牌:{cardId}";
        StaticInstance.MainRoot.ShowBanner(errorLog);
        GD.PrintErr(errorLog);
        return null;
    }

    public static void RegisterCard(string cardId, Type type)
    {
        if (string.IsNullOrWhiteSpace(cardId))
        {
            GD.PrintErr($"试图注册空卡牌Id的卡牌:{type.Name}");
            return;
        }

        if (CardTypeMap.ContainsKey(cardId))
        {
            GD.PrintErr($"试图注册已存在的卡牌:{cardId}");
            return;
        }

        if (type == null)
        {
            GD.PrintErr($"试图注册空类型卡牌:{cardId}");
            return;
        }

        if (type.IsSubclassOf(typeof(BaseCardScript)) || type == typeof(BaseCardScript))
        {
            CardTypeMap[cardId] = type;
        }
        else
        {
            GD.PrintErr($"试图注册非卡牌类:{type.Name}");
        }
    }
}