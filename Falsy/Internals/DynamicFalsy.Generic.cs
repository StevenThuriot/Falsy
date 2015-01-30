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
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using Invocation;

namespace Falsy.NET.Internals
{
    [DebuggerDisplay("Falsy: {_instance} == {GetBooleanValue()}")]
    public class DynamicFalsy<T> : DynamicFalsy
    {
        private static readonly Predicate<T> Falsy;


        protected readonly T _instance;
        protected readonly Lazy<bool> _isFalse;
        protected readonly Lazy<bool> _isFalsyEquivalent;
        protected readonly Lazy<bool> _isFalsyNaN;
        protected readonly Lazy<bool> _isFalsyNull;

        static DynamicFalsy()
        {
            Predicate<T> nullCheck = obj => ReferenceEquals(null, obj);

            if (Constants.Typed<T>.OwnerType == Constants.StringType)
            {
                Falsy = obj => nullCheck(obj) || ((string) (object) obj) == "";
            }
            else if (Constants.Typed<T>.OwnerType == Constants.DoubleType)
            {
                Falsy = obj =>
                        {
                            if (nullCheck(obj)) return true;

                            var value = (double) (object) obj;
                            return Double.IsNaN(value) || Math.Abs(value) < Double.Epsilon;
                        };
            }
            else if (Constants.Typed<T>.OwnerType == Constants.FloatType)
            {
                Falsy = obj =>
                        {
                            if (nullCheck(obj)) return true;

                            var value = (float) (object) obj;
                            return Single.IsNaN(value) || Math.Abs(value) < Single.Epsilon;
                        };
            }
            else if (Constants.Typed<T>.OwnerType.IsNumeric())
            {
                Falsy = obj => nullCheck(obj) || ((int) (object) obj) == 0;
            }
            else if (Constants.Typed<T>.OwnerType == Constants.BooleanType)
            {
                Falsy = obj => nullCheck(obj) || ((bool) (object) obj) == false;
            }
            else
            {
                Falsy = nullCheck;
            }
        }


        public DynamicFalsy()
        {
            throw new NotSupportedException("Creating instances of DynamicFalsy is not allowed.");
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
            object prop;

            if (TypeInfo<T>.TryGetProperty(_instance, binder.Name, out prop))
            {
                dynamic value = prop;
                result = NET.Falsy.Falsify(value);
                return true;
            }

            if (TypeInfo<T>.TryGetField(_instance, binder.Name, out prop))
            {
                dynamic value = prop;
                result = NET.Falsy.Falsify(value);
                return true;
            }

            result = UndefinedFalsy.Value;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return TypeInfo<T>.TrySetProperty(_instance, binder.Name, value) ||
                   TypeInfo<T>.TrySetField(_instance, binder.Name, value);
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            //TODO: Falsify result?
            return TypeInfo<T>.TryCall(_instance, binder, args, out result);
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.ReturnType == Constants.BooleanType)
            {
                result = !_isFalse.Value;
                return true;
            }

            if (binder.ReturnType == Constants.Typed<T>.OwnerType)
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
                    result = _isFalse.Value;
                    return true;


                case ExpressionType.IsTrue:
                    result = !_isFalse.Value;
                    return true;

                case ExpressionType.IsFalse:
                    result = _isFalse.Value;
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
                    if (binder.ReturnType == Constants.BooleanType)
                    {
                        result = !_isFalse.Value || !Equals(arg);
                    }
                    else
                    {
                        if (!_isFalse.Value)
                        {
                            if (binder.ReturnType == Constants.Typed<T>.OwnerType)
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
            if (binder.Operation == ExpressionType.Not)
            {
                result = _isFalse.Value;
                return true;
            }

            if (binder.ReturnType == Constants.BooleanType)
            {
                if (binder.Operation == ExpressionType.IsTrue)
                {
                    result = !_isFalse.Value;
                    return true;
                }

                if (binder.Operation == ExpressionType.IsFalse)
                {
                    result = _isFalse.Value;
                    return true;
                }
            }

            return base.TryUnaryOperation(binder, out result);
        }


        protected internal override bool GetBooleanValue()
        {
            return !_isFalse.Value;
        }

        public override bool IsFalsyEquivalent()
        {
            return _isFalsyEquivalent.Value;
        }

        public override bool IsFalsyNull()
        {
            return _isFalsyNull.Value;
        }

        public override bool IsFalsyNaN()
        {
            return _isFalsyNaN.Value;
        }


        public override bool Equals(DynamicFalsy arg)
        {
            if (ReferenceEquals(null, arg))
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
            if (ReferenceEquals(null, o))
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