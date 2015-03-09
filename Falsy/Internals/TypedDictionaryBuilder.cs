﻿#region License

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

using System.Collections.Generic;

namespace Falsy.NET.Internals
{
    static class TypedDictionaryBuilder
    {
        public static object Build<TKey, TValue>(IDictionary<TKey, TValue> instance)
        {
            return TypedDictionaryBuilder<TKey, TValue>.Build((dynamic)instance);
        }
    }

    static class TypedDictionaryBuilder<TKey, TValue>
    {
        public static object Build<T>(T instance)
            where T : IDictionary<TKey, TValue>
        {
            return new TypedDictionaryFalsy<T, TKey, TValue>(instance);
        }
    }
}