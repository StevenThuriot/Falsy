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
                dynamic dynamicValue = _instance[binder.Name];
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
            if (indexes.Length != 1)
            {
                result = null;
                return false;
            }

            result = _instance[indexes[0]];
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (indexes.Length != 1)
                return false;

            _instance[indexes[0]] = value;
            return true;
        }
    }
}