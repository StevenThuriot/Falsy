#region License

//  
// Copyright 2015 Steven Thuriot
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using TypeInfo = Horizon.TypeInfo;

namespace Falsy.NET.Internals.TypeBuilder
{
    /*
     * Goal:
               
     Falsy.Define
          .WithInterface(type) //Optional
          .Person(
              FirstName: typeof(string),
              LastName: typeof(string),
              Age: new DynamicMember("Age", isProperty: false), //For special cases with extra configuration options
           );
     
     Falsy.New
          .Person(                      //If person does not exist, build it from the passed properties?
               FirstName: "Steven",     //DynamicMember.Create("FirstName", "Steven", isProperty: true)
               LastName: "Thuriot",
               Age: 28
           );

     */

    class TypeBuilder
    {
        private const MethodAttributes GetSetAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName;
        private const MethodAttributes VirtGetSetAttr = GetSetAttr | MethodAttributes.Virtual;
        
        private static readonly Dictionary<string, Type> _typeCache = new Dictionary<string, Type>();
        private static readonly ModuleBuilder _falsyModule;

        static TypeBuilder()
        {
            var guid = Guid.NewGuid().ToString("N");
            var assemblyName = new AssemblyName("Falsy_" + guid);
            var assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            _falsyModule = assemblyBuilder.DefineDynamicModule("FalsyModule_" + guid);
        }

        internal static dynamic CreateTypeInstance(string typeName, IReadOnlyList<DynamicMember> nodes, Type parent = null)
        {
            Type type;
            if (!_typeCache.TryGetValue(typeName, out type))
                throw new NotSupportedException("Unknown Falsy type.");

            // Now we have our type. Let's create an instance from it:
            object instance = TypeInfo.Create(type);

            foreach (var node in nodes.OfType<ICanVisit>())
                node.Visit(instance);

            return instance;
        }

        internal static Type CreateType(string typeName, IReadOnlyList<DynamicMember> nodes, Type parent = null, IEnumerable<Type> interfaces = null)
        {
            Type type;
            if (_typeCache.TryGetValue(typeName, out type))
                return type;

            var typeBuilder = _falsyModule.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class);
            
            IReadOnlyList<DynamicMember> members;

            if (parent == null)
            {
                members = nodes;
            }
            else
            {
                typeBuilder.SetParent(parent);
                var properties = parent.GetProperties();
                var fields = parent.GetFields();
                var names = properties.Select(x => x.Name).Union(fields.Select(x => x.Name)).ToList();
                members = nodes.Where(x => !names.Contains(x.Name)).ToList();
            }

            if (interfaces != null)
            {
                foreach (var @interface in interfaces)
                {
                    typeBuilder.AddInterfaceImplementation(@interface);

                    var properties = @interface.GetProperties().Select(x => new DynamicMember(x, isVirtual: true));
                    members = members.Union(properties).ToList();
                }
            }

            foreach (var node in members)
            {
                var memberName = node.Name;
                var memberType = node.Type;
                var isProperty = node.IsProperty;

                var fieldName = memberName;
                FieldAttributes fieldAttributes;

                if (isProperty)
                {
                    fieldName = "m_" + fieldName;
                    fieldAttributes = FieldAttributes.Private;
                }
                else
                {
                    fieldAttributes = FieldAttributes.Public;
                }

                // Generate a private field
                var field = typeBuilder.DefineField(fieldName, memberType, fieldAttributes);

                if (isProperty)
                {
                    var methodAttributes = node.IsVirtual ? VirtGetSetAttr : GetSetAttr;

                    // Define the property getter method for our private field.
                    var getBuilder = typeBuilder.DefineMethod("get_" + memberName, methodAttributes, memberType, Type.EmptyTypes);

                    // Hard IL stuff
                    var getIL = getBuilder.GetILGenerator();
                    getIL.Emit(OpCodes.Ldarg_0);
                    getIL.Emit(OpCodes.Ldfld, field);
                    getIL.Emit(OpCodes.Ret);

                    var parameterTypes = new[] { memberType };

                    // Define the property setter method for our private field.
                    var setBuilder = typeBuilder.DefineMethod("set_" + memberName, methodAttributes, null, parameterTypes);

                    // Hard IL stuff
                    var setIL = setBuilder.GetILGenerator();
                    setIL.Emit(OpCodes.Ldarg_0);
                    setIL.Emit(OpCodes.Ldarg_1);
                    setIL.Emit(OpCodes.Stfld, field);
                    setIL.Emit(OpCodes.Ret);

                    // Generate a public property
                    var property = typeBuilder.DefineProperty(memberName, PropertyAttributes.None, memberType, parameterTypes);

                    // Map our two methods created above to their corresponding behaviors, "get" and "set" respectively. 
                    property.SetGetMethod(getBuilder);
                    property.SetSetMethod(setBuilder);
                }
            }

            // Generate our type and cache it.
            _typeCache[typeName] = type = typeBuilder.CreateType();
            return type;
        }
    }
}