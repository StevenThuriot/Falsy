namespace Falsy.Tests
{
    public class Parent
    {
        public int Test3_Field;
        internal const int ImAVirtualGetterValue = 2;
        

        public virtual int ImAVirtualGetterSetter { get; set; }

        
        public virtual int ImAVirtualGetter
        {
            get { return ImAVirtualGetterValue; }
        }

        public virtual int ImAVirtualSetter
        {
            set { Test3_Field = value; }
        }

        public int ImANormalGetterSetter { get; set; }
    }
}
