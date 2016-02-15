using System;
using System.Dynamic;
using Horizon;

namespace Falsy.NET.Internals
{

    public abstract class DynamicFalsy : DynamicObject
    {
        protected bool Equals(bool other)
        {
            var isFalsyEquivalent = IsFalsyEquivalent();
            return other ? !isFalsyEquivalent : isFalsyEquivalent;
        }

        public override sealed bool Equals(object obj)
        {
            if (Reference.IsNull(obj))
                return IsFalsyNull();

            if (obj is bool)
                return Equals((bool) obj);

            var falsy = obj as DynamicFalsy;
            if (Reference.IsNotNull(falsy))
                return Equals(falsy);

            if (IsFalsyNull())
                //The falsy values null and undefined are not equivalent to anything except themselves
                return FalsyNull(obj);

            if (IsFalsyEquivalent())
                //The falsy values false, 0 (zero), and "" (empty string) are all equivalent and can be compared against each other
                return FalsyEquivalent(obj);

            if (IsFalsyNaN())
                //Finally, the falsy value NaN is not equivalent to anything — including NaN!
                return false;

            if (FalsyNull(obj) || FalsyNaN(obj))
                return false;

            return InternalEquals(obj);
        }
        
        public override sealed int GetHashCode() => 1; //Make sure falsies clash.

        public static bool FalsyEquivalent(object arg)
        {
            if (Reference.IsNull(arg))
                return false;

            if (Equals(false, arg) || Equals("", arg))
                return true;

            dynamic argument = arg;
            bool isNumeric = NumericInfo.IsNumeric(argument);

            if (!isNumeric) return false;

            var result = (double) Math.Abs(argument) < double.Epsilon;
            return result;
        }

        public static bool FalsyNull(object arg) => Reference.IsNull(arg) || ReferenceEquals((object)UndefinedFalsy.Value, arg);

        public static bool FalsyNaN(object arg) => Equals(float.NaN, arg) || Equals(double.NaN, arg);

        public static bool operator ==(DynamicFalsy falsy1, DynamicFalsy falsy2)
        {
            // ReSharper disable PossibleNullReferenceException
            if (Reference.IsNull(falsy1))
                return Reference.IsNull(falsy2) || falsy2.Equals(null);

            if (Reference.IsNull(falsy2))
                return falsy1.Equals(null);
            // ReSharper restore PossibleNullReferenceException

            return Equals(falsy1, falsy2);
        }

        public static bool operator !=(DynamicFalsy falsy1, DynamicFalsy falsy2) => !(falsy1 == falsy2);

        public static bool operator ==(object value, DynamicFalsy falsy) => Equals(falsy, value);

        public static bool operator !=(object value, DynamicFalsy falsy) => !Equals(falsy, value);

        public static bool operator ==(DynamicFalsy falsy, object value) => Equals(falsy, value);

        public static bool operator !=(DynamicFalsy falsy, object value) => !Equals(falsy, value);

        public static bool operator ==(bool value, DynamicFalsy falsy) => Reference.IsNull(falsy) ? value : falsy.Equals(value);

        public static bool operator !=(bool value, DynamicFalsy falsy) => !(Reference.IsNull(falsy) ? value : falsy.Equals(value));

        public static bool operator ==(DynamicFalsy falsy, bool value) => Reference.IsNull(falsy) ? value : falsy.Equals(value);

        public static bool operator !=(DynamicFalsy falsy, bool value) => !(Reference.IsNull(falsy) ? value : falsy.Equals(value));


        public abstract bool IsFalsyEquivalent();
        public abstract bool IsFalsyNull();
        public abstract bool IsFalsyNaN();
        public abstract bool Equals(DynamicFalsy arg);
        protected internal abstract bool GetBooleanValue();
        protected internal abstract dynamic GetValue();
        protected abstract bool InternalEquals(object o);
    }
}