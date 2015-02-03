#region License

//  
// Copyright 2015 Steven Thuriot
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 

#endregion

using System.Collections;
using System.Diagnostics;
using System.Dynamic;
using Invocation;

namespace Falsy.NET.Internals
{
    [DebuggerDisplay("DictionaryFalsy: {_instance} == {GetBooleanValue()}")]
    public class DictionaryFalsy<T> : DynamicFalsy<T>
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

                if (ReferenceEquals(null, value))
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
                result = NET.Falsy.Falsify(value);
                return true;
            }

            result = UndefinedFalsy.Value;
            return false;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            return TypeInfo<T>.TrySetIndexer(_instance, indexes, value);
        }
    }
}