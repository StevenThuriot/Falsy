using System.Collections;
using System.Collections.Generic;
using Falsy.NET.Internals;
using Horizon;
using Horizon.Forge;
using System.Reflection;

namespace Falsy.NET
{
    public static class Falsy
    {
        public static readonly dynamic undefined = UndefinedFalsy.Value;
        private static MethodInfo _caller = typeof(Falsy).GetMethod("RealTypedFalsify", BindingFlags.Static | BindingFlags.NonPublic);


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
                    return _caller.MakeGenericMethod(actualType).Invoke(null, new object[] { instance }); //TODO : Cached Expression Tree
                }
            }
            else if (typeof(T).IsNotPublic)
            {
                //DLR does not like non-public types
                return _caller.MakeGenericMethod(instance.GetType()).Invoke(null, new object[] { instance }); //TODO : Cached Expression Tree
            }

            //Resolve actual type through the DLR
            return RealTypedFalsify((dynamic)instance);
        }

        private static dynamic RealTypedFalsify<T>(this T instance)
        {
            if (instance is DynamicFalsy)
                return instance;
            
            if (instance is IDictionary)
                return InternalDictionaryFalsify((dynamic)instance); //TODO : Fix for internal types

            if (instance is IEnumerable)
                return InternalEnumerableFalsify((dynamic)instance); //TODO : Fix for internal types

            //Resolve actual type through the DLR
            return InternalFalsify(instance);
        }

        //public static dynamic Falsify(this IEnumerable instance)
        //{
        //    return Reference.IsNull(instance)
        //        ? undefined
        //        //Resolve actual type through the DLR
        //        : InternalEnumerableFalsify((dynamic) instance);
        //}

        //public static dynamic Falsify(this IDictionary instance)
        //{
        //    return Reference.IsNull(instance)
        //        ? undefined
        //        //Resolve actual type through the DLR
        //        : InternalDictionaryFalsify((dynamic)instance);
        //}

        //public static dynamic Falsify(this DynamicFalsy instance)
        //{
        //    return Reference.IsNull(instance)
        //        ? undefined
        //        : instance;
        //}


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