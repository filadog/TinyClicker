using System;
using System.ComponentModel;

namespace TinyClicker.Core.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        var description = value.GetAttributeOfType<DescriptionAttribute>();
        return description == null ? string.Empty : description.Description;
    }

    private static T? GetAttributeOfType<T>(this Enum enumVal) where T : Attribute
    {
        var type = enumVal.GetType();
        var memInfo = type.GetMember(enumVal.ToString());
        var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
        return attributes.Length > 0 ? (T)attributes[0] : null;
    }
}
