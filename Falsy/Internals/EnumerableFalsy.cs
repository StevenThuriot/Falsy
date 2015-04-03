using System.Collections;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using Horizon;

namespace Falsy.NET.Internals
{
	[DebuggerDisplay("EnumerableFalsy: {_instance} == {GetBooleanValue()}")]
	public class EnumerableFalsy<T> : DynamicFalsy<T>, IEnumerable
		where T : IEnumerable
	{
		internal EnumerableFalsy(T instance)
			: base(instance)
		{
		}
        
		public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
		{
			object output;
			if (TypeInfo<T>.TryGetIndexer(_instance, indexes, out output))
			{
				if (Reference.IsNull(output))
				{
					result = UndefinedFalsy.Value;
					return true;
				}

				dynamic value = output;
				result = Falsy.Falsify(value);
				return true;
			}

			result = UndefinedFalsy.Value;
			return false;
		}

		public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
		{
			return TypeInfo<T>.TrySetIndexer(_instance, indexes, value);
		}
		
		public IEnumerator GetEnumerator()
		{
			var enumerator = (from dynamic item in _instance
			                  select Falsy.Falsify(item))
								.GetEnumerator();

			return enumerator;
		}
	}
}