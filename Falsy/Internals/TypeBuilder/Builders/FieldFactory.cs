using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Falsy.NET.Internals.TypeBuilder.Builders
{
    static class FieldFactory
    {
        public static FieldBuilder BuildField(this System.Reflection.Emit.TypeBuilder typeBuilder, DynamicMember node)
        {
            var memberType = node.Type;
            var fieldName = node.Name;

            FieldAttributes fieldAttributes;

            if (node.MemberType == MemberType.Property)
            {
                fieldName = "m_" + fieldName;
                fieldAttributes = FieldAttributes.Private;
            }
            else
            {
                fieldAttributes = FieldAttributes.Public;
            }

            // Generate a field
            var field = typeBuilder.DefineField(fieldName, memberType, fieldAttributes);
            return field;
        }

        public static FieldBuilder BuildField(this System.Reflection.Emit.TypeBuilder typeBuilder, string name, Type type, bool isPublic)
        {
            var fieldAttributes = isPublic ? FieldAttributes.Public : FieldAttributes.Private;

            // Generate a field
            var field = typeBuilder.DefineField(name, type, fieldAttributes);
            return field;
        }
    }
}
