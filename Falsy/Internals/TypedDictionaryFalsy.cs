using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using Horizon;

namespace Falsy.NET.Internals
{
    [DebuggerDisplay("TypedDictionaryFalsy: {_instance} == {GetBooleanValue()}")]
    public class TypedDictionaryFalsy<T, TKey, TValue> : EnumerableFalsy<T>
        where T : IDictionary<TKey, TValue>
    {
        internal TypedDictionaryFalsy(T instance)
            : base(instance)
        {
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            TValue value;
            TKey key = GetKey(binder.Name);

            if (_instance.TryGetValue(key, out value))
            {
                if (Reference.IsNull(value))
                {
                    result = UndefinedFalsy.Value;
                    return true;
                }

                dynamic dynamicValue = value;
                result = Falsy.Falsify(dynamicValue);
                return true;
            }

            return base.TryGetMember(binder, out result);
        }

        static TKey GetKey(string name)
        {            
            if (typeof(TKey) == typeof(string))
            {
                //Since we know name is a string and so is TKey, we can use C# tricks
                return __refvalue(__makeref(name), TKey);
            }

            return (TKey)(object)name;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            TKey key = GetKey(binder.Name);
            _instance[key] = (TValue)value;
            return true;
        }
    }
}