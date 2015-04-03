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