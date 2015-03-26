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
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Falsy.NET.Internals.TypeBuilder
{
    public abstract class TypeFactory : DynamicObject
    {
        static IReadOnlyList<DynamicMember> CreateNodes(CallInfo callInfo, IReadOnlyList<object> args, bool objectsAreValues)
        {
            var result = new List<DynamicMember>();

            if (callInfo.ArgumentCount != args.Count)
                throw new NotSupportedException("All arguments must be named.");

            var argumentNames = callInfo.ArgumentNames;

            for (var i = 0; i < argumentNames.Count; i++)
            {
                var argument = args[i];

                var dynamicMember = argument as DynamicMember;
                if (dynamicMember == null)
                {
                    var name = argumentNames[i];
                    if (objectsAreValues)
                    {
                        dynamicMember = DynamicMember.Create(name, (dynamic) argument, true, false);
                    }
                    else
                    {
                        var type = argument as Type;
                        if (type == null)
                            throw new NotSupportedException("Definitions require a type.");

                        dynamicMember = new DynamicMember(name, type, true, false);
                    }
                }

                result.Add(dynamicMember);
            }

            return result;
        }

        public class NewTypeFactory : TypeFactory
        {
            internal NewTypeFactory() { }

            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                var nodes = CreateNodes(binder.CallInfo, args, objectsAreValues: true);
                result = TypeBuilder.CreateTypeInstance(binder.Name, nodes);
                return true;
            }
        }
        public class DefineTypeFactory : TypeFactory
        {
            private HashSet<Type> _interfaces;
            
            internal DefineTypeFactory() { }

            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                if (StringComparer.OrdinalIgnoreCase.Equals("WITHINTERFACE", binder.Name))
                {
                    var types = args.Cast<Type>();

                    if (_interfaces == null)
                    {
                        _interfaces = new HashSet<Type>(types);
                    }
                    else
                    {
                        foreach (var type in types)
                            _interfaces.Add(type);
                    }


                    result = this;
                    return true;
                }

                
                var nodes = CreateNodes(binder.CallInfo, args, objectsAreValues: false);
                result = TypeBuilder.CreateType(binder.Name, nodes, interfaces: _interfaces);
                return true;
            }
        }
    }
}
