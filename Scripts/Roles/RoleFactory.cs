using System;
using System.Collections.Generic;
using Godot;

namespace KemoCard.Scripts.Roles;

public static class RoleFactory
{
    private static readonly Dictionary<string, Type> RoleDict = new();

    public static BaseRoleScript CreateRole(string roleId)
    {
        if (RoleDict.TryGetValue(roleId, out var value)) return (BaseRoleScript)Activator.CreateInstance(value);
        var errorLog = $"试图创建不存在的角色:{roleId}";
        GD.PrintErr(errorLog);
        StaticInstance.MainRoot.ShowBanner(errorLog);
        return null;
    }

    public static void RegisterRole(string roleId, Type roleType)
    {
        if (string.IsNullOrWhiteSpace(roleId))
        {
            GD.PrintErr($"试图注册空角色Id的角色:{roleType.Name}");
            return;
        }

        if (roleType == null)
        {
            GD.PrintErr($"试图注册空角色:{roleId}");
            return;
        }

        if (RoleDict.ContainsKey(roleId))
        {
            GD.PrintErr($"重复注册角色Id:{roleId}");
            return;
        }

        if (!typeof(BaseRoleScript).IsAssignableFrom(roleType))
        {
            GD.PrintErr($"试图注册非角色类:{roleType.Name}");
            return;
        }

        RoleDict.Add(roleId, roleType);
    }
}