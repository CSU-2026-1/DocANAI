using DocANAI.Persistence.Abstractions;
using DocANAI.Persistence.ValueObjects;

namespace DocANAI.Persistence.Entities.User;

public sealed class User : AuditableEntity<IdOf<User>>
{
    public string Username { get; private set; }
    public string PasswordHash { get; private set; }

    public UserType UserType { get; private set; }

    private User(IdOf<User> id, string username, string passwordHash, UserType userType)
    {
        Id = id;
        Username = username;
        PasswordHash = passwordHash;
        UserType = userType;
    }
    
    public static User Create(
        IdOf<User> id, 
        string username, 
        string passwordHash, 
        UserType userType = UserType.Basic)
        => new(id, username, passwordHash, userType);

    public void ChangeUserPassword(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    public void ChangeUserType(UserType userType)
    {
        UserType = userType;
    }
}