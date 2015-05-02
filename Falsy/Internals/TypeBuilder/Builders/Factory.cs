using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Falsy.NET.Internals.TypeBuilder.Builders
{
    static class Factory
    {
        internal const MethodAttributes PublicProperty = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName;
        internal const MethodAttributes VirtPublicProperty = PublicProperty | MethodAttributes.Virtual;


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
                        var emptyDynamicMethod = member as EmptyDynamicMethod;
                        if (emptyDynamicMethod != null)
                        {
                            typeBuilder.BuildEmptyMethod(emptyDynamicMethod.Name, emptyDynamicMethod.Type, emptyDynamicMethod.ParameterTypes);
                        }
                        else
                        {
                            typeBuilder.BuildMethod(member);
                        }
                        break;

                    case MemberType.Event:
                        typeBuilder.BuildEvent(member);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
