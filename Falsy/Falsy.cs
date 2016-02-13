using System.Collections;
using System.Collections.Generic;
using Falsy.NET.Internals;
using Horizon;
using Horizon.Forge;
using System.Reflection;
using System.Linq.Expressions;
using System;

namespace Falsy.NET
{
    public static class Falsy
    {
        public static readonly dynamic undefined = UndefinedFalsy.Value;

        public static dynamic Falsify<T>(this T instance)
        {
            if (Reference.IsNull(instance))
                return undefined;

            if (typeof (T) == typeof(object))
            {
                //suspicious~
                var actualType = instance.GetType();
                if (actualType == typeof(object))
                {
                    //nope, it's actually object after all
                    return InternalFalsify(instance);
                }

                if (actualType.IsNotPublic)
                {
                    //DLR does not like non-public types
                    return CallRealTypedFalsify(instance, actualType);
                }
            }
            else if (typeof(T).IsNotPublic)
            {
                //DLR does not like non-public types
                return CallRealTypedFalsify(instance, instance.GetType());
            }

            //Resolve actual type through the DLR, since it's public this will work and it will be fast.
            return RealTypedFalsify((dynamic)instance);
        }

        private static dynamic CallRealTypedFalsify<T>(T instance, Type actualType)
        {
            if (actualType.IsAssignableFrom(typeof(DynamicFalsy)))
                return instance;

            var parameter = Expression.Parameter(actualType, "instance");

            string method;
            if (typeof(IDictionary).IsAssignableFrom(actualType))
            {
                method = "InternalDictionaryFalsify";
            }
            else if (typeof (IEnumerable).IsAssignableFrom(actualType))
            {
                method = "InternalEnumerableFalsify";
            }
            else
            {
                method = "InternalFalsify";
            }

            var call = Expression.Call(typeof(Falsy), method, new[] { actualType }, parameter);

            var lambda = Expression.Lambda<Func<T, object>>(call, parameter);
            var @delegate = lambda.Compile();

            return @delegate(instance); //TODO : Cache
        }

        private static dynamic RealTypedFalsify<T>(this T instance)
        {
            if (instance is DynamicFalsy)
                return instance;

            if (instance is IDictionary)
                return InternalDictionaryFalsify((dynamic)instance); //TODO : Fix for internal types

            if (instance is IEnumerable)
                return InternalEnumerableFalsify((dynamic)instance); //TODO : Fix for internal types
            
            return InternalFalsify(instance);
        }


        static dynamic InternalFalsify<T>(T instance)
        {
            return new DynamicFalsy<T>(instance);
        }

        static dynamic InternalEnumerableFalsify<T>(T instance)
            where T : IEnumerable
        {
            return new EnumerableFalsy<T>(instance);
        }

        static dynamic InternalDictionaryFalsify<T>(T instance)
            where T : IDictionary
        {
            if (Helper<T>.IsGenericDictionary)
                return TypedDictionaryBuilder.Build((dynamic) instance);

            return new DictionaryFalsy<T>(instance);
        }


        public static dynamic Define => new TypeFactory.DefineTypeFactory();
        public static dynamic WrapType => new TypeFactory.WrapFactory();

        public static dynamic New { get; } = new TypeFactory.NewTypeFactory();
        public static dynamic Wrap { get; } = new TypeFactory.NewWrapFactory();


        static class Helper<T>
        {
            static bool? _isGenericDictionary;
            public static bool IsGenericDictionary
            {
                get
                {
                    if (_isGenericDictionary.HasValue)
                        return _isGenericDictionary.Value;

                    var result = typeof(T).GetInterface(typeof(IDictionary<,>).Name) != null;
                    _isGenericDictionary = result;

                    return result;
                }
            }
        }
    }
}