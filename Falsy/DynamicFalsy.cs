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
using System.Dynamic;

namespace Falsy.NET
{
    public abstract class DynamicFalsy : DynamicObject
    {
        protected bool Equals(bool other)
        {
            var isFalsyEquivalent = IsFalsyEquivalent();
            return other ? !isFalsyEquivalent : isFalsyEquivalent;
        }

        public sealed override bool Equals(object arg)
        {
            if (ReferenceEquals(null, arg))
                return IsFalsyNull();

            if (arg is bool)
                return Equals((bool)arg);

            var falsy = arg as DynamicFalsy;
            if (!ReferenceEquals(null, falsy))
                return Equals(falsy);

            if (IsFalsyNull())
                //The falsy values null and undefined are not equivalent to anything except themselves
                return FalsyNull(arg);

            if (IsFalsyEquivalent())
                //The falsy values false, 0 (zero), and "" (empty string) are all equivalent and can be compared against each other
                return FalsyEquivalent(arg);

            if (IsFalsyNaN())
                //Finally, the falsy value NaN is not equivalent to anything — including NaN!
                return false;

            if (FalsyNull(arg) || FalsyNaN(arg))
                return false;

            return InternalEquals(arg);
        }


        public sealed override int GetHashCode()
        {
            return 1; //Make sure falsies clash.
        }









        public static bool FalsyEquivalent(object arg)
        {
            if (ReferenceEquals(null, arg))
                return false;

            if (Equals(false, arg) || Equals("", arg) )
                return true;

            dynamic argument = arg;
            bool isNumeric = NumericInfo.IsNumeric(argument);

            if (!isNumeric) return false;

            var result = (double) Math.Abs(argument) < double.Epsilon;
            return result;
        }

        public static bool FalsyNull(object arg)
        {
            return ReferenceEquals(null, arg) /*|| Equals(undefined, arg)*/;
        }

        public static bool FalsyNaN(object arg)
        {
            return Equals(float.NaN, arg) || Equals(double.NaN, arg);
        }
        
        public static bool operator ==(DynamicFalsy falsy1, DynamicFalsy falsy2)
        {
            if (ReferenceEquals(null, falsy1))
                return ReferenceEquals(null, falsy2) || falsy2.Equals(null);

            if (ReferenceEquals(null, falsy2))
                return falsy1.Equals(null);

            return Equals(falsy1, falsy2);
        }
        public static bool operator !=(DynamicFalsy falsy1, DynamicFalsy falsy2)
        {
            return !(falsy1 == falsy2);
        }

        public static bool operator ==(object value, DynamicFalsy falsy)
        {
            return Equals(falsy, value);
        }
        public static bool operator !=(object value, DynamicFalsy falsy)
        {
            return !Equals(falsy, value);
        }

        public static bool operator ==(DynamicFalsy falsy, object value)
        {
            return Equals(falsy, value);
        }
        public static bool operator !=(DynamicFalsy falsy, object value)
        {
            return !Equals(falsy, value);
        }

        public static bool operator ==(bool value, DynamicFalsy falsy)
        {
            return ReferenceEquals(null, falsy) ? value : falsy.Equals(value);
        }
        public static bool operator !=(bool value, DynamicFalsy falsy)
        {
            return !(ReferenceEquals(null, falsy) ? value : falsy.Equals(value));
        }

        public static bool operator ==(DynamicFalsy falsy, bool value)
        {
            return ReferenceEquals(null, falsy) ? value : falsy.Equals(value);
        }
        public static bool operator !=(DynamicFalsy falsy, bool value)
        {
            return !(ReferenceEquals(null, falsy) ? value : falsy.Equals(value));
        }








        public abstract bool IsFalsyEquivalent();
        public abstract bool IsFalsyNull();
        public abstract bool IsFalsyNaN();
        public abstract bool Equals(DynamicFalsy arg);
        protected internal abstract bool GetBooleanValue();
        protected abstract bool InternalEquals(object o);
    }
}