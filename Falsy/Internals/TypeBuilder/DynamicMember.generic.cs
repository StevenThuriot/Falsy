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

using Horizon;

namespace Falsy.NET.Internals.TypeBuilder
{
    sealed class DynamicMember<T> : DynamicMember, ICanVisit
    {
        private readonly T _value;

        public DynamicMember(string name, T value, bool isProperty, bool isVirtual)
            : base(name, typeof (T), isProperty, isVirtual)
        {
            _value = value;
        }

        public void Visit(dynamic instance)
        {
            if (IsProperty)
            {
                TypeInfo.SetProperty(instance, Name, _value);
            }
            else
            {
                TypeInfo.SetField(instance, Name, _value);
            }
        }
    }
}