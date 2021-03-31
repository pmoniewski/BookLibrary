using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using BookLibrary.Data;
using BookLibrary.Models;
using BookLibrary.ViewModels;

namespace BookLibrary.Services
{
    public class RentalsService : BaseService
    {
        public RentalsService(ApplicationDbContext db, IMapper mapper)
            : base(db, mapper)
        {
        }

        public Rental GetLastRentalOfBook(Book book)
        {
            return book.Rentals
                .OrderByDescending(r => r.BeginDate)
                .FirstOrDefault();
        }

        public async Task<IEnumerable<BookViewModel>> GetBooksCollection()
        {
            IEnumerable<Book> booksList = await _db.Books
                .Include(b => b.Rentals)
                .ToListAsync();

            return _mapper.Map<IEnumerable<BookViewModel>>(booksList);
        }

        public async Task<IEnumerable<BookViewModel>> GetAvailableBooksCollection()
        {
            IEnumerable<BookViewModel> booksList = await GetBooksCollection();

            return booksList
                .Where(b => b.Status == Status.Available)
                .ToList();
        }

        public async Task<IEnumerable<BookViewModel>> GetRentedBooksCollection()
        {
            IEnumerable<BookViewModel> booksList = await GetBooksCollection();

            return booksList
                .Where(b => b.Status == Status.Rented)
                .ToList();
        }

        public async Task StartRental(int? id)
        {
            Book book = await GetBookById(id);

            if (book.Status != Status.Available)
            {
                throw new InvalidOperationException("Method call is invalid.");
            }

            Rental rental = new();
            rental.Book = book;
            rental.BeginDate = DateTime.Now;
            _db.Add(rental);

            book.Status = Status.Rented;
            _db.Update(book);

            await _db.SaveChangesAsync();
        }

        public async Task FinishRental(int? id)
        {
            Book book = await GetBookById(id);

            if (book.Status != Status.Rented)
            {
                throw new InvalidOperationException("Method call is invalid.");
            }

            Rental rental = GetLastRentalOfBook(book);

            if (rental == null || rental.EndDate != null)
            {
                throw new InvalidOperationException("Method call is invalid.");
            }

            rental.EndDate = DateTime.Now;

            if (rental.BeginDate > rental.EndDate)
            {
                throw new InvalidOperationException("Method call is invalid.");
            }

            _db.Update(rental);

            book.Status = Status.Available;
            _db.Update(book);

            await _db.SaveChangesAsync();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
