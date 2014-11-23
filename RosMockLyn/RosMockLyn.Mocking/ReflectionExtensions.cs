using System;
using System.Collections.Generic;
using System.Reflection;

namespace RosMockLyn.Mocking
{
    internal static class ReflectionExtensions
    {
        internal static bool IsAssignableTo<T>(this Type type)
        {
            return typeof(T).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        internal static bool IsAssignableFrom<T>(this Type type)
        {
            return type.GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo());
        }

        internal static void Apply<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items) action(item);
        }
    }
}