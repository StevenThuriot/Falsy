using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;

namespace Falsy.NET.Internals.TypeBuilder
{
    public abstract class TypeFactory : DynamicObject
    {
        static IReadOnlyList<Member> CreateValueNodes(CallInfo callInfo, IReadOnlyList<object> args)
        {
            var result = new List<Member>();

            int difference;
            var nameCount = callInfo.ArgumentNames.Count;
            var argumentCount = args.Count;
            if (argumentCount != nameCount)
            {
                if (nameCount != args.Count(x => !(x is Member)))
                    throw new NotSupportedException("Unnamed arguments must be MemberDefinition instances.");

                difference = argumentCount - nameCount;
            }
            else
            {
                difference = 0;
            }

            var argumentNames = callInfo.ArgumentNames;

            for (var i = argumentCount - 1; i >= 0; i--)
            {
                var argument = args[i];

                var dynamicMember = argument as Member;
                if (dynamicMember == null)
                {
                    var name = argumentNames[i - difference];
                    dynamic value = argument;
                    dynamicMember = Member.Unknown(name, value);
                }

                result.Add(dynamicMember);
            }

            return result;
        }
        static IReadOnlyList<MemberDefinition> CreateNodes(CallInfo callInfo, IReadOnlyList<object> args)
        {
            var result = new List<MemberDefinition>();

            int difference;
            var nameCount = callInfo.ArgumentNames.Count;
            var argumentCount = args.Count;

            if (argumentCount != nameCount)
            {
                if (nameCount != args.Count(x => !(x is MemberDefinition)))
                    throw new NotSupportedException("Unnamed arguments must be MemberDefinition instances.");

                difference = argumentCount - nameCount;
            }
            else
            {
                difference = 0;
            }

            var argumentNames = callInfo.ArgumentNames;

            for (var i = argumentCount - 1; i >= 0; i--)
            {
                var argument = args[i];

                var dynamicMember = argument as MemberDefinition;
                if (dynamicMember == null)
                {
                    var name = argumentNames[i - difference];

                    var @delegate = argument as Delegate;
                    if (@delegate != null)
                    {
                        dynamicMember = MemberDefinition.Method(name, @delegate);
                    }
                    else
                    {
                        var type = argument as Type;

                        if (type == null)
                            throw new NotSupportedException("Definitions require a type.");

                        if (typeof(Delegate).IsAssignableFrom(type))
                        {
                            dynamicMember = MemberDefinition.EmptyMethod(name, type);
                        }
                        else
                        {
                            dynamicMember = MemberDefinition.Property(name, type);
                        }
                    }
                }

                result.Add(dynamicMember);   
            }

            return result;
        }

        public class NewTypeFactory : TypeFactory
        {
            internal NewTypeFactory() { }

            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                var nodes = CreateValueNodes(binder.CallInfo, args);
                result = DynamicTypeBuilder.CreateTypeInstance(binder.Name, nodes);
                return true;
            }
        }
        public class DefineTypeFactory : TypeFactory
        {
            private HashSet<Type> _interfaces;
            private Type _parent;

            internal DefineTypeFactory() { }

            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                var binderName = binder.Name.ToUpperInvariant();

                if ("WITHINTERFACE" == binderName)
                {
                    var types = args.Cast<Type>();

                    if (_interfaces == null)
                    {
                        _interfaces = new HashSet<Type>(types);
                    }
                    else
                    {
                        foreach (var type in types)
                            _interfaces.Add(type);
                    }
                    
                    result = this;
                    return true;
                }

                if ("NOTIFYCHANGES" == binderName)
                {
                    var propertyChangedInterface = typeof(INotifyPropertyChanged);

                    if (_interfaces == null)
                    {
                        _interfaces = new HashSet<Type>
                                      {
                                          propertyChangedInterface
                                      };
                    }
                    else
                    {
                        _interfaces.Add(propertyChangedInterface);
                    }

                    result = this;
                    return true;
                }

                if ("INHERITFROM" == binderName)
                {
                    if (args.Length != 1)
                        throw new NotSupportedException("You can only have 1 parent.");

                    _parent = (Type) args[0];

                    result = this;
                    return true;
                }

                try
                {
                    var nodes = CreateNodes(binder.CallInfo, args);
                    result = DynamicTypeBuilder.CreateType(binder.Name, nodes, interfaces: _interfaces, parent: _parent);
                }
                finally
                {
                    _interfaces?.Clear();
                    _parent = null;
                }


                return true;
            }
        }
    }
}
