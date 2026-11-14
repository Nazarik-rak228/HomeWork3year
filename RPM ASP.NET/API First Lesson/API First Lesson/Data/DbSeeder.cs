using API_First_Lesson.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API_First_Lesson.Data;

public static class DbSeeder
{
    public static void Seed(WebAppContext db)
    {
        if (!db.Roles.Any())
        {
            var roles = new[]
            {
                new Role { RoleId = 1, RoleName = "Admin" },
                new Role { RoleId = 2, RoleName = "Customer" },
                new Role { RoleId = 3, RoleName = "Manager" }
            };

            if (db.Database.IsSqlServer())
            {
                // SQL Server не разрешает явные значения в IDENTITY-колонке без IDENTITY_INSERT.
                // Открываем соединение явно, чтобы SET и INSERT выполнялись в одной сессии.
                db.Database.OpenConnection();
                try
                {
                    db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Roles] ON");
                    db.Roles.AddRange(roles);
                    db.SaveChanges();
                    db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Roles] OFF");
                }
                finally
                {
                    db.Database.CloseConnection();
                }
            }
            else
            {
                db.Roles.AddRange(roles);
                db.SaveChanges();
            }
        }

        if (!db.Users.Any(u => u.Username == "admin"))
        {
            var admin = new User
            {
                Username = "admin",
                RoleId = 1,
                IsActive = true
            };
            admin.PasswordHash = new PasswordHasher<User>().HashPassword(admin, "Admin123!");
            db.Users.Add(admin);
            db.SaveChanges();
        }

        if (!db.Categories.Any())
        {
            var category = new Category { CategoryName = "Без категории" };
            db.Categories.Add(category);
            db.SaveChanges();

            db.Products.AddRange(
                new Product { ProductName = "Товар 1", Discription = "Тестовый товар", Price = 199.99m, Stock = 10, CategoryId = category.CategoryId },
                new Product { ProductName = "Товар 2", Discription = "Тестовый товар", Price = 349.50m, Stock = 5, CategoryId = category.CategoryId });
            db.SaveChanges();
        }
    }
}
