using System;
using System.Reflection;
using System.Reflection.Emit;
using Horizon;

namespace Falsy.NET.Internals.TypeBuilder.Builders
{
    static class EventFactory
    {
        private static readonly MethodInfo _delegateCombine;
        private static readonly MethodInfo _delegateRemove;

        static EventFactory()
        {
            var delegateType = typeof (Delegate);
            var arguments = new[] {delegateType, delegateType};

            _delegateCombine = TypeInfo<Delegate>.GetSpecificMethod("Combine", arguments).MethodInfo;
            _delegateRemove = TypeInfo<Delegate>.GetSpecificMethod("Remove", arguments).MethodInfo;
        }

        public static Tuple<EventBuilder, FieldBuilder> BuildEvent(this System.Reflection.Emit.TypeBuilder typeBuilder, DynamicMember node)
        {
            var eventName = node.Name;
            var eventHandlerType = node.Type;

            var eventHandlerTypes = new[] { eventHandlerType };

            var eventBackingField = typeBuilder.BuildField(eventName, eventHandlerType, false);

            var voidType = typeof(void);

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

            return Tuple.Create(eventBuilder, eventBackingField);
        }
    }
}
