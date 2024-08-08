using System;
using System.Collections.Generic;
using System.Globalization;

namespace CizaMirrorNetworkExtension
{
    public static class StringExtension
    {
        public static uint ToUint(this string str)
        {
            if (str == null || !UInt32.TryParse(str, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
                return uint.MaxValue;

            return value;
        }

        public static uint[] ToUint(this string[] strs)
        {
            var values = new List<uint>();
            if (strs == null)
                return values.ToArray();

            foreach (var str in strs)
                values.Add(str.ToUint());

            return values.ToArray();
        }
    }
}