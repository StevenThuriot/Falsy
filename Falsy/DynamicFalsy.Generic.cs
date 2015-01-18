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
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Falsy.NET
{
    [DebuggerDisplay("Falsy: {_instance} == {GetBooleanValue()}")]
    public sealed class DynamicFalsy<T> : DynamicFalsy
    {
        public static readonly Type InstanceType = typeof (T);
        
        private static readonly Predicate<T> _falsy;
        private static readonly Dictionary<string, Info> _properties;


        private readonly T _instance;

        static DynamicFalsy()
        {
            Predicate<T> nullCheck = obj => ReferenceEquals(null, obj);

            if (InstanceType == typeof (string))
            {
                _falsy = obj => nullCheck(obj) || ((string) (object) obj) == "";
            }
            else if (InstanceType == typeof (int))
            {
                _falsy = obj => nullCheck(obj) || ((int) (object) obj) == 0;
            }
            else if (InstanceType == typeof (double))
            {
                _falsy = obj =>
                {
                    if (nullCheck(obj)) return true;

                    var value = (double) (object) obj;
                    return Double.IsNaN(value) || Math.Abs(value) < Double.Epsilon;
                };
            }
            else if (InstanceType == typeof (float))
            {
                _falsy = obj =>
                {
                    if (nullCheck(obj)) return true;

                    var value = (float) (object) obj;
                    return Single.IsNaN(value) || Math.Abs(value) < Single.Epsilon;
                };
            }
            else if (InstanceType == typeof (bool))
            {
                _falsy = obj => nullCheck(obj) || ((bool) (object) obj) == false;
            }
            else
            {
                _falsy = nullCheck;
            }

            _properties = InstanceType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.GetField)
                .Where(x => ((MemberTypes.Property | MemberTypes.Field) & x.MemberType) == x.MemberType)
                .Select(x => new Info(x))
                .ToDictionary(x => x.Name, x => x);
        }

        public DynamicFalsy(T instance)
        {
            _instance = instance;
        }


        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            Info info;
            if (!_properties.TryGetValue(binder.Name, out info))
            {
                result = null;
                return false;
            }

            result = info.GetValue(_instance).Falsify(/*info.ReturnType*/);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            //TODO??
            return base.TrySetMember(binder, value);
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            //TODO??
            return base.TryInvokeMember(binder, args, out result);
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            //TODO??
            return base.TryInvoke(binder, args, out result);
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.ReturnType == typeof (bool))
            {
                result = !_falsy(_instance);
                return true;
            }

            if (binder.ReturnType == InstanceType)
            {
                result = _instance;
                return true;
            }

            return base.TryConvert(binder, out result);
        }

        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            //if (binder.ReturnType != typeof (bool))
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
                    result = _falsy(_instance);
                    return true;

                case ExpressionType.IsTrue:
                    result = !_falsy(_instance);
                    return true;

                case ExpressionType.IsFalse:
                    result = _falsy(_instance);
                    return true;

                case ExpressionType.And:
                    result = argumentValue & !_falsy(_instance);
                    return true;
                case ExpressionType.AndAlso:
                    result = argumentValue && !_falsy(_instance);
                    return true;

                case ExpressionType.Equal:
                    result = argumentValue == !_falsy(_instance);
                    return true;
                case ExpressionType.NotEqual:
                    result = argumentValue != !_falsy(_instance);
                    return true;

                case ExpressionType.Or:
                    result = argumentValue | !_falsy(_instance);
                    return true;
                case ExpressionType.OrElse:
                    result = argumentValue || !_falsy(_instance);
                    return true;
            }

            return base.TryBinaryOperation(binder, arg, out result);
        }

        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            if (binder.Operation == ExpressionType.Not)
            {
                result = _falsy(_instance);
                return true;
            }

            if (binder.ReturnType == typeof (bool))
            {
                if (binder.Operation == ExpressionType.IsTrue)
                {
                    result = !_falsy(_instance);
                    return true;
                }

                if (binder.Operation == ExpressionType.IsFalse)
                {
                    result = _falsy(_instance);
                    return true;
                }
            }

            return base.TryUnaryOperation(binder, out result);
        }


        protected internal override bool GetBooleanValue()
        {
            return !_falsy(_instance);
        }

        protected override bool InternalEquals(object o)
        {
            return o is T && Equals(_instance, o);
        }

        private class Info
        {
            //Fields for speed!
            public readonly string Name;
            //public readonly Type ReturnType;

            private readonly Func<T, object> _getValue;

            public Info(MemberInfo memberInfo)
            {
                Name = memberInfo.Name;

                var parameter = Expression.Parameter(InstanceType, "instance");
                Expression memberExpression;

                var info = memberInfo as FieldInfo;
                if (info != null)
                {
                    //Field
                    //ReturnType = info.FieldType;
                    memberExpression = Expression.Field(parameter, info);
                }
                else
                {
                    //Property
                    var propertyInfo = ((PropertyInfo) memberInfo);
                    //ReturnType = propertyInfo.PropertyType;
                    memberExpression = Expression.Property(parameter, propertyInfo);
                }

                var boxExpression = Expression.Convert(memberExpression, typeof (object));
                var lambda = Expression.Lambda<Func<T, object>>(boxExpression, parameter);

                _getValue = lambda.Compile();
            }

            public dynamic GetValue(T instance)
            {
                return _getValue(instance);
            }
        }
    }
}