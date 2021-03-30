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

namespace BookLibrary.Tests
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

            var seeder = new TestDbSeeder();
            seeder.Seed(_db);

            var mockMapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            _mapper = mockMapperConfiguration.CreateMapper();

            _controller = new BooksController(_db, _mapper);
        }

        [Fact]
        public async Task IndexOnGet_ReturnsViewResult_WithListOfEntities()
        {
            //Arrange & Act  
            var result = await _controller.Index();

            //Assert  
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<BookViewModel>>(viewResult.Model);
            Assert.Equal(3, model.Count());
        }

        [Fact]
        public void CreateOnGet_ReturnsViewResult_WithFormForCreate()
        {
            //Arrange & Act
            var result = _controller.Create();

            //Assert  
            var viewResult = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task CreateOnPost_ActionExecutes_WhenEntityIsValid()
        {
            //Arrange
            var newBook = new BookViewModel
            {
                Isbn = "978-83-274-9932-5",
                Title = "Alicja w Krainie Czarów",
                Author = "Lewis Carroll"
            };

            //Act  
            var result = await _controller.Create(newBook);

            //Assert  
            var books = await _db.Books.ToListAsync();
            Assert.Equal(4, books.Count);
        }

        [Fact]
        public async Task CreateOnPost_ReturnsRedirectToIndex_WhenModelStateIsValid()
        {
            //Arrange
            var newBook = new BookViewModel
            {
                Isbn = "978-83-274-9932-5",
                Title = "Alicja w Krainie Czarów",
                Author = "Lewis Carroll"
            };

            //Act  
            var result = await _controller.Create(newBook);

            //Assert  
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task CreateOnPost_ActionDoesNotExecute_WhenEntityIsInvalid()
        {
            //Arrange
            var newBook = new BookViewModel
            {
                Isbn = "978-83-274-9932-5",
                Title = string.Empty,
                Author = "Lewis Carroll"
            };

            _controller.ModelState.AddModelError("Title", "The Title field is required.");

            //Act  
            var result = await _controller.Create(newBook);

            //Assert  
            var books = await _db.Books.ToListAsync();
            Assert.Equal(3, books.Count());
        }

        [Fact]
        public async Task CreateOnPost_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            //Arrange
            var newBook = new BookViewModel
            {
                Isbn = "978-83-274-9932-5",
                Title = string.Empty,
                Author = "Lewis Carroll"
            };

            _controller.ModelState.AddModelError("Title", "The Title field is required.");

            //Act  
            var result = await _controller.Create(newBook);

            //Assert  
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<BookViewModel>(viewResult.Model);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
        }
    }
}
