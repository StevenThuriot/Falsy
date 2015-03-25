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

using System;
using System.Reflection;

namespace Falsy.NET.Internals.TypeBuilder
{
    class DynamicMember
    {
        public readonly bool IsProperty;
        public readonly bool IsVirtual;
        public readonly string Name;
        private readonly Type _type;

        public DynamicMember(string name, Type type, bool isProperty, bool isVirtual)
        {
            Name = name;
            _type = type;
            IsProperty = isProperty;
            IsVirtual = isVirtual;
        }

        public DynamicMember(PropertyInfo info, bool isVirtual)
        {
            Name = info.Name;
            _type = info.PropertyType;
            IsProperty = true;
            IsVirtual = isVirtual;
        }

        public DynamicMember(FieldInfo info, bool isVirtual)
        {
            Name = info.Name;
            _type = info.FieldType;
            IsProperty = false;
            IsVirtual = isVirtual;
        }

        public Type Type
        {
            get { return _type; }
        }
        
        public static DynamicMember Create<T>(string name, T value, bool isProperty = true, bool isVirtual = false)
        {
            return new DynamicMember<T>(name, value, isProperty, isVirtual);
        }
    }
}