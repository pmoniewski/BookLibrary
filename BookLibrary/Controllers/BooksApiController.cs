using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BookLibrary.Data;
using BookLibrary.Services;
using BookLibrary.ViewModels;

namespace BookLibrary.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BooksApiController : ControllerBase
    {
        private readonly BooksService _booksService;

        public BooksApiController(ApplicationDbContext db, IMapper mapper)
        {
            _booksService = new BooksService(db, mapper);
        }

        // GET: api/books
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<BookViewModel> booksList;

            try
            {
                booksList = await _booksService.GetBooksCollection();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(booksList);
        }

        // GET: api/books/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            BookViewModel bookViewModel;

            try
            {
                bookViewModel = await _booksService.GetBookDetails(id);
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(bookViewModel);
        }

        // POST: api/books
        [HttpPost]
        public async Task<IActionResult> Create(BookViewModel bookViewModel)
        {
            try
            {
                int bookId = await _booksService.CreateBook(bookViewModel);
                bookViewModel = await _booksService.GetBookDetails(bookId);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(bookViewModel);
        }

        // PUT: api/books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, BookViewModel bookViewModel)
        {
            try
            {
                if (id == bookViewModel.Id)
                {
                    int bookId = await _booksService.EditBook(bookViewModel);
                    bookViewModel = await _booksService.GetBookDetails(bookId);
                }
                else
                {
                    return BadRequest("Parameters are not valid.");
                }
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(bookViewModel);
        }

        // DELETE: api/books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _booksService.DeleteBook(id);
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }
    }
}
