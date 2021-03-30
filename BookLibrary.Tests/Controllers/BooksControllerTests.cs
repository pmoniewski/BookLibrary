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
    public class BooksControllerTests
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly BooksController _controller;

        public BooksControllerTests(WebApplicationFactory<Startup> factory)
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

            _controller = new BooksController(_db, _mapper);
        }

        [Fact]
        public async Task IndexOnGet_WhenEntitiesExist_ReturnsViewResult()
        {
            //Arrange
            TestDbSeeder.Initialize(_db);
            TestDbSeeder.SeedBooks(_db);

            //Act
            var result = await _controller.Index();

            //Assert
            var books = await _db.Books.ToListAsync();
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
            var books = await _db.Books.ToListAsync();
            Assert.NotNull(books);
            Assert.Empty(books);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<BookViewModel>>(viewResult.Model);
            Assert.Empty(model);
        }

        [Fact]
        public async Task DetailsOnGet_WhenEntityExists_ReturnsViewResult()
        {
            //Arrange
            TestDbSeeder.Initialize(_db);
            TestDbSeeder.SeedBooks(_db);

            //Act
            var bookId = 1;
            var result = await _controller.Details(bookId);

            //Assert
            var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == bookId);
            Assert.NotNull(book);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookViewModel>(viewResult.Model);
            Assert.Equal(book.Id, model.Id);
            Assert.Equal(book.Isbn, model.Isbn);
            Assert.Equal(book.Title, model.Title);
            Assert.Equal(book.Author, model.Author);
        }

        [Fact]
        public async Task DetailsOnGet_WhenEntityDoesNotExist_ReturnsNotFoundObjectResult()
        {
            //Arrange
            TestDbSeeder.Initialize(_db);

            //Act
            var bookId = 1;
            var result = await _controller.Details(bookId);

            //Assert
            var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == bookId);
            Assert.Null(book);

            var objectResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Entity does not exist.", objectResult.Value);
        }

        [Fact]
        public void CreateOnGet_WhenEnteringEmptyForm_ReturnsViewResult()
        {
            //Arrange & Act
            var result = _controller.Create();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        [Fact]
        public async Task CreateOnPost_WhenModelStateIsValid_ReturnsRedirectToIndex()
        {
            //Arrange
            TestDbSeeder.Initialize(_db);

            var newBook = new Book
            {
                Isbn = "978-83-274-9932-5",
                Title = "Alicja w Krainie Czarów",
                Author = "Lewis Carroll"
            };

            var newBookViewModel = _mapper.Map<BookViewModel>(newBook);

            //Act
            var result = await _controller.Create(newBookViewModel);

            //Assert
            var book = await _db.Books.FirstOrDefaultAsync(b => b.Isbn == newBookViewModel.Isbn);
            Assert.NotNull(book);
            Assert.Equal(newBookViewModel.Isbn, book.Isbn);
            Assert.Equal(newBookViewModel.Title, book.Title);
            Assert.Equal(newBookViewModel.Author, book.Author);
            var books = await _db.Books.ToListAsync();
            Assert.NotNull(books);
            Assert.Single(books);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal(nameof(BooksController.Index), redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task CreateOnPost_WhenModelStateIsInvalid_ReturnsViewResult()
        {
            //Arrange
            TestDbSeeder.Initialize(_db);

            var newBook = new Book
            {
                Isbn = "978-83-274-9932-5",
                Title = string.Empty,
                Author = "Lewis Carroll"
            };

            var newBookViewModel = _mapper.Map<BookViewModel>(newBook);

            _controller.ModelState.AddModelError(nameof(Book.Title), "The Title field is required.");

            //Act
            var result = await _controller.Create(newBookViewModel);

            //Assert
            var book = await _db.Books.FirstOrDefaultAsync(b => b.Isbn == newBookViewModel.Isbn);
            Assert.Null(book);
            var books = await _db.Books.ToListAsync();
            Assert.NotNull(books);
            Assert.Empty(books);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookViewModel>(viewResult.Model);
            Assert.Equal(newBookViewModel.Isbn, model.Isbn);
            Assert.Equal(newBookViewModel.Title, model.Title);
            Assert.Equal(newBookViewModel.Author, model.Author);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
        }

        [Fact]
        public async Task EditOnGet_WhenEntityExists_ReturnsViewResult()
        {
            //Arrange
            TestDbSeeder.Initialize(_db);
            TestDbSeeder.SeedBooks(_db);

            //Act
            var bookId = 1;
            var result = await _controller.Edit(bookId);

            //Assert
            var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == bookId);
            Assert.NotNull(book);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookViewModel>(viewResult.Model);
            Assert.Equal(book.Id, model.Id);
            Assert.Equal(book.Isbn, model.Isbn);
            Assert.Equal(book.Title, model.Title);
            Assert.Equal(book.Author, model.Author);
        }

        [Fact]
        public async Task EditOnGet_WhenEntityDoesNotExist_ReturnsNotFoundObjectResult()
        {
            //Arrange
            TestDbSeeder.Initialize(_db);

            //Act
            var bookId = 1;
            var result = await _controller.Edit(bookId);

            //Assert
            var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == bookId);
            Assert.Null(book);

            var objectResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Entity does not exist.", objectResult.Value);
        }

        [Fact]
        public async Task EditOnPost_WhenModelStateIsValid_ReturnsRedirectToActionResult()
        {
            //Arrange
            TestDbSeeder.Initialize(_db);
            TestDbSeeder.SeedBooks(_db);

            var editedBook = TestDbSeeder.Books[0];
            editedBook.Id = 1;

            var editedBookViewModel = _mapper.Map<BookViewModel>(editedBook);
            editedBookViewModel.Title += " i pół";

            //Act
            var result = await _controller.Edit(editedBookViewModel.Id, editedBookViewModel);

            //Assert
            var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == editedBookViewModel.Id);
            Assert.NotNull(book);
            Assert.Equal(editedBookViewModel.Isbn, book.Isbn);
            Assert.Equal(editedBookViewModel.Title, book.Title);
            Assert.Equal(editedBookViewModel.Author, book.Author);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal(nameof(BooksController.Index), redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task EditOnPost_WhenModelStateIsInvalid_ReturnsViewResult()
        {
            //Arrange
            TestDbSeeder.Initialize(_db);
            TestDbSeeder.SeedBooks(_db);

            var editedBook = TestDbSeeder.Books[0];
            editedBook.Id = 1;

            var editedBookViewModel = _mapper.Map<BookViewModel>(editedBook);
            editedBookViewModel.Title = string.Empty;

            _controller.ModelState.AddModelError(nameof(Book.Title), "The Title field is required.");

            //Act
            var result = await _controller.Edit(editedBookViewModel.Id, editedBookViewModel);

            //Assert
            var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == editedBookViewModel.Id);
            Assert.NotNull(book);
            Assert.Equal(editedBook.Isbn, book.Isbn);
            Assert.Equal(editedBook.Title, book.Title);
            Assert.Equal(editedBook.Author, book.Author);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookViewModel>(viewResult.Model);
            Assert.Equal(editedBookViewModel.Isbn, model.Isbn);
            Assert.Equal(editedBookViewModel.Title, model.Title);
            Assert.Equal(editedBookViewModel.Author, model.Author);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
        }

        [Fact]
        public async Task DeleteOnGet_WhenEntityExists_ReturnsViewResult()
        {
            //Arrange
            TestDbSeeder.Initialize(_db);
            TestDbSeeder.SeedBooks(_db);

            //Act
            var bookId = 1;
            var result = await _controller.Delete(bookId);

            //Assert
            var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == bookId);
            Assert.NotNull(book);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookViewModel>(viewResult.Model);
            Assert.Equal(book.Id, model.Id);
            Assert.Equal(book.Isbn, model.Isbn);
            Assert.Equal(book.Title, model.Title);
            Assert.Equal(book.Author, model.Author);
        }

        [Fact]
        public async Task DeleteOnGet_WhenEntityDoesNotExist_ReturnsNotFoundObjectResult()
        {
            //Arrange
            TestDbSeeder.Initialize(_db);

            //Act
            var bookId = 1;
            var result = await _controller.Delete(bookId);

            //Assert
            var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == bookId);
            Assert.Null(book);

            var objectResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Entity does not exist.", objectResult.Value);
        }

        [Fact]
        public async Task DeleteOnPost_WhenEntityExists_ReturnsRedirectToIndex()
        {
            //Arrange
            TestDbSeeder.Initialize(_db);
            TestDbSeeder.SeedBooks(_db);

            //Act
            var bookId = 1;
            var result = await _controller.DeleteConfirmed(bookId);

            //Assert
            var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == bookId);
            Assert.Null(book);
            var books = await _db.Books.ToListAsync();
            Assert.NotNull(books);
            Assert.Equal(TestDbSeeder.Books.Length - 1, books.Count);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal(nameof(BooksController.Index), redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task DeleteOnPost_WhenEntityDoesNotExist_ReturnsNotFoundObjectResult()
        {
            //Arrange
            TestDbSeeder.Initialize(_db);

            //Act
            var bookId = 1;
            var result = await _controller.DeleteConfirmed(bookId);

            //Assert
            var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == bookId);
            Assert.Null(book);
            var books = await _db.Books.ToListAsync();
            Assert.NotNull(books);
            Assert.Empty(books);

            var objectResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Entity does not exist.", objectResult.Value);
        }
    }
}
