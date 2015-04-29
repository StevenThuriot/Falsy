using System;
using Horizon;

namespace Falsy.NET.Internals.TypeBuilder
{
    sealed class DynamicMember<T> : DynamicMember, ICanVisit
    {
        public readonly T Value;

        public DynamicMember(string name, T value, MemberType memberType, bool isVirtual)
            : base(name, typeof(T), memberType, isVirtual)
        {
            Value = value;
        }

        public void Visit(dynamic instance)
        {
            switch (MemberType)
            {
                case MemberType.Field:
                    TypeInfo.SetField(instance, Name, Value);
                    break;

                case MemberType.Property:
                    TypeInfo.SetProperty(instance, Name, Value);
                    break;
            }
        }
    }
}