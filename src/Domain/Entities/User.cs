using Domain.Entities.Profiles;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities;

public class User
{
    private User() { }

    public User(
        Name name,
        Email email,
        string passwordHash,
        UserRole role,
        string? phone = null,
        string? cpf = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        Phone = phone;
        Cpf = cpf;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public Guid Id { get; private set; }
    public Name Name { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string? Phone { get; private set; }
    public string? Cpf { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public UserRole Role { get; private set; }
    public ProfessionalProfile? ProfessionalProfile { get; private set; }
    public void DisableAccount()
    {
        IsActive = false;
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void ChangeName(Name name)
    {
        Name = name;
    }

    public void ChangeEmail(Email email)
    {
        Email = email;
    }

    public void ChangePhone(string phone)
    {
        Phone = phone;
    }

    public void AttachProfessionalProfile(ProfessionalProfile profile)
    {
        ProfessionalProfile = profile;
    }
}
