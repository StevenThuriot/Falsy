using System.Collections.Generic;

namespace Falsy.NET.Internals
{
    static class TypedDictionaryBuilder
    {
        public static object Build<TKey, TValue>(IDictionary<TKey, TValue> instance) => TypedDictionaryBuilder<TKey, TValue>.Build((dynamic)instance);
    }

    static class TypedDictionaryBuilder<TKey, TValue>
    {
        public static object Build<T>(T instance)
            where T : IDictionary<TKey, TValue>
        {
            return new TypedDictionaryFalsy<T, TKey, TValue>(instance);
        }
    }
}