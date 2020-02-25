using System;
using System.Linq;
using System.Reflection;

namespace GhandiBot.Mixins
{
    public static class EnumMixins
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum @enum) where TAttribute : Attribute
        {
            return @enum.GetType().GetMember(@enum.ToString()).Single().GetCustomAttribute<TAttribute>();
        }
    }
}