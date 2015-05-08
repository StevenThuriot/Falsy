using Horizon;

namespace Falsy.NET.Internals.TypeBuilder
{
    class PropertyMember<T> : Member<T>
    {
        public PropertyMember(string name, T value) : base(name, value)
        {
        }

        internal override void SetValue(dynamic instance)
        {
            Info.SetProperty(instance, Name, Value);
        }
    }
}