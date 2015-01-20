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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Falsy.NET
{
    internal static class Invoke<T>
    {
        public static Lazy<Func<T, object>> CreateLazy(PropertyInfo info)
        {
            return new Lazy<Func<T, object>>(() => Create(info));
        }

        public static Lazy<Constants.Typed<T>.Invoker> CreateLazy(MethodInfo info)
        {
            return new Lazy<Constants.Typed<T>.Invoker>(() => Create(info));
        }

        public static Lazy<Func<T, object>> CreateLazy(FieldInfo info)
        {
            return new Lazy<Func<T, object>>(() => Create(info));
        }







        public static Func<T, object> Create(FieldInfo info)
        {
            var parameter = Expression.Parameter(Constants.Typed<T>.OwnerType, "instance");
            var memberExpression = Expression.Field(parameter, info);
            var boxExpression = Expression.Convert(memberExpression, typeof (object));
            var lambda = Expression.Lambda<Func<T, object>>(boxExpression, parameter);

            return lambda.Compile();
        }


        public static Func<T, object> Create(PropertyInfo info)
        {
            var parameter = Expression.Parameter(Constants.Typed<T>.OwnerType, "instance");
            var memberExpression = Expression.Property(parameter, info);
            var boxExpression = Expression.Convert(memberExpression, typeof (object));
            var lambda = Expression.Lambda<Func<T, object>>(boxExpression, parameter);

            return lambda.Compile();
        }









        public static Constants.Typed<T>.Invoker Create(MethodInfo method)
        {
            var instance = Expression.Parameter(Constants.Typed<T>.OwnerType, "instance");
            var @params = Expression.Parameter(Constants.ObjectArrayType, "params");

            var unboxedParameters = method.GetParameters()
                                          .Select(x => Expression.Convert(Expression.ArrayIndex(@params, Expression.Constant(x.Position)), x.ParameterType));

            var call = Expression.Call(instance, method, unboxedParameters);

            Expression wrapper;
            if (method.ReturnType == Constants.VoidType)
            {
                var @null = Expression.Constant(null);

                var returnTarget = Expression.Label(typeof (object), "result");
                var returnLabel = Expression.Label(returnTarget, @null);

                wrapper = Expression.Block
                    (
                        call,
                        Expression.Return(returnTarget, @null, typeof (object)),
                        returnLabel
                    );
            }
            else if (method.ReturnType == Constants.ObjectType)
            {
                wrapper = call;
            }
            else
            {
                wrapper = Expression.Convert(call, typeof(object));
            }


            var lambda = Expression.Lambda<Constants.Typed<T>.Invoker>(wrapper, "invoker", new[] {instance, @params});
            var invoker = lambda.Compile();

            return invoker;
        }

        // IL Generator
        //    public static Constants.Typed<T>.Invoker Create(MethodInfo method)
        //    {
        //        // Create a dynamic method and obtain its IL generator to inject code
        //        var dynamicMethod = new DynamicMethod("", Constants.ObjectType, Constants.Typed<T>.ArgTypes, Constants.Typed<T>.InvokeType);
        //        var il = dynamicMethod.GetILGenerator();





        //        // If method isn't static push target instance on top of stack
        //        il.Emit(OpCodes.Ldarg_0); // Argument 0 of dynamic method is target instance

        //        // Lay out args array onto stack
        //        var parms = method.GetParameters();
        //        for (var i = 0; i < parms.Length; i++)
        //        {
        //            var parameterInfo = parms[i];
        //            // Push args array reference onto the stack, followed
        //            // by the current argument index (i). The Ldelem_Ref opcode
        //            // will resolve them to args[i]

        //            // Argument 1 of dynamic method is argument array
        //            il.Emit(OpCodes.Ldarg_1);
        //            il.Emit(OpCodes.Ldc_I4, i);
        //            il.Emit(OpCodes.Ldelem_Ref);

        //            // If parameterInfo is a value type perform an unboxing
        //            var parmType = parameterInfo.ParameterType;

        //            if (parmType.IsValueType)
        //            {
        //                il.Emit(OpCodes.Unbox_Any, parmType);
        //            }
        //        }




        //        // Perform actual call.
        //        // If method is not final and virtual callvirt is required
        //        // otherwise a normal call will be emitted
        //        var callCode = !method.IsFinal && method.IsVirtual
        //            ? OpCodes.Callvirt
        //            : OpCodes.Call;

        //        il.Emit(callCode, method);

        //        if (method.ReturnType == typeof (void))
        //        {
        //            il.Emit(OpCodes.Ldnull);
        //        }
        //        // If result is of value type it needs to be boxed
        //        else if (method.ReturnType.IsValueType)
        //        {
        //            il.Emit(OpCodes.Box, method.ReturnType);
        //        }

        //        // Emit return opcode
        //        il.Emit(OpCodes.Ret);




        //        return (Constants.Typed<T>.Invoker) dynamicMethod.CreateDelegate(Constants.Typed<T>.InvokerType);
        //    }
    }
}