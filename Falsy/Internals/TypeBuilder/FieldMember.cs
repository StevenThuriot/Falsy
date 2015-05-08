﻿using Horizon;

namespace Falsy.NET.Internals.TypeBuilder
{
    class FieldMember<T> : Member<T>
    {
        public FieldMember(string name, T value) : base(name, value)
        {
        }

        internal override void SetValue(dynamic instance)
        {
            Info.SetField(instance, Name, Value);
        }
    }
}