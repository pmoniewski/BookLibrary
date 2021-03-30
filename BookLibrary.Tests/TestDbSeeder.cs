using System;
using BookLibrary.Data;
using BookLibrary.Models;

namespace BookLibrary.Tests
{
    public class TestDbSeeder
    {
        public TestDbSeeder()
        {
        }

        public void Seed(ApplicationDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

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
