using System;
using System.Reflection;
using Horizon;

namespace Falsy.NET.Internals.TypeBuilder
{
    public abstract class MemberDefinition
    {
        internal const MethodAttributes PublicProperty = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName;
        internal const MethodAttributes VirtPublicProperty = PublicProperty | MethodAttributes.Virtual;


        public readonly bool IsVirtual;
        public readonly string Name;
        public readonly Type MemberType;

        protected MemberDefinition(string name, Type type, bool isVirtual)
        {
            Name = name;
            MemberType = type;
            IsVirtual = isVirtual;
        }

        internal abstract void Build(System.Reflection.Emit.TypeBuilder typeBuilder);




        public static MemberDefinition Field(string name, Type type)
        {
            return new FieldMemberDefinition(name, type);
        }

        public static MemberDefinition Property(string name, Type type, bool isVirtual = true, MethodInfo raisePropertyChanged = null)
        {
            return new PropertyMemberDefinition(name, type, isVirtual, raisePropertyChanged);
        }

        public static MemberDefinition Event(string name, Type type)
        {
            return new EventMemberDefinition(name, type);
        }

        public static MemberDefinition Method(string name, Delegate @delegate, bool isVirtual = true)
        {
            return new MethodMemberDefinition(name, @delegate, isVirtual);
        }

        public static MemberDefinition EmptyMethod(string name, Type returnType, bool isVirtual = true)
        {
            return new EmptyMethodMemberDefinition(name, returnType, isVirtual);
        }

        public static MemberDefinition EmptyMethod(string name, Type returnType, Type[] parameterTypes, bool isVirtual = true)
        {
            return new EmptyMethodMemberDefinition(name, returnType, isVirtual, parameterTypes);
        }

        internal static MemberDefinition EmptyMethod(IMethodCaller caller, bool isVirtual = true)
        {
            return new EmptyMethodMemberDefinition(caller, isVirtual);
        }

        internal static MemberDefinition Property(IPropertyCaller caller, bool isVirtual = true, MethodInfo raisePropertyChanged = null)
        {
            return Property(caller.Name, caller.MemberType, isVirtual, raisePropertyChanged);
        }
    }
}
