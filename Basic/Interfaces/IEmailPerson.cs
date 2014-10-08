namespace Swarmops.Basic.Interfaces
{
    public interface IEmailPerson : IHasIdentity
    {
        string Email { get; }
        string Name { get; }
    }
}