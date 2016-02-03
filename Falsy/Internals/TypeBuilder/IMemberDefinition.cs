using System;

namespace Falsy.NET.Internals.TypeBuilder
{
    public interface IMemberDefinition
    {
        bool IsVirtual { get; }
        string Name { get; }
        Type MemberType { get; }
        void Build(System.Reflection.Emit.TypeBuilder typeBuilder);
    }
}
