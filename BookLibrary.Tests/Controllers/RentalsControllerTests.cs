using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Xunit;
using BookLibrary.Controllers;
using BookLibrary.Data;
using BookLibrary.Models;
using BookLibrary.ViewModels;
using BookLibrary.Services;

namespace BookLibrary.Tests.Controllers
{
    [Collection("TestCollection")]
    public class RentalsControllerTests
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly RentalsController _controller;

        public RentalsControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();

            var mockDbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;
            _db = new ApplicationDbContext(mockDbContextOptions);

            var mockMapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            _mapper = mockMapperConfiguration.CreateMapper();

            _controller = new RentalsController(_db, _mapper);
        }

        [Fact]
        public async Task IndexOnGet_WhenEntitiesExist_ReturnsViewResult()
        {
            //Arrange
            TestDbSeeder.Initialize(_db);
            TestDbSeeder.SeedBooks(_db);
            TestDbSeeder.SeedRentals(_db);

            //Act
            var result = await _controller.Index();

            //Assert
            var books = await _db.Books.Include(b => b.Rentals).ToListAsync();
            Assert.NotNull(books);
            Assert.Equal(TestDbSeeder.Books.Length, books.Count);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<BookViewModel>>(viewResult.Model);
            Assert.Equal(books.Count, model.Count());
        }

        [Fact]
        public async Task IndexOnGet_WhenEntitiesDoNotExist_ReturnsViewResult()
        {
            //Arrange
            TestDbSeeder.Initialize(_db);

            //Act
            var result = await _controller.Index();

            //Assert
            var books = await _db.Books.Include(b => b.Rentals).ToListAsync();
            Assert.NotNull(books);
            Assert.Empty(books);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<BookViewModel>>(viewResult.Model);
            Assert.Empty(model);
        }

        [Fact]
        public async Task AvailableBooksOnGet_WhenEntitiesExist_ReturnsViewResult()
        {
            //Arrange
            TestDbSeeder.Initialize(_db);
            TestDbSeeder.SeedBooks(_db);
            TestDbSeeder.SeedRentals(_db);

            //Act
            var result = await _controller.AvailableBooks();

            //Assert
            var books = await _db.Books.Include(b => b.Rentals).ToListAsync();
            Assert.NotNull(books);
            Assert.Equal(TestDbSeeder.Books.Length, books.Count);

            var rentals = await _db.Rentals.Include(r => r.Book).ToListAsync();
            Assert.NotNull(rentals);
            Assert.Equal(TestDbSeeder.Rentals.Length, books.Count);

            var availableBooks = books.Where(b => b.Status == Status.Available).ToList();
            Assert.NotNull(availableBooks);
            Assert.Equal(2, availableBooks.Count);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<BookViewModel>>(viewResult.Model);
            Assert.Equal(availableBooks.Count, model.Count());
        }

        [Fact]
        public async Task AvailableBooksOnGet_WhenEntitiesDoNotExist_ReturnsViewResult()
        {
            //Arrange
            TestDbSeeder.Initialize(_db);

            //Act
            var result = await _controller.AvailableBooks();

            //Assert
            var books = await _db.Books.Include(b => b.Rentals).ToListAsync();
            Assert.NotNull(books);
            Assert.Empty(books);

            var rentals = await _db.Rentals.Include(r => r.Book).ToListAsync();
            Assert.NotNull(rentals);
            Assert.Empty(rentals);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<BookViewModel>>(viewResult.Model);
            Assert.Empty(model);
        }

        [Fact]
        public async Task RentedBooksOnGet_WhenEntitiesExist_ReturnsViewResult()
        {
            //Arrange
            TestDbSeeder.Initialize(_db);
            TestDbSeeder.SeedBooks(_db);
            TestDbSeeder.SeedRentals(_db);

            //Act
            var result = await _controller.RentedBooks();

            //Assert
            var books = await _db.Books.Include(b => b.Rentals).ToListAsync();
            Assert.NotNull(books);
            Assert.Equal(TestDbSeeder.Books.Length, books.Count);

            var rentals = await _db.Rentals.Include(r => r.Book).ToListAsync();
            Assert.NotNull(rentals);
            Assert.Equal(TestDbSeeder.Rentals.Length, books.Count);

            var rentedBooks = books.Where(b => b.Status == Status.Rented).ToList();
            Assert.NotNull(rentedBooks);
            Assert.Single(rentedBooks);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<BookViewModel>>(viewResult.Model);
            Assert.Equal(rentedBooks.Count, model.Count());
        }

        [Fact]
        public async Task RentedBooksOnGet_WhenEntitiesDoNotExist_ReturnsViewResult()
        {
            //Arrange
            TestDbSeeder.Initialize(_db);

            //Act
            var result = await _controller.RentedBooks();

            //Assert
            var books = await _db.Books.Include(b => b.Rentals).ToListAsync();
            Assert.NotNull(books);
            Assert.Empty(books);

            var rentals = await _db.Rentals.Include(r => r.Book).ToListAsync();
            Assert.NotNull(rentals);
            Assert.Empty(rentals);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<BookViewModel>>(viewResult.Model);
            Assert.Empty(model);
        }

        [Fact]
        public async Task StartRentalOnGet_WhenEntityExistsAndIsAvailable_ReturnsRedirectToIndex()
        {
            //Arrange
            TestDbSeeder.Initialize(_db);
            TestDbSeeder.SeedBooks(_db);
            TestDbSeeder.SeedRentals(_db);

            //Act
            var bookId = 2;
            var result = await _controller.StartRental(bookId);

            //Assert
            var book = await _db.Books.Include(b => b.Rentals).FirstOrDefaultAsync(b => b.Id == bookId);
            Assert.NotNull(book);
            Assert.Equal(Status.Rented, book.Status);

            var lastRental = await _db.Rentals
                .Where(r => r.BookId == bookId)
                .OrderByDescending(r => r.BeginDate)
                .FirstOrDefaultAsync();
            Assert.NotNull(lastRental);
            Assert.Null(lastRental.EndDate);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal(nameof(RentalsController.RentedBooks), redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task StartRentalOnGet_WhenEntityExistsAndIsNotAvailable_ReturnsBadRequestObjectResult()
        {
            //Arrange
            TestDbSeeder.Initialize(_db);
            TestDbSeeder.SeedBooks(_db);
            TestDbSeeder.SeedRentals(_db);

            //Act
            var bookId = 1;
            var result = await _controller.StartRental(bookId);

            //Assert
            var book = await _db.Books.Include(b => b.Rentals).FirstOrDefaultAsync(b => b.Id == bookId);
            Assert.NotNull(book);
            Assert.NotEqual(Status.Available, book.Status);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(405, objectResult.StatusCode);
            Assert.Equal("Method call is invalid.", objectResult.Value);
        }

        [Fact]
        public async Task StartRentalOnGet_WhenEntityDoesNotExist_ReturnsNotFoundObjectResult()
        {
            //Arrange
            TestDbSeeder.Initialize(_db);

            //Act
            var bookId = 1;
            var result = await _controller.StartRental(bookId);

            //Assert
            var book = await _db.Books.Include(b => b.Rentals).FirstOrDefaultAsync(b => b.Id == bookId);
            Assert.Null(book);

            var objectResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Entity does not exist.", objectResult.Value);
        }

        [Fact]
        public async Task FinishRentalOnGet_WhenEntityExistsAndIsRented_ReturnsRedirectToIndex()
        {
            //Arrange
            TestDbSeeder.Initialize(_db);
            TestDbSeeder.SeedBooks(_db);
            TestDbSeeder.SeedRentals(_db);

            //Act
            var bookId = 1;
            var result = await _controller.FinishRental(bookId);

            //Assert
            var book = await _db.Books.Include(b => b.Rentals).FirstOrDefaultAsync(b => b.Id == bookId);
            Assert.NotNull(book);
            Assert.Equal(Status.Available, book.Status);

            var lastRental = await _db.Rentals
                .Where(r => r.BookId == bookId)
                .OrderByDescending(r => r.BeginDate)
                .FirstOrDefaultAsync();
            Assert.NotNull(lastRental);
            Assert.NotNull(lastRental.EndDate);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal(nameof(RentalsController.AvailableBooks), redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task FinishRentalOnGet_WhenEntityExistsAndIsNotRented_ReturnsBadRequestObjectResult()
        {
            //Arrange
            TestDbSeeder.Initialize(_db);
            TestDbSeeder.SeedBooks(_db);
            TestDbSeeder.SeedRentals(_db);

            //Act
            var bookId = 2;
            var result = await _controller.FinishRental(bookId);

            //Assert
            var book = await _db.Books.Include(b => b.Rentals).FirstOrDefaultAsync(b => b.Id == bookId);
            Assert.NotNull(book);
            Assert.NotEqual(Status.Rented, book.Status);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(405, objectResult.StatusCode);
            Assert.Equal("Method call is invalid.", objectResult.Value);
        }

        [Fact]
        public async Task FinishRentalOnGet_WhenEntityDoesNotExist_ReturnsNotFoundObjectResult()
        {
            //Arrange
            TestDbSeeder.Initialize(_db);

            //Act
            var bookId = 1;
            var result = await _controller.FinishRental(bookId);

            //Assert
            var book = await _db.Books.Include(b => b.Rentals).FirstOrDefaultAsync(b => b.Id == bookId);
            Assert.Null(book);

            var objectResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Entity does not exist.", objectResult.Value);
        }
    }
}
