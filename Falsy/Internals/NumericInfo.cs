using System;
using System.Collections.Generic;

namespace Falsy.NET.Internals
{
    static class NumericInfo
    {
        private static readonly HashSet<Type> NumericTypes = new HashSet<Type>
                                                             {
                                                                 typeof (Byte),
                                                                 typeof (SByte),
                                                                 typeof (UInt16),
                                                                 typeof (UInt32),
                                                                 typeof (UInt64),
                                                                 typeof (Int16),
                                                                 typeof (Int32),
                                                                 typeof (Int64),
                                                                 typeof (Decimal),
                                                                 typeof (Double),
                                                                 typeof (Single),
                                                                 //typeof (BigInteger),
                                                                 //typeof (Complex),
                                                             };

        public static bool IsNumeric<T>(T instance)
        {
            return Typed<T>.TypeIsNumeric;
        }

        public static bool IsNumeric(this Type type)
        {
            if (!type.IsValueType)
                return false;

            var numericTypes = NumericTypes;

            if (numericTypes.Contains(type))
                return true;

            var underlying = Nullable.GetUnderlyingType(type);
            if (underlying == null)
                return false;

            return numericTypes.Contains(underlying);
        }


        private static class Typed<T>
        {
            public static readonly bool TypeIsNumeric = NumericTypes.Contains(typeof (T));
        }
    }
}