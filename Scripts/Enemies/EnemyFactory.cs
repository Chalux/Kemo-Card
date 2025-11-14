using System;
using System.Collections.Generic;
using Godot;

namespace KemoCard.Scripts.Enemies;

public static class EnemyFactory
{
    private static readonly Dictionary<string, Type> EnemyTypeDict = new();

    public static BaseEnemyScript CreateEnemy(string enemyId)
    {
        if (EnemyTypeDict.TryGetValue(enemyId, out var value)) return (BaseEnemyScript)Activator.CreateInstance(value);
        var errorLog = $"试图创建不存在的敌人:{enemyId}";
        GD.PrintErr(errorLog);
        StaticInstance.MainRoot.ShowBanner(errorLog);
        return null;
    }

    public static void RegisterEnemy(string enemyId, Type enemyType)
    {
        if (string.IsNullOrWhiteSpace(enemyId))
        {
            GD.PrintErr($"试图注册空enemyId的敌人:{enemyType.Name}");
            return;
        }

        if (enemyType == null)
        {
            GD.PrintErr($"试图注册空enemyType的敌人:{enemyId}");
            return;
        }

        if (EnemyTypeDict.ContainsKey(enemyId))
        {
            GD.PrintErr($"试图注册重复的敌人:{enemyId}");
            return;
        }

        if (!typeof(BaseEnemyScript).IsAssignableFrom(enemyType))
        {
            GD.PrintErr($"试图注册非敌人类:{enemyType.Name}");
            return;
        }

        EnemyTypeDict.Add(enemyId, enemyType);
    }
}