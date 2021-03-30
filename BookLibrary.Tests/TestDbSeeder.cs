using System;
using System.Linq;
using BookLibrary.Data;
using BookLibrary.Models;

namespace BookLibrary.Tests
{
    public class TestDbSeeder
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        public static void SeedBooks(ApplicationDbContext context)
        {
            if (context.Books.Any())
            {
                return;
            }

            context.Books.AddRange(Books);
            context.SaveChanges();
        }

        public static void SeedRentals(ApplicationDbContext context)
        {
            if (context.Rentals.Any())
            {
                return;
            }

            context.Rentals.AddRange(Rentals);
            context.SaveChanges();
        }

        public static readonly Book[] Books =
        {
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
        };

        public static readonly Rental[] Rentals =
{
                new Rental
                {
                    BookId = 1,
                    StartDate = new DateTime(2021, 03, 01, 12, 00, 00),
                    EndDate = new DateTime(2021, 03, 08, 10, 00, 00)
                },
                new Rental
                {
                    BookId = 1,
                    StartDate = new DateTime(2021, 03, 30, 08, 00, 00),
                    EndDate = null
                },
                new Rental
                {
                    BookId = 2,
                    StartDate = new DateTime(2021, 03, 10, 14, 00, 00),
                    EndDate = new DateTime(2021, 03, 20, 16, 00, 00)
                }
        };
    }
}
