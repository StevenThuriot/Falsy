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

using System.Collections;
using Falsy.NET.Internals;
using Horizon;

namespace Falsy.NET
{
    public static class Falsy
    {
        public static readonly dynamic undefined = UndefinedFalsy.Value;


        public static dynamic Falsify(this object instance)
        {
            return Reference.IsNull(instance)
                ? undefined
                //Resolve actual type through the DLR
                : InternalFalsify((dynamic) instance);
        }

        public static dynamic Falsify(this IEnumerable instance)
        {
            return Reference.IsNull(instance)
                ? undefined
                //Resolve actual type through the DLR
                : InternalEnumerableFalsify((dynamic) instance);
        }

        public static dynamic Falsify(this IDictionary instance)
        {
            return Reference.IsNull(instance)
                ? undefined
                //Resolve actual type through the DLR
                : InternalDictionaryFalsify((dynamic)instance);
        }

        public static dynamic Falsify(this DynamicFalsy instance)
        {
            return Reference.IsNull(instance)
                ? undefined
                : instance;
        }


        private static dynamic InternalFalsify<T>(T instance)
        {
            return new DynamicFalsy<T>(instance);
        }

        private static dynamic InternalEnumerableFalsify<T>(T instance)
            where T : IEnumerable
        {
            return new EnumerableFalsy<T>(instance);
        }

        private static dynamic InternalDictionaryFalsify<T>(T instance)
            where T : IDictionary
        {
            if (Constants.Typed<T>.IsGenericDictionary)
                return TypedDictionaryBuilder.Build((dynamic) instance);

            return new DictionaryFalsy<T>(instance);
        }




        private static readonly TypeFactory _newFactory = new TypeFactory.NewTypeFactory();
        private static readonly TypeFactory _defineFactory = new TypeFactory.DefineTypeFactory();
        public static dynamic Define
        {
            get { return _defineFactory; }
        }
        public static dynamic New
        {
            get { return _newFactory; }
        }



    }
}