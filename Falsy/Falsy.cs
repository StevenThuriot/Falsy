using System;
using System.Collections;
using Falsy.NET.Internals;
using Falsy.NET.Internals.TypeBuilder;
using Horizon;

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


        private static dynamic InternalFalsify<T>(T instance)
        {
            return new DynamicFalsy<T>(instance);
        }

        private static dynamic InternalEnumerableFalsify<T>(T instance)
            where T : IEnumerable
        {
            return new EnumerableFalsy<T>(instance);
        }

        private static dynamic InternalDictionaryFalsify<T>(T instance)
            where T : IDictionary
        {
            if (Constants.Typed<T>.IsGenericDictionary)
                return TypedDictionaryBuilder.Build((dynamic) instance);

            return new DictionaryFalsy<T>(instance);
        }




        private static readonly Lazy<TypeFactory> _newFactory = new Lazy<TypeFactory>(() => new TypeFactory.NewTypeFactory());
        private static readonly Lazy<TypeFactory> _defineFactory = new Lazy<TypeFactory>(() => new TypeFactory.DefineTypeFactory());
        public static dynamic Define
        {
            get { return _defineFactory.Value; }
        }
        public static dynamic New
        {
            get { return _newFactory.Value; }
        }



    }
}