using Horizon;

namespace Falsy.NET.Internals.TypeBuilder
{
    class UnknownMember<T> : Member<T>
    {
        public UnknownMember(string name, T value)
            : base(name, value)
        {
        }

        internal override void SetValue(dynamic instance)
        {
            Info.TrySetValue(instance, Name, Value);
        }
    }
}