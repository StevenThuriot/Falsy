using System;
using System.Reflection;

namespace Falsy.NET.Internals.TypeBuilder
{
    class FieldMemberDefinition : MemberDefinition
    {        
        public FieldMemberDefinition(string name, Type type) 
            : base(name, type, false)
        {
        }

        internal override void Build(System.Reflection.Emit.TypeBuilder typeBuilder)
        {
            typeBuilder.DefineField(Name, MemberType, FieldAttributes.Public);
        }
    }
}