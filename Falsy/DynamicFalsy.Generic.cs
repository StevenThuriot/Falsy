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

namespace Falsy.NET
{
    [DebuggerDisplay("Falsy: {_instance} == {GetBooleanValue()}")]
    public sealed class DynamicFalsy<T> : DynamicFalsy
    {
        private static readonly Predicate<T> Falsy;

        static DynamicFalsy()
        {
            Predicate<T> nullCheck = obj => ReferenceEquals(null, obj);

            if (Constants.Typed<T>.OwnerType == Constants.StringType)
            {
                Falsy = obj => nullCheck(obj) || ((string) (object) obj) == "";
            }
            else if (Constants.Typed<T>.OwnerType == Constants.IntegerType)
            {
                Falsy = obj => nullCheck(obj) || ((int) (object) obj) == 0;
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
            else if (Constants.Typed<T>.OwnerType == Constants.BooleanType)
            {
                Falsy = obj => nullCheck(obj) || ((bool) (object) obj) == false;
            }
            else
            {
                Falsy = nullCheck;
            }
        }






        private readonly T _instance;
        private readonly Lazy<bool> _booleanValue;

        public DynamicFalsy(T instance)
        {
            _instance = instance;
            _booleanValue = new Lazy<bool>(() => Falsy(_instance));
        }


        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            //TODO: Fields?

            object prop;
            var @return = TypeInfo<T>.TryGetProperty(_instance, binder.Name, out prop);

            dynamic value = prop;
            result = NET.Falsy.Falsify(value);

            return @return;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            //TODO: Fields?
            return TypeInfo<T>.TrySetProperty(_instance, binder.Name, value);
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            //TODO: Falsify result?
            return TypeInfo<T>.TryCall(_instance, binder, args, out  result);
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            //TODO??
            return base.TryInvoke(binder, args, out result);
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.ReturnType == Constants.BooleanType)
            {
                result = !_booleanValue.Value;
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
            //if (binder.ReturnType != Constants.BooleanType)
            //    return base.TryBinaryOperation(binder, arg, out result);
            
            bool argumentValue;
            if (arg is bool)
            {
                argumentValue = (bool) arg;
            }
            else
            {
                var falsy = arg as DynamicFalsy;
                if (!ReferenceEquals(null, falsy))
                {
                    argumentValue = falsy.GetBooleanValue();
                }
                else
                {
                    return base.TryBinaryOperation(binder, arg, out result);
                }
            }


            switch (binder.Operation)
            {
                case ExpressionType.Not:
                    result = _booleanValue.Value;
                    return true;



                case ExpressionType.IsTrue:
                    result = !_booleanValue.Value;
                    return true;

                case ExpressionType.IsFalse:
                    result = _booleanValue.Value;
                    return true;


                case ExpressionType.Equal:
                    result = argumentValue == !_booleanValue.Value;
                    return true;

                case ExpressionType.NotEqual:
                    result = argumentValue != !_booleanValue.Value;
                    return true;





                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    result = argumentValue && !_booleanValue.Value;
                    return true;



                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    if (binder.ReturnType == Constants.BooleanType)
                    {
                        result = argumentValue || !_booleanValue.Value;
                    }
                    else
                    {
                        if (!_booleanValue.Value)
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
                        else if (argumentValue)
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
                result = _booleanValue.Value;
                return true;
            }

            if (binder.ReturnType == Constants.BooleanType)
            {
                if (binder.Operation == ExpressionType.IsTrue)
                {
                    result = !_booleanValue.Value;
                    return true;
                }

                if (binder.Operation == ExpressionType.IsFalse)
                {
                    result = _booleanValue.Value;
                    return true;
                }
            }

            return base.TryUnaryOperation(binder, out result);
        }


        protected internal override bool GetBooleanValue()
        {
            return !_booleanValue.Value;
        }

        protected override bool InternalEquals(object o)
        {
            return o is T && Equals(_instance, o);
        }
    }
}