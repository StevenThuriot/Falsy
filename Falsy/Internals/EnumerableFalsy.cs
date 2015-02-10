#region License	

//  Copyright 2015 Steven Thuriot
//   
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

#endregion

using System.Collections;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using Invocation;

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
				if (ReferenceEquals(null, output))
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