namespace ETS.Domain.Contracts
{
    public interface IUserContext
    {
        Guid? UserId { get; }
        string? UserName { get; }
        string? UserEmail { get; }
        string? UserPhone { get; }
    }
}
