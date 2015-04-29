using System;
using Horizon;

namespace Falsy.NET.Internals.TypeBuilder
{
    class DynamicMember
    {
        public readonly MemberType MemberType;
        public readonly bool IsVirtual;
        public readonly string Name;
        private readonly Type _type;

        public DynamicMember(string name, Type type, MemberType memberType, bool isVirtual)
        {
            Name = name;
            _type = type;
            MemberType = memberType;
            IsVirtual = isVirtual;
        }

        public DynamicMember(IMemberCaller info, bool isVirtual)
        {
            Name = info.Name;
            _type = info.MemberType;
            MemberType = info.IsProperty ? MemberType.Property : MemberType.Field;
            IsVirtual = isVirtual;
        }

        public Type Type
        {
            get { return _type; }
        }

        public static DynamicMember Create<T>(string name, T value, bool isVirtual = false)
        {
            var memberType = value is Delegate ? MemberType.Method : MemberType.Property;
            return new DynamicMember<T>(name, value, memberType, isVirtual);
        }

        public static DynamicMember Create<T>(string name, T value, MemberType memberType, bool isVirtual = false)
        {
            return new DynamicMember<T>(name, value, memberType, isVirtual);
        }
    }
}