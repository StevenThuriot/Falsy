﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Falsy.NET.Internals.TypeBuilder.Builders;
using Horizon;
using TypeInfo = Horizon.TypeInfo;

namespace Falsy.NET.Internals.TypeBuilder
{
    class TypeBuilder
    {
        private const MethodAttributes PublicProperty = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName;
        private const MethodAttributes VirtPublicProperty = PublicProperty | MethodAttributes.Virtual;

        private static readonly Dictionary<string, Type> _typeCache = new Dictionary<string, Type>();
        private static readonly ModuleBuilder _falsyModule;

        static TypeBuilder()
        {
            var guid = Guid.NewGuid().ToString("N");
            var assemblyName = new AssemblyName("Falsy_" + guid);
            var assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            _falsyModule = assemblyBuilder.DefineDynamicModule("FalsyModule_" + guid);
        }

        internal static dynamic CreateTypeInstance(string typeName, IReadOnlyList<DynamicMember> nodes, Type parent = null)
        {
            Type type;
            if (!_typeCache.TryGetValue(typeName, out type))
                throw new NotSupportedException("Unknown Falsy type.");

            // Now we have our type. Let's create an instance from it:
            object instance = TypeInfo.Create(type);

            foreach (var node in nodes.OfType<ICanVisit>())
                node.Visit(instance);

            return instance;
        }

        internal static Type CreateType(string typeName, IReadOnlyList<DynamicMember> nodes, Type parent = null, IEnumerable<Type> interfaces = null)
        {
            //Todo: the nodes need to become builders that can take care of things on their own.

            Type type;
            if (_typeCache.TryGetValue(typeName, out type))
                return type;

            var typeBuilder = _falsyModule.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class);

            IReadOnlyList<DynamicMember> members;

            if (parent == null)
            {
                members = nodes;
            }
            else
            {
                typeBuilder.SetParent(parent);


                var properties = TypeInfo.Extended.Properties(parent);
                var fields = TypeInfo.Extended.Fields(parent);

                var names = properties.Select(x => x.Name).Union(fields.Select(x => x.Name)).ToList();
                members = nodes.Where(x => !names.Contains(x.Name)).ToList();
            }

            var notifyChanges = false;
            MethodBuilder raisePropertyChanged = null;

            if (interfaces != null)
            {
                var interfaceSet = new HashSet<Type>();
                
                foreach (var @interface in interfaces.Where(interfaceSet.Add).SelectMany(x => x.GetInterfaces()))
                    interfaceSet.Add(@interface);

                foreach (var @interface in interfaceSet)
                {
                    typeBuilder.AddInterfaceImplementation(@interface);

                    var properties = TypeInfo.Extended.Properties(@interface).Select(x => new DynamicMember(x, true));
                    members = members.Union(properties).ToList();


                    var events = TypeInfo.Extended.Events(@interface);

                    if (!notifyChanges &&
                        (notifyChanges = typeof(INotifyPropertyChanged).IsAssignableFrom(@interface)))
                    {
                        foreach (var @event in events)
                        {
                            var tuple = GenerateEvent(typeBuilder, @event);
                            if (@event.Name != "PropertyChanged") continue;

                            var eventBuilder = tuple.Item1;
                            var eventBackingField = tuple.Item2;

                            raisePropertyChanged = BuildOnPropertyChanged(typeBuilder, eventBuilder, eventBackingField);
                        }
                    }
                    else
                    {
                        foreach (var @event in events)
                            GenerateEvent(typeBuilder, @event);
                    }


                    foreach (var method in TypeInfo.Extended.Methods(@interface))
                        GenerateMethod(typeBuilder, method);
                }
            }
            
            typeBuilder.Build(members, raisePropertyChanged);

            if (parent != null)
            {
                if (raisePropertyChanged != null) OverrideParentPropertiesForPropertyChanged(typeBuilder, parent, raisePropertyChanged);
                //TODO: Check if parent is abstract and properties need to be implemented.
            }

            // Generate our type and cache it.
            _typeCache[typeName] = type = typeBuilder.CreateType();
            return type;
        }

