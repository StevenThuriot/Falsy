using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Falsy.NET.Internals.TypeBuilder
{
    class MethodMemberDefinition : MemberDefinition
    {
        private readonly Delegate _delegate;

        public MethodMemberDefinition(string name, Delegate @delegate, bool isVirtual)
            : base(name, @delegate.GetType(), isVirtual)
        {
            _delegate = @delegate;
        }

        internal override void Build(System.Reflection.Emit.TypeBuilder typeBuilder)
        {
            var name = Name;

            var methodInfo = _delegate.Method;
            var parameterTypes = methodInfo.GetParameters().Select(x => x.ParameterType).ToArray();

            var methodAttributes = MethodAttributes.Public | MethodAttributes.Final |
                                   MethodAttributes.HideBySig | MethodAttributes.NewSlot;

            if (IsVirtual)
                methodAttributes |= MethodAttributes.Virtual;

            var methodBuilder = typeBuilder.DefineMethod(name, methodAttributes, methodInfo.ReturnType, parameterTypes);

            var generator = methodBuilder.GetILGenerator();

            for (var i = 1; i <= parameterTypes.Length; i++)
                generator.Emit(OpCodes.Ldarg_S, i);

            generator.Emit(OpCodes.Call, methodInfo);
            generator.Emit(OpCodes.Ret);
        }
    }
}