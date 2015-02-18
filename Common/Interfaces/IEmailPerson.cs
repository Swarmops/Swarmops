namespace Swarmops.Common.Interfaces
{
    public interface IEmailPerson : IHasIdentity
    {
        string Email { get; }
        string Name { get; }
    }
}