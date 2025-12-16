using Domain.Entities;
using Domain.Entities.Profiles;
using Domain.Enums;
using Domain.ValueObjects;
using Domain.Interfaces.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Infrastructure.Configurations;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<NeuroFlowContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        if (await context.Users.AnyAsync())
            return;

        var passwordHash = passwordHasher.Hash("123456");

        var user = new User(
            name: new Name("Thiago", "Admin"),
            email: new Email("admin@neuroflow.com"),
            passwordHash: passwordHash,
            role: UserRole.Professional,
            phone: "24999999999"
        );

        var address = new Address(
            street: "Rua Central",
            number: 100,
            city: "Volta Redonda",
            state: "RJ",
            zipCode: "27200-000",
            country:"Brazil"
        );

        var profile = new ProfessionalProfile(
            userId: user.Id,
            documentNumber: "CRP-12345",
            speciality: "Psicologia Clínica",
            address: address
        );

        user.AttachProfessionalProfile(profile);

        context.Users.Add(user);
        await context.SaveChangesAsync();
    }
}