        private static MethodBuilder BuildOnPropertyChanged(System.Reflection.Emit.TypeBuilder typeBuilder, EventBuilder eventBuilder, FieldInfo eventBackingField)
        {
            var raisePropertyChanged = typeBuilder.DefineMethod("OnPropertyChanged", MethodAttributes.Private, typeof(void), new[] {typeof(string)});
            var generator = raisePropertyChanged.GetILGenerator();
            var returnLabel = generator.DefineLabel();

            generator.DeclareLocal(typeof(PropertyChangedEventHandler));
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, eventBackingField);
            generator.Emit(OpCodes.Stloc_0);
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ldnull);
            generator.Emit(OpCodes.Ceq);
            generator.Emit(OpCodes.Brtrue, returnLabel);

            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Newobj, _createEventArgs.Value);
            generator.Emit(OpCodes.Callvirt, TypeInfo<ProgressChangedEventHandler>.GetMethod("Invoke").First().MethodInfo);

            generator.MarkLabel(returnLabel);
            generator.Emit(OpCodes.Ret);

            eventBuilder.SetRaiseMethod(raisePropertyChanged);
            return raisePropertyChanged;
        }

        private static void OverrideParentPropertiesForPropertyChanged(System.Reflection.Emit.TypeBuilder typeBuilder, Type parent, MethodInfo raiseEvent)
        {
            foreach (var propertyInfo in TypeInfo.Extended.Properties(parent))
            {
                var propertySetter = propertyInfo.GetSetMethod();
                if (propertySetter == null) continue;
                if (!propertySetter.IsVirtual) continue;

                var name = propertyInfo.Name;
                var pb = typeBuilder.DefineProperty(name, PropertyAttributes.None, propertyInfo.MemberType, Type.EmptyTypes);

                ILGenerator generator;

                var propertyGetter = propertyInfo.GetGetMethod();
                if (propertyGetter != null)
                {
                    var getMethod = typeBuilder.DefineMethod("get_" + name, VirtPublicProperty, propertyInfo.MemberType, Type.EmptyTypes);
                    generator = getMethod.GetILGenerator();
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Call, propertyGetter);
                    generator.Emit(OpCodes.Ret);

                    pb.SetGetMethod(getMethod);
                }
                
                
                var setMethod = typeBuilder.DefineMethod("set_" + name, VirtPublicProperty, null, new[] {propertyInfo.MemberType});

                generator = setMethod.GetILGenerator();
                generator.Emit(OpCodes.Ldarg_0);

                if (propertyGetter != null)
                {
                    generator.Emit(OpCodes.Call, propertyGetter);
                    generator.Emit(OpCodes.Ldarg_1);
                    var setValueLabel = generator.DefineLabel();
                    generator.Emit(OpCodes.Bne_Un_S, setValueLabel);
                    generator.Emit(OpCodes.Ret);
                    generator.MarkLabel(setValueLabel);
                    generator.Emit(OpCodes.Ldarg_0);
                }

                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Call, propertySetter);
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldstr, name);
                generator.Emit(OpCodes.Call, raiseEvent);

                generator.Emit(OpCodes.Ret);

                pb.SetSetMethod(setMethod);
            }
        }


        private static readonly Lazy<MethodInfo> _delegateCombine = new Lazy<MethodInfo>(() =>
        {
            var delegateType = typeof(Delegate);
            return TypeInfo<Delegate>.GetSpecificMethod("Combine", new []{ delegateType, delegateType }).MethodInfo;
        });
        
        
        private static readonly Lazy<MethodInfo> _delegateRemove = new Lazy<MethodInfo>(() =>
        {
            var delegateType = typeof(Delegate);
            return TypeInfo<Delegate>.GetSpecificMethod("Remove", new []{ delegateType, delegateType }).MethodInfo;
        });
        
        private static readonly Lazy<ConstructorInfo> _createEventArgs = new Lazy<ConstructorInfo>(() => TypeInfo<PropertyChangingEventArgs>.GetConstructor(typeof(string)).ConstructorInfo);


        private static void GenerateMethod(System.Reflection.Emit.TypeBuilder typeBuilder, IMethodCaller methodInfo)
        {
            var name = methodInfo.Name;
            var returnType = methodInfo.ReturnType;
            var parameterTypes = methodInfo.ParameterTypes.Select(x => x.ParameterType).ToArray();

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

        private static Tuple<EventBuilder, FieldBuilder> GenerateEvent(System.Reflection.Emit.TypeBuilder typeBuilder, IEventCaller @event)
        {
            var eventName = @event.Name;
            var eventHandlerType = @event.EventHandlerType;
            var eventHandlerTypes = new[] { eventHandlerType };

            var eventBackingField = typeBuilder.DefineField("m_" + eventName, eventHandlerType, FieldAttributes.Private);

            var voidType = typeof(void);

            //Combine event
            var add = typeBuilder.DefineMethod("add_" + eventName,
                                               VirtPublicProperty |
                                               MethodAttributes.Final |
                                               MethodAttributes.NewSlot,
                                               voidType,
                                               eventHandlerTypes);

            var generator = add.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, eventBackingField);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, _delegateCombine.Value);
            generator.Emit(OpCodes.Castclass, eventHandlerType);
            generator.Emit(OpCodes.Stfld, eventBackingField);
            generator.Emit(OpCodes.Ret);

            //Remove event
            var remove = typeBuilder.DefineMethod("remove_" + eventName,
                                                  VirtPublicProperty |
                                                  MethodAttributes.Final |
                                                  MethodAttributes.NewSlot,
                                                  voidType,
                                                  eventHandlerTypes);

            generator = remove.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, eventBackingField);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, _delegateRemove.Value);
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