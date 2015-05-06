using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Falsy.NET.Internals.TypeBuilder.Builders;
using Horizon;

namespace Falsy.NET.Internals.TypeBuilder
{
    abstract class MemberDefinition
    {
        public readonly bool IsVirtual;
        public readonly string Name;
        public readonly Type MemberType;

        protected MemberDefinition(string name, Type type, bool isVirtual)
        {
            Name = name;
            MemberType = type;
            IsVirtual = isVirtual;
        }

        public abstract void Build(System.Reflection.Emit.TypeBuilder typeBuilder);

        //public abstract MemberInfo GetBuilder();
    }


    class FieldMemberDefinition : MemberDefinition
    {        
        public FieldMemberDefinition(string name, Type type) 
            : base(name, type, false)
        {
        }

        public override void Build(System.Reflection.Emit.TypeBuilder typeBuilder)
        {
            typeBuilder.DefineField(Name, MemberType, FieldAttributes.Public);
        }
    }


    class PropertyMemberDefinition : MemberDefinition
    {
        private readonly MethodInfo _raisePropertyChanged;

        public PropertyMemberDefinition(string name, Type type, bool isVirtual, MethodInfo raisePropertyChanged = null)
            : base(name, type, isVirtual)
        {
            _raisePropertyChanged = raisePropertyChanged;
        }

        public override void Build(System.Reflection.Emit.TypeBuilder typeBuilder)
        {
            var memberName = Name;
            var memberType = MemberType;

            var field = typeBuilder.DefineField("m_" + memberName, memberType, FieldAttributes.Private);

            var methodAttributes = IsVirtual ? Factory.VirtPublicProperty : Factory.PublicProperty;

            // Define the property getter method for our private field.
            var getBuilder = typeBuilder.DefineMethod("get_" + memberName, methodAttributes, memberType, Type.EmptyTypes);

            var getIL = getBuilder.GetILGenerator();
            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Ldfld, field);
            getIL.Emit(OpCodes.Ret);

            var parameterTypes = new[] {memberType};

            // Define the property setter method for our private field.
            var setBuilder = typeBuilder.DefineMethod("set_" + memberName, methodAttributes, null, parameterTypes);

            var setIL = setBuilder.GetILGenerator();

            setIL.Emit(OpCodes.Ldarg_0);

            var notifyChanges = _raisePropertyChanged != null;

            if (notifyChanges)
            {
                setIL.Emit(OpCodes.Ldfld, field);
                setIL.Emit(OpCodes.Ldarg_1);

                var setFieldLabel = setIL.DefineLabel();
                setIL.Emit(OpCodes.Bne_Un_S, setFieldLabel);

                setIL.Emit(OpCodes.Ret);


                setIL.MarkLabel(setFieldLabel);
                setIL.Emit(OpCodes.Ldarg_0);
            }

            setIL.Emit(OpCodes.Ldarg_1);
            setIL.Emit(OpCodes.Stfld, field);

            if (notifyChanges)
            {
                setIL.Emit(OpCodes.Ldarg_0);
                setIL.Emit(OpCodes.Ldstr, memberName);
                setIL.Emit(OpCodes.Call, _raisePropertyChanged);
            }

            setIL.Emit(OpCodes.Ret);

            // Generate a public property
            var property = typeBuilder.DefineProperty(memberName, PropertyAttributes.None, memberType,
                                                      parameterTypes);

            // Map our two methods created above to their corresponding behaviors, "get" and "set" respectively. 
            property.SetGetMethod(getBuilder);
            property.SetSetMethod(setBuilder);
        }
    }


    class MethodMemberDefinition : MemberDefinition
    {
        private readonly Delegate _delegate;

        public MethodMemberDefinition(string name, Type type, bool isVirtual, Delegate @delegate)
            : base(name, type, isVirtual)
        {
            _delegate = @delegate;
        }

        public override void Build(System.Reflection.Emit.TypeBuilder typeBuilder)
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


    class EmptyMethodMemberDefinition : MemberDefinition
    {
        public Type[] ParameterTypes { get; private set; }

        public EmptyMethodMemberDefinition(IMethodCaller caller, bool isVirtual = true)
            : base(caller.Name, caller.ReturnType, isVirtual)
        {
            ParameterTypes = caller.ParameterTypes.Select(x => x.ParameterType).ToArray();
        }

        public EmptyMethodMemberDefinition(string name, Type type, bool isVirtual, Type[] parameterTypes)
            : base(name, type, isVirtual)
        {
            ParameterTypes = parameterTypes;
        }

        public override void Build(System.Reflection.Emit.TypeBuilder typeBuilder)
        {
            var name = Name;
            var returnType = MemberType;
            var parameterTypes = ParameterTypes;

            var methodBuilder = typeBuilder.DefineMethod(name,
                                                         MethodAttributes.Public | MethodAttributes.Final |
                                                         MethodAttributes.HideBySig | MethodAttributes.NewSlot |
                                                         MethodAttributes.Virtual,
                                                         returnType,
                                                         parameterTypes);


            var generator = methodBuilder.GetILGenerator();

            if (returnType != typeof (void))
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


    class EventMemberDefinition : MemberDefinition
    {
        private static readonly MethodInfo _delegateCombine;
        private static readonly MethodInfo _delegateRemove;

        static EventMemberDefinition()
        {
            var delegateType = typeof (Delegate);
            var arguments = new[] {delegateType, delegateType};

            _delegateCombine = TypeInfo<Delegate>.GetSpecificMethod("Combine", arguments).MethodInfo;
            _delegateRemove = TypeInfo<Delegate>.GetSpecificMethod("Remove", arguments).MethodInfo;
        }

        public EventMemberDefinition(string name, Type type)
            : base(name, type, false)
        {
        }

        public override void Build(System.Reflection.Emit.TypeBuilder typeBuilder)
        {
            var eventName = Name;
            var eventHandlerType = MemberType;

            var eventHandlerTypes = new[] {eventHandlerType};

            var eventBackingField = typeBuilder.BuildField(eventName, eventHandlerType, false);

            var voidType = typeof (void);

            //Combine event
            var add = typeBuilder.DefineMethod("add_" + eventName,
                                               Factory.VirtPublicProperty |
                                               MethodAttributes.Final |
                                               MethodAttributes.NewSlot,
                                               voidType,
                                               eventHandlerTypes);

            var generator = add.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, eventBackingField);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, _delegateCombine);
            generator.Emit(OpCodes.Castclass, eventHandlerType);
            generator.Emit(OpCodes.Stfld, eventBackingField);
            generator.Emit(OpCodes.Ret);

            //Remove event
            var remove = typeBuilder.DefineMethod("remove_" + eventName,
                                                  Factory.VirtPublicProperty |
                                                  MethodAttributes.Final |
                                                  MethodAttributes.NewSlot,
                                                  voidType,
                                                  eventHandlerTypes);

            generator = remove.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, eventBackingField);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, _delegateRemove);
            generator.Emit(OpCodes.Castclass, eventHandlerType);
            generator.Emit(OpCodes.Stfld, eventBackingField);
            generator.Emit(OpCodes.Ret);


            //event
            var eventBuilder = typeBuilder.DefineEvent(eventName, EventAttributes.None, eventHandlerType);
            eventBuilder.SetAddOnMethod(add);
            eventBuilder.SetRemoveOnMethod(remove);
        }
    }
}
