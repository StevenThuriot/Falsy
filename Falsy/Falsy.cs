using System.Collections;
using System.Collections.Generic;
using Falsy.NET.Internals;
using Horizon;
using Horizon.Forge;

namespace Falsy.NET
{
    public static class Falsy
    {
        public static readonly dynamic undefined = UndefinedFalsy.Value;


        public static dynamic Falsify(this object instance)
        {
            return Reference.IsNull(instance)
                ? undefined
                //Resolve actual type through the DLR
                : InternalFalsify((dynamic) instance);
        }

        public static dynamic Falsify(this IEnumerable instance)
        {
            return Reference.IsNull(instance)
                ? undefined
                //Resolve actual type through the DLR
                : InternalEnumerableFalsify((dynamic) instance);
        }

        public static dynamic Falsify(this IDictionary instance)
        {
            return Reference.IsNull(instance)
                ? undefined
                //Resolve actual type through the DLR
                : InternalDictionaryFalsify((dynamic)instance);
        }

        public static dynamic Falsify(this DynamicFalsy instance)
        {
            return Reference.IsNull(instance)
                ? undefined
                : instance;
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