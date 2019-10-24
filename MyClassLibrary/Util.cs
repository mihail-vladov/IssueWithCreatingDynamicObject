using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MyClassLibrary
{
    internal static class Util
    {
        internal static readonly object[] NoObjects = { };

        internal const BindingFlags AllMembers = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
        internal const BindingFlags Default = (BindingFlags)0;

        internal static PropertyInfo ResolveProperty(Type type, string name, bool ignoreCase, object[] indexArgs, bool hasInstance, object setterValue = null, bool getter = true)
        {
            var candidates = type.GetAllProperties().Where(prop => Util.StringEqual(prop.Name, name, ignoreCase)).ToArray();
            if (candidates.Length == 1)
                return candidates[0];

            if (!getter)
            {
                Array.Resize(ref indexArgs, indexArgs.Length + 1);
                indexArgs[indexArgs.Length - 1] = setterValue;
            }

            var propMethods = candidates
                .Select(prop => getter ? prop.GetGetMethod(true) : prop.GetSetMethod(true))
                .Where(m => m != null && CanCall(m, hasInstance))
                .ToArray();

            indexArgs = indexArgs ?? Util.NoObjects;
            object state;
            var foundGetter = Util.BindToMethod(Util.AllMembers, propMethods, ref indexArgs, null, null, null, out state);
            return candidates.First(prop => (getter ? prop.GetGetMethod(true) : prop.GetSetMethod(true)) == foundGetter);
        }

        internal static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
        {
            return type.GetInheritanceChain().SelectMany(t => t.GetProperties(AllMembers | BindingFlags.DeclaredOnly));
        }

        internal static IEnumerable<Type> GetInheritanceChain(this Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }

        internal static bool StringEqual(string a, string b, bool ignoreCase)
        {
            return String.Equals(a, b, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        internal static MethodBase BindToMethod(BindingFlags bindingAttr, MethodBase[] match, ref Object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] names, out Object state)
        {
            return Type.DefaultBinder.BindToMethod(bindingAttr, match, ref args, modifiers, culture, names, out state);
        }

        internal static bool CanCall(MethodBase method, bool hasInstance)
        {
            return method.IsStatic || hasInstance;
        }
    }
}
