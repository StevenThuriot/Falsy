using System.Collections;
using System.Diagnostics;
using System.Dynamic;
using Horizon;

namespace Falsy.NET.Internals
{
    [DebuggerDisplay("DictionaryFalsy: {_instance} == {GetBooleanValue()}")]
    public class DictionaryFalsy<T> : EnumerableFalsy<T>
        where T : IDictionary
    {
        internal DictionaryFalsy(T instance)
            : base(instance)
        {
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_instance.Contains(binder.Name))
            {
                var value = _instance[binder.Name];

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

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _instance[binder.Name] = value;
            return true;
        }
    }
}