using System;
using System.Linq;
using Horizon;

namespace Falsy.NET.Internals.TypeBuilder
{
    class EmptyDynamicMethod : DynamicMember
    {
        public Type[] ParameterTypes { get; private set; }

        public EmptyDynamicMethod(string name, Type type, Type[] parameterTypes) 
            : base(name, type, MemberType.Method, true)
        {
            ParameterTypes = parameterTypes;
        }
        public EmptyDynamicMethod(IMethodCaller caller) 
            : base(caller.Name, caller.ReturnType, MemberType.Method, true)
        {
            ParameterTypes = caller.ParameterTypes.Select(x => x.ParameterType).ToArray();
        }
    }
}