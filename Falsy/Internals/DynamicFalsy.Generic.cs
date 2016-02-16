using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using Horizon;

namespace Falsy.NET.Internals
{
    [DebuggerDisplay("Falsy: {_instance} == {GetBooleanValue()}")]
    [TypeConverter(typeof(FalsyTypeConverter))]
	public class DynamicFalsy<T> : DynamicFalsy
    {
        static readonly Func<T, bool> Falsy;
        
        protected readonly T _instance;
        protected readonly Lazy<bool> _isFalse;
        protected readonly Lazy<bool> _isFalsyEquivalent;
        protected readonly Lazy<bool> _isFalsyNaN;
        protected readonly Lazy<bool> _isFalsyNull;

        public static Type UnderlyingType => typeof(T);

        static DynamicFalsy() //Once per T.
        {
            var ownerType = typeof(T);

            if (ownerType == typeof(string))
            {
                Falsy = obj => string.IsNullOrEmpty(__refvalue(__makeref(obj), string));
            }
            else if (ownerType == typeof(double))
            {
                Falsy = obj =>
                        {
                            if (Reference.IsNull(obj)) return true;

                            var value = __refvalue(__makeref(obj), double);
                            return double.IsNaN(value) || Math.Abs(value) < double.Epsilon;
                        };
            }
            else if (ownerType == typeof(float))
            {
                Falsy = obj =>
                        {
                            if (Reference.IsNull(obj)) return true;

                            var value = __refvalue(__makeref(obj), float);
                            return float.IsNaN(value) || Math.Abs(value) < float.Epsilon;
                        };
            }
            else if (ownerType.IsNumeric())
            {
                Falsy = obj => Reference.IsNull(obj) || 0.Equals(obj);
            }
            else if (ownerType == typeof(bool))
            {
                Falsy = obj => Reference.IsNull(obj) || false.Equals(obj);
            }
            else
            {
                Falsy = obj => Reference.IsNull(obj);
            }
        }

        internal DynamicFalsy(T instance)
        {
            _instance = instance;
            _isFalse = new Lazy<bool>(() => Falsy(_instance));

            _isFalsyNaN = new Lazy<bool>(() => FalsyNaN(_instance));
            _isFalsyNull = new Lazy<bool>(() => FalsyNull(_instance));
            _isFalsyEquivalent = new Lazy<bool>(() => FalsyEquivalent(_instance));
        }


        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            object member;

            if (Info<T>.TryGetValue(_instance, binder.Name, out member))
            {
                if (Reference.IsNotNull(member))
                {
                    dynamic value = member;
                    result = NET.Falsy.Falsify(value);
                    return true;
                }
            }

            result = UndefinedFalsy.Value;
            return true;
        }
        
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return Info<T>.TrySetProperty(_instance, binder.Name, value) ||
                   Info<T>.TrySetField(_instance, binder.Name, value);
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            object output;
            if (Info<T>.TryCall(_instance, binder, args, out output))
            {
                if (Reference.IsNull(output))
                {
                    result = UndefinedFalsy.Value;
                    return true;
                }

                dynamic value = output;
                result = NET.Falsy.Falsify(value);
                return true;
            }

            if (Info<T>.TryGetValue(_instance, binder.Name, out output))
            {
                var @delegate = output as Delegate;
                if (Reference.IsNotNull(@delegate))
                {
                    dynamic value = @delegate.FastInvoke(args);

                    if (Reference.IsNull((object)value))
                    {
                        result = UndefinedFalsy.Value;
                        return true;
                    }

                    result = NET.Falsy.Falsify(value);
                    return true;
                }
            }

            result = UndefinedFalsy.Value;
            return false;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.ReturnType == typeof(bool))
            {
                result = !_isFalse.Value;
                return true;
            }

            if (binder.ReturnType == typeof(T))
            {
                result = _instance;
                return true;
            }

            return base.TryConvert(binder, out result);
        }

        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            switch (binder.Operation)
            {
                case ExpressionType.Not:
                case ExpressionType.IsFalse:
                    result = _isFalse.Value;
                    return true;

                case ExpressionType.IsTrue:
                    result = !_isFalse.Value;
                    return true;
                    
                case ExpressionType.Equal:
                    result = Equals(arg);
                    return true;

                case ExpressionType.NotEqual:
                    result = !Equals(arg);
                    return true;

                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    result = !_isFalse.Value && Equals(arg);
                    return true;


                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    if (binder.ReturnType == typeof(bool))
                    {
                        result = !_isFalse.Value || !Equals(arg);
                    }
                    else
                    {
                        if (!_isFalse.Value)
                        {
                            if (binder.ReturnType == typeof(T))
                            {
                                result = _instance;
                            }
                            else
                            {
                                result = this;
                            }
                        }
                        else if (!Equals(arg))
                        {
                            result = arg;
                        }
                        else
                        {
                            result = false;
                        }
                    }

                    return true;
            }

            return base.TryBinaryOperation(binder, arg, out result);
        }

        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            switch (binder.Operation)
            {
                case ExpressionType.Not:
                    result = _isFalse.Value;
                    return true;

                case ExpressionType.IsTrue:
                    if (binder.ReturnType == typeof(bool))
                    {
                        result = !_isFalse.Value;
                        return true;
                    }
                    break;

                case ExpressionType.IsFalse:
                    if (binder.ReturnType == typeof(bool))
                    {
                        result = _isFalse.Value;
                        return true;
                    }
                    break;
            }
            
            return base.TryUnaryOperation(binder, out result);
        }


        protected internal override bool GetBooleanValue() => !_isFalse.Value;

        protected internal override dynamic GetValue() => _instance;

        public override bool IsFalsyEquivalent() => _isFalsyEquivalent.Value;

        public override bool IsFalsyNull() => _isFalsyNull.Value;

        public override bool IsFalsyNaN() => _isFalsyNaN.Value;


        public override bool Equals(DynamicFalsy arg)
        {
            if (Reference.IsNull(arg))
                return _isFalsyNull.Value;

            if (_isFalsyNull.Value)
                //The falsy values null and undefined are not equivalent to anything except themselves
                return arg.IsFalsyNull();

            if (_isFalsyEquivalent.Value)
                //The falsy values false, 0 (zero), and "" (empty string) are all equivalent and can be compared against each other
                return arg.IsFalsyEquivalent();

            if (_isFalsyNaN.Value)
                //Finally, the falsy value NaN is not equivalent to anything — including NaN!
                return false;

            if (_isFalse.Value == arg.GetBooleanValue())
                //Different boolean values, thus can't be equal.
                return false;

            return !arg.IsFalsyNull() && !arg.IsFalsyNaN();
        }


        protected override bool InternalEquals(object o)
        {
            if (Reference.IsNull(o))
                return _isFalsyNull.Value;

            //If o is T, then compare it to our actual value
            if (o is T)
                return Equals(o, _instance);

            //The falsy values false, 0 (zero), and "" (empty string) are all equivalent and can be compared against each other
            if (FalsyEquivalent(o))
                return _isFalse.Value;

            //Note: the falsy value NaN is not equivalent to anything — including NaN!
            return false;
        }
    }
}