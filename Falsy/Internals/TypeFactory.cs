﻿#region License

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

namespace Falsy.NET.Internals
{
    class TypeFactory : DynamicObject
    {
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = this;

            if ("NEW".Equals(binder.Name, StringComparison.OrdinalIgnoreCase))
            {
                var nodes = CreateNodes(binder.CallInfo, args, objectsAreValues: false);
                TypeBuilder.CreateTypeInstance(binder.Name, nodes);
                return true;
            }
            
            if ("DEFINE".Equals(binder.Name, StringComparison.OrdinalIgnoreCase))
            {
                var nodes = CreateNodes(binder.CallInfo, args, objectsAreValues: true);
                TypeBuilder.CreateType(binder.Name, nodes);
                return true;
            }

            return false;
        }

        private static IReadOnlyList<TypeBuilder.DynamicMember> CreateNodes(CallInfo callInfo, object[] args, bool objectsAreValues)
        {
            var result = new List<TypeBuilder.DynamicMember>();

            if (callInfo.ArgumentCount != args.Length)
            {
                throw new NotSupportedException("All arguments must be named.");
            }

            var argumentNames = callInfo.ArgumentNames;

            for (var i = 0; i < argumentNames.Count; i++)
            {
                var argument = args[i];

                var dynamicMember = argument as TypeBuilder.DynamicMember;
                if (dynamicMember == null)
                {
                    var name = argumentNames[i];
                    if (objectsAreValues)
                    {
                        dynamicMember = TypeBuilder.DynamicMember.Create(name, argument);
                    }
                    else
                    {
                        var type = argument as Type;
                        if (type == null)
                            throw new NotSupportedException("Definitions require a type.");

                        dynamicMember = new TypeBuilder.DynamicMember(name, type, true);
                    }
                }

                result.Add(dynamicMember);
            }

            return result;
        }
    }
}