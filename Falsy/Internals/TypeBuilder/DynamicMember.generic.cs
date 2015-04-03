using Horizon;

namespace Falsy.NET.Internals.TypeBuilder
{
    sealed class DynamicMember<T> : DynamicMember, ICanVisit
    {
        private readonly T _value;

        public DynamicMember(string name, T value, bool isProperty, bool isVirtual)
            : base(name, typeof (T), isProperty, isVirtual)
        {
            _value = value;
        }

        public void Visit(dynamic instance)
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