using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Falsy.NET.Internals.TypeBuilder.Builders
{
    static class Factory
    {
        public static void Build(this System.Reflection.Emit.TypeBuilder typeBuilder, IReadOnlyList<DynamicMember> members, MethodBuilder raisePropertyChanged = null)
        {
            foreach (var member in members)
            {
                switch (member.MemberType)
                {
                    case MemberType.Field:
                        typeBuilder.BuildField(member);
                        break;

                    case MemberType.Property:
                        typeBuilder.BuildProperty(member, raisePropertyChanged);
                        break;

                    case MemberType.Method:
                        typeBuilder.BuildMethod(member);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
