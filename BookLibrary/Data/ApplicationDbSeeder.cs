using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BookLibrary.Models;

namespace BookLibrary.Data
{
    public class ApplicationDbSeeder
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ApplicationDbContext>>()))
            {
                if (SeedBooks(context))
                {
                    SeedRentals(context);
                }
            }
        }

        private static bool SeedBooks(ApplicationDbContext context)
        {
            if (context.Books.Any())
            {
                return false;
            }

            context.Books.AddRange(Books);
            context.SaveChanges();

            return true;
        }

        private static bool SeedRentals(ApplicationDbContext context)
        {
            if (context.Rentals.Any())
            {
                return false;
            }

            context.Rentals.AddRange(Rentals);
            context.SaveChanges();

            return true;
        }

        public static readonly Book[] Books =
        {
            new Book
            {
                Isbn = "978-83-749-5905-6",
                Title = "Rok 1984",
                Author = "George Orwell",
                Status = Status.Rented
            },
            new Book
            {
                Isbn = "978-83-7648-809-7",
                Title = "Lśnienie",
                Author = "Stephen King",
                Status = Status.Available
            },
            new Book
            {
                Isbn = "978-83-274-3154-7",
                Title = "Mały Książę",
                Author = "Antoine de Saint-Exupéry",
                Status = Status.Available
            }
        };

        public static readonly Rental[] Rentals =
        {
            new Rental
            {
                BookId = 1,
                BeginDate = new DateTime(2021, 03, 01, 12, 00, 00),
                EndDate = new DateTime(2021, 03, 08, 10, 00, 00)
            },
            new Rental
            {
                BookId = 1,
                BeginDate = new DateTime(2021, 03, 30, 08, 00, 00),
                EndDate = null
            },
            new Rental
            {
                BookId = 2,
                BeginDate = new DateTime(2021, 03, 10, 14, 00, 00),
                EndDate = new DateTime(2021, 03, 20, 16, 00, 00)
            }
        };
    }
}
