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
                    return ResolverRealTypeCaller(actualType)(instance);
                }
            }
            else if (typeof(T).IsNotPublic || typeof(T).IsInterface)
            {
                //DLR does not like non-public types
                return ResolverRealTypeCaller(instance.GetType())(instance);
            }

            //Resolve actual type through the DLR, since it's public this will work and it will be fast.
            return RealTypedFalsify((dynamic)instance);
        }


        static readonly Dictionary<Type, Func<object, object>> _realTypeCallerCache = new Dictionary<Type, Func<object, object>>();
        static Func<object, object> ResolverRealTypeCaller(Type actualType)
        {
            Func<object, object> @delegate;

            if (!_realTypeCallerCache.TryGetValue(actualType, out @delegate))
            {
                if (actualType.IsAssignableFrom(typeof(DynamicFalsy)))
                {
                    @delegate = new Func<object, object>(x => x);
                }
                else
                {
                    string method;
                    if (typeof(IDictionary).IsAssignableFrom(actualType))
                    {
                        method = "InternalDictionaryFalsify";
                    }
                    else if (typeof(IEnumerable).IsAssignableFrom(actualType))
                    {
                        method = "InternalEnumerableFalsify";
                    }
                    else
                    {
                        method = "InternalFalsify";
                    }

                    var parameter = Expression.Parameter(typeof(object), "instance");
                    var call = Expression.Call(typeof(Falsy), method, new[] { actualType }, Expression.Convert(parameter, actualType));
                    var lambda = Expression.Lambda<Func<object, object>>(call, parameter);

                    @delegate = lambda.Compile();
                }

                _realTypeCallerCache[actualType] = @delegate;
            }

            return @delegate;
        }

        static dynamic RealTypedFalsify<T>(this T instance)
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