namespace Falsy.Tests
{
    public interface IPerson
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        int Age { get; set; }
    }    
    
    public interface IPersonWithMethods : IPerson
    {
        int ThisIsAMethod();
    }
}