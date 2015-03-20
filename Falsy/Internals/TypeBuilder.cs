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

namespace Falsy.NET.Internals
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
            var type = CreateType(typeName, nodes, parent);

            // Now we have our type. Let's create an instance from it:
            object instance = TypeInfo.Create(type);

            foreach (var node in nodes)
                node.Visit(instance);

            return instance;
        }

        internal static Type CreateType(string typeName, IReadOnlyList<DynamicMember> nodes, Type parent = null)
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
                var names = properties.Select(x => x.Name).Union(fields.Select(x => x.Name)).Distinct().ToList();
                members = nodes.Where(x => !names.Contains(x.Name)).ToList();
            }

            //TODO: typeBuilder.AddInterfaceImplementation(repositoryInteface);

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
                    // Define the property getter method for our private field.
                    var getBuilder = typeBuilder.DefineMethod("get_" + memberName, GetSetAttr, memberType, Type.EmptyTypes);

                    // Hard IL stuff
                    var getIL = getBuilder.GetILGenerator();
                    getIL.Emit(OpCodes.Ldarg_0);
                    getIL.Emit(OpCodes.Ldfld, field);
                    getIL.Emit(OpCodes.Ret);

                    var parameterTypes = new[] { memberType };

                    // Define the property setter method for our private field.
                    var setBuilder = typeBuilder.DefineMethod("set_" + memberName, GetSetAttr, null, parameterTypes);

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






        internal class DynamicMember
        {
            public readonly bool IsProperty;
            public readonly string Name;
            private readonly Type _type;

            public DynamicMember(string name, Type type, bool isProperty)
            {
                Name = name;
                _type = type;
                IsProperty = isProperty;
            }

            public Type Type
            {
                get { return _type; }
            }

            public virtual void Visit(dynamic instance) { }


            public static DynamicMember Create<T>(string name, T value, bool isProperty = true)
            {
                return new DynamicMember<T>(name, value, isProperty);
            }
        }

        internal sealed class DynamicMember<T> : DynamicMember
        {
            private readonly T _value;

            public DynamicMember(string name, T value, bool isProperty)
                : base(name, typeof(T), isProperty)
            {
                _value = value;
            }

            public override void Visit(dynamic instance)
            {
                if (IsProperty)
                {
                    TypeInfo.SetProperty(instance, Name, _value);
                }
                else
                {
                    TypeInfo.SetField(instance, Name, _value);
                }
            }
        }
    }
}