﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public static class EnumHelpers
    {
        private const string MustBeAnEnumeratedType = @"T must be an enumerated type";

        public static T ToEnum<T>(this string s) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(MustBeAnEnumeratedType);
            T @enum;
            Enum.TryParse(s, out @enum);
            return @enum;
        }

        public static T ToEnum<T>(this int i) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(MustBeAnEnumeratedType);
            return (T)Enum.ToObject(typeof(T), i);
        }

        public static T[] ToEnum<T>(this int[] value) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(MustBeAnEnumeratedType);
            var result = new T[value.Length];
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i].ToEnum<T>();
            return result;
        }

        public static T[] ToEnum<T>(this string[] value) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(MustBeAnEnumeratedType);
            var result = new T[value.Length];
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i].ToEnum<T>();
            return result;
        }

        public static IEnumerable<T> ToEnumFlags<T>(this int i) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(MustBeAnEnumeratedType);
            return
                (from flagIterator in Enum.GetValues(typeof(T)).Cast<int>()
                 where (i & flagIterator) != 0
                 select ToEnum<T>(flagIterator));
        }

        public static bool CheckFlag<T>(this Enum value, T flag) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(MustBeAnEnumeratedType);
            return (Convert.ToInt32(value, CultureInfo.InvariantCulture) & Convert.ToInt32(flag, CultureInfo.InvariantCulture)) != 0;
        }

        public static IDictionary<string, T> EnumToDictionary<T>() where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(MustBeAnEnumeratedType);
            IDictionary<string, T> list = new Dictionary<string, T>();
            Enum.GetNames(typeof(T)).ToList().ForEach(name => list.Add(name, name.ToEnum<T>()));
            return list;
        }
        public class Utils
        {
        }
    }
}
