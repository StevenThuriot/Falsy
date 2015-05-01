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

        public static void BuildEmptyMethod(this System.Reflection.Emit.TypeBuilder typeBuilder, string name, Type returnType, Type[] parameterTypes)
        {
            var methodBuilder = typeBuilder.DefineMethod(name,
                                                         MethodAttributes.Public | MethodAttributes.Final |
                                                         MethodAttributes.HideBySig | MethodAttributes.NewSlot |
                                                         MethodAttributes.Virtual,
                                                         returnType,
                                                         parameterTypes);


            var generator = methodBuilder.GetILGenerator();

            if (returnType != typeof(void))
            {
                if (!returnType.IsValueType)
                {
                    generator.Emit(OpCodes.Ldnull);
                }
                else if (returnType.IsPrimitive)
                {
                    generator.Emit(OpCodes.Ldc_I4_0);

                    //Do we need to manually convert? Seems to work on its own!
                    //var size = System.Runtime.InteropServices.Marshal.SizeOf(returnType);
                    //if (size == 8)
                    //    generator.Emit(OpCodes.Conv_I8);
                }
                else
                {
                    var local = generator.DeclareLocal(returnType);
                    generator.Emit(OpCodes.Ldloca_S, local);
                    generator.Emit(OpCodes.Initobj, returnType);
                    generator.Emit(OpCodes.Ldloc_0);
                }
            }

            generator.Emit(OpCodes.Ret);
        }
    }
}
