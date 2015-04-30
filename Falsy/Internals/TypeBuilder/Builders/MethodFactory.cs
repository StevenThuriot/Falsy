using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Falsy.NET.Internals.TypeBuilder.Builders
{
    static class MethodFactory
    {
        public static MethodBuilder BuildMethod(this System.Reflection.Emit.TypeBuilder typeBuilder, DynamicMember node)
        {
            var name = node.Name;
            Delegate call = ((dynamic)node).Value;
            

            var methodInfo = call.Method;
            var parameterTypes = methodInfo.GetParameters().Select(x => x.ParameterType).ToArray();

            var methodAttributes = MethodAttributes.Public | MethodAttributes.Final |
                                   MethodAttributes.HideBySig | MethodAttributes.NewSlot;

            if (node.IsVirtual)
                methodAttributes |= MethodAttributes.Virtual;

            var methodBuilder = typeBuilder.DefineMethod(name, methodAttributes, methodInfo.ReturnType, parameterTypes);

            var generator = methodBuilder.GetILGenerator();

            for (var i = 1; i <= parameterTypes.Length; i++)
                generator.Emit(OpCodes.Ldarg_S, i);

            generator.Emit(OpCodes.Call, methodInfo);
            generator.Emit(OpCodes.Ret);

            return methodBuilder;
        }
    }
}
