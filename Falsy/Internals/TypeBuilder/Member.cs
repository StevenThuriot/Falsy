using Horizon;

namespace Falsy.NET.Internals.TypeBuilder
{
    public abstract class Member
    {
        public readonly string Name;

        protected Member(string name)
        {
            Name = name;
        }

        public static Member Field<T>(string name, T value)
        {
            return new FieldMember<T>(name, value);
        }

        public static Member Property<T>(string name, T value)
        {
            return new PropertyMember<T>(name, value);
        }

        public static Member Unknown<T>(string name, T value)
        {
            return new UnknownMember<T>(name, value);
        }

        public abstract void SetValue(dynamic instance);
    }

    public abstract class Member<T> : Member
    {
        public readonly T Value;
        
        protected Member(string name, T value)
            : base(name)
        {
            Value = value;
        }
    }


    class FieldMember<T> : Member<T>
    {
        public FieldMember(string name, T value) : base(name, value)
        {
        }

        public override void SetValue(dynamic instance)
        {
            Info.SetField(instance, Name, Value);
        }
    }


    class PropertyMember<T> : Member<T>
    {
        public PropertyMember(string name, T value) : base(name, value)
        {
        }

        public override void SetValue(dynamic instance)
        {
            Info.SetProperty(instance, Name, Value);
        }
    }


    class UnknownMember<T> : Member<T>
    {
        public UnknownMember(string name, T value)
            : base(name, value)
        {
        }

        public override void SetValue(dynamic instance)
        {
            Info.TrySetValue(instance, Name, Value);
        }
    }
}
