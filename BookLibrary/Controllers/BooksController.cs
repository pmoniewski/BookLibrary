using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BookLibrary.Data;
using BookLibrary.Services;
using BookLibrary.ViewModels;

namespace BookLibrary.Controllers
{
    public class BooksController : Controller
    {
        private readonly BooksService _booksService;

        public BooksController(ApplicationDbContext db, IMapper mapper)
        {
            _booksService = new BooksService(db, mapper);
        }

        // GET: Books
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

            return View(booksList);
        }

        // GET: Books/Details/5
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

            return View(bookViewModel);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookViewModel bookViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _booksService.CreateBook(bookViewModel);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return View(bookViewModel);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            BookViewModel book;

            try
            {
                book = await _booksService.GetBookDetails(id);
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return View(book);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookViewModel bookViewModel)
        {
            try
            {
                if (ModelState.IsValid && id == bookViewModel.Id)
                {
                    await _booksService.EditBook(bookViewModel);
                    return RedirectToAction(nameof(Index));
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

            return View(bookViewModel);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

            return View(bookViewModel);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _booksService.DeleteBook(id);
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
