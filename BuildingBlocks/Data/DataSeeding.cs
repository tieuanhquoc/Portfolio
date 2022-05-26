using BuildingBlocks.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Data;

public static class DataSeeding
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        var passwordHasher = new PasswordHasher<User>();
        modelBuilder.Entity<User>().HasData(
            // Admin
            new User
            {
                Id = 1,
                Username = "admin",
                Password = passwordHasher.HashPassword(new User(), "123123a@"),
                Avatar = string.Empty,
                Email = "admin@yobmail.com",
                Phone = "0982456789",
                AboutMe = "No thing",
                Address = "Địa chỉ",
                Status = UserStatus.Enabled,
                Role = UserRole.Admin,
                CreatedAt = DateTime.Now
            },
            new User
            {
                Id = 2,
                Username = "user",
                Password = passwordHasher.HashPassword(new User(), "1"),
                Avatar = string.Empty,
                Email = "user@yobmail.com",
                Phone = "0982456799",
                AboutMe = "No thing",
                Address = "Địa chỉ",
                Status = UserStatus.Enabled,
                Role = UserRole.Member,
                CreatedAt = DateTime.Now
            }
        );
    }
}