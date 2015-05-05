using System;
using System.Linq;
using Falsy.NET.Internals.TypeBuilder.Builders;
using Horizon;

namespace Falsy.NET.Internals.TypeBuilder
{
    abstract class MemberDefinition
    {
        public bool IsVirtual { get; protected set; }
        public string Name { get; protected set; }
        public Type MemberType { get; protected set; }

        protected MemberDefinition(string name, Type type, bool isVirtual)
        {
            Name = name;
            MemberType = type;
            IsVirtual = isVirtual;
        }

        public abstract void Build(System.Reflection.Emit.TypeBuilder typeBuilder);
    }


    class FieldMemberDefinition : MemberDefinition
    {
        public FieldMemberDefinition(string name, Type type) 
            : base(name, type, false)
        {
        }

        public override void Build(System.Reflection.Emit.TypeBuilder typeBuilder)
        {
            //typeBuilder.BuildField(this);
        }
    }


    class PropertyMemberDefinition : MemberDefinition
    {
        public PropertyMemberDefinition(string name, Type type, bool isVirtual) 
            : base(name, type, isVirtual)
        {
        }

        public override void Build(System.Reflection.Emit.TypeBuilder typeBuilder)
        {
            //typeBuilder.BuildProperty(this, raisePropertyChanged);
        }
    }


    class MethodMemberDefinition : MemberDefinition
    {
        public MethodMemberDefinition(string name, Type type, bool isVirtual) 
            : base(name, type, isVirtual)
        {
        }

        public override void Build(System.Reflection.Emit.TypeBuilder typeBuilder)
        {
            //typeBuilder.BuildMethod(this);
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
            typeBuilder.BuildEmptyMethod(Name, MemberType, ParameterTypes);
        }
    }


    class EventMemberDefinition : MemberDefinition
    {
        public EventMemberDefinition(string name, Type type) 
            : base(name, type, false)
        {
        }

        public override void Build(System.Reflection.Emit.TypeBuilder typeBuilder)
        {
            //typeBuilder.BuildEvent(this);
        }
    }
}
