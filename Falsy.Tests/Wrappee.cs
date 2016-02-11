namespace Falsy.Tests
{
    public class Wrappee
    {
        public int GetNumber() => 5;
        public string Name { get; set; }
    }
    
    public interface IWrapper
    {
        int GetNumber();
        string Name { get; set; }
    }
}
