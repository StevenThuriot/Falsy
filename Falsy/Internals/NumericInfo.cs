#region License

//  
// Copyright 2015 Steven Thuriot
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 

#endregion

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