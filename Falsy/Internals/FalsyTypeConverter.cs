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
using System.ComponentModel;
using System.Globalization;
using Horizon;

namespace Falsy.NET.Internals
{
    public sealed class FalsyTypeConverter : TypeConverter
    {
        private readonly Type _innerType;

        public FalsyTypeConverter(Type type)
        {
            if (!type.IsGenericType) return; // undefined
            
            //Dictionary or normal
            var arguments = type.GetGenericArguments();
            _innerType = arguments[0];
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (_innerType == null)
                return destinationType.IsClass; //undefined, null thus should be a class.

            return _innerType.IsAssignableFrom(destinationType) || Constants.BooleanType == destinationType;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destinationType)
        {
            if (_innerType == null)
                return null; //undefined.
            
            var falsy = (DynamicFalsy) value;

            if (_innerType.IsAssignableFrom(destinationType))
                return falsy.GetValue();

            if (Constants.BooleanType == destinationType)
                return falsy.GetBooleanValue();

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}