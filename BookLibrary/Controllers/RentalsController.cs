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
    public class RentalsController : Controller
    {
        private readonly RentalsService _rentalsService;

        public RentalsController(ApplicationDbContext db, IMapper mapper)
        {
            _rentalsService = new RentalsService(db, mapper);
        }

        // GET: Rentals
        public async Task<IActionResult> Index()
        {
            IEnumerable<BookViewModel> booksList;

            try
            {
                booksList = await _rentalsService.GetBooksCollection();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            ViewData["Title"] = "Rentals Index";

            return View(booksList);
        }

        // GET: Rentals/AvailableBooks
        public async Task<IActionResult> AvailableBooks()
        {
            IEnumerable<BookViewModel> booksList;

            try
            {
                booksList = await _rentalsService.GetAvailableBooksCollection();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            ViewData["Title"] = "Available Books";

            return View(nameof(Index), booksList);
        }


        // GET: Rentals/RentedBooks
        public async Task<IActionResult> RentedBooks()
        {
            IEnumerable<BookViewModel> booksList;

            try
            {
                booksList = await _rentalsService.GetRentedBooksCollection();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            ViewData["Title"] = "Rented Books";

            return View(nameof(Index), booksList);
        }

        // GET: Rentals/StartRental/5
        public async Task<IActionResult> StartRental(int? id)
        {
            try
            {
                await _rentalsService.StartRental(id);
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return StatusCode(StatusCodes.Status405MethodNotAllowed, e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return RedirectToAction(nameof(RentedBooks));
        }

        // GET: Rentals/FinishRental/5
        public async Task<IActionResult> FinishRental(int? id)
        {
            try
            {
                await _rentalsService.FinishRental(id);
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return StatusCode(StatusCodes.Status405MethodNotAllowed, e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return RedirectToAction(nameof(AvailableBooks));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
