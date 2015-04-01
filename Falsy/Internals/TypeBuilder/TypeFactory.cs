using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using Horizon;

namespace Falsy.NET.Internals.TypeBuilder
{
    public abstract class TypeFactory : DynamicObject
    {
        static IReadOnlyList<DynamicMember> CreateNodes(CallInfo callInfo, IReadOnlyList<object> args, bool objectsAreValues)
        {
            var result = new List<DynamicMember>();

            if (callInfo.ArgumentCount != args.Count)
                throw new NotSupportedException("All arguments must be named.");

            var argumentNames = callInfo.ArgumentNames;

            for (var i = 0; i < argumentNames.Count; i++)
            {
                var argument = args[i];

                var dynamicMember = argument as DynamicMember;
                if (dynamicMember == null)
                {
                    var name = argumentNames[i];
                    if (objectsAreValues)
                    {
                        dynamicMember = DynamicMember.Create(name, (dynamic) argument, true, false);
                    }
                    else
                    {
                        var type = argument as Type;
                        if (type == null)
                            throw new NotSupportedException("Definitions require a type.");

                        dynamicMember = new DynamicMember(name, type, true, false);
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
                var nodes = CreateNodes(binder.CallInfo, args, objectsAreValues: true);
                result = TypeBuilder.CreateTypeInstance(binder.Name, nodes);
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
                    var propertyChangedInterface = Constants.Typed<INotifyPropertyChanged>.OwnerType;

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

                var nodes = CreateNodes(binder.CallInfo, args, objectsAreValues: false);
                result = TypeBuilder.CreateType(binder.Name, nodes, interfaces: _interfaces, parent: _parent);
                return true;
            }
        }
    }
}
