using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BookLibrary.Models;

namespace BookLibrary.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.Books.Any())
                {
                    return;   // DB has been seeded
                }

                context.Books.AddRange(
                    new Book
                    {
                        Isbn = "978-83-749-5905-6",
                        Title = "Rok 1984",
                        Author = "George Orwell"
                    },
                    new Book
                    {
                        Isbn = "978-83-7648-809-7",
                        Title = "Lśnienie",
                        Author = "Stephen King"
                    },
                    new Book
                    {
                        Isbn = "978-83-274-3154-7",
                        Title = "Mały Książę",
                        Author = "Antoine de Saint-Exupéry"
                    }
                );

                context.SaveChanges();
            }
        }
    }
}
