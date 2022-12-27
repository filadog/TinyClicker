using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;

namespace TinyClickerLib.Extensions;

public static class EnumExtensions
{
    private static readonly ConcurrentDictionary<string, string> _displayNameCache = new();

    public static string GetName(this Enum value)
    {
        var key = $"{value.GetType().FullName}.{value}";

        var displayName = _displayNameCache.GetOrAdd(key, x =>
        {
            var name = (DescriptionAttribute[])value
                .GetType()
                .GetTypeInfo()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false);

            return name.Length > 0 ? name[0].Description : value.ToString();
        });

        return displayName;
    }
}
