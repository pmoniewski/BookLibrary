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
                .OrderByDescending(r => r.StartDate)
                .FirstOrDefault();
        }

        public RentalViewModel GetLastRentalOfBook(BookViewModel book)
        {
            return book.Rentals
                .OrderByDescending(r => r.StartDate)
                .FirstOrDefault();
        }

        public async Task<IEnumerable<BookViewModel>> GetBooksWithRentalsCollection()
        {
            IEnumerable<Book> booksList = await _db.Books
                .Include(b => b.Rentals)
                .ToListAsync();

            IEnumerable<BookViewModel> modelsList = _mapper.Map<IEnumerable<BookViewModel>>(booksList);

            foreach (var book in modelsList)
            {
                var rental = GetLastRentalOfBook(book);
                book.CurrentlyRented = (rental != null && rental.EndDate == null);
            }

            return modelsList;
        }

        public async Task<IEnumerable<BookViewModel>> GetAvailableBooksCollection()
        {
            IEnumerable<BookViewModel> booksList = await GetBooksWithRentalsCollection();

            return booksList.Where(b => !b.CurrentlyRented);
        }

        public async Task<IEnumerable<BookViewModel>> GetRentedBooksCollection()
        {
            IEnumerable<BookViewModel> booksList = await GetBooksWithRentalsCollection();

            return booksList.Where(b => b.CurrentlyRented);
        }

        public async Task StartRental(int? id)
        {
            Book book = await GetBookById(id);
            Rental lastRental = GetLastRentalOfBook(book);

            if (lastRental != null && lastRental.EndDate == null)
            {
                throw new InvalidOperationException("Parameters are not valid.");
            }

            Rental rental = new();
            rental.Book = book;
            rental.StartDate = DateTime.Now;

            _db.Add(rental);
            await _db.SaveChangesAsync();
        }

        public async Task EndRental(int? id)
        {
            Book book = await GetBookById(id);
            Rental rental = GetLastRentalOfBook(book);

            if (rental == null || rental.EndDate != null)
            {
                throw new InvalidOperationException("Parameters are not valid.");
            }

            rental.EndDate = DateTime.Now;

            if (rental.StartDate > rental.EndDate)
            {
                throw new InvalidOperationException("Parameters are not valid.");
            }

            _db.Update(rental);
            await _db.SaveChangesAsync();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
