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
    [ApiController]
    public class RentalsApiController : ControllerBase
    {
        private readonly RentalsService _rentalsService;

        public RentalsApiController(ApplicationDbContext db, IMapper mapper)
        {
            _rentalsService = new RentalsService(db, mapper);
        }

        // GET: api/rentals
        [Route("api/rentals")]
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

            return Ok(booksList);
        }

        // GET: api/rentals/availablebooks
        [Route("api/rentals/availablebooks")]
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

            return Ok(booksList);
        }


        // GET: api/rentals/rentedbooks
        [Route("api/rentals/rentedbooks")]
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

            return Ok(booksList);
        }

        // PUT: api/rentals/startrental/5
        [Route("api/rentals/startrental/{id}")]
        [HttpPut]
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

            return Ok();
        }

        // PUT: api/rentals/finishrental/5
        [Route("api/rentals/finishrental/{id}")]
        [HttpPut]
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

            return Ok();
        }
    }
}
