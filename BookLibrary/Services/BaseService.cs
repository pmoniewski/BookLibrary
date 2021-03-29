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
    public class BaseService
    {
        protected readonly ApplicationDbContext _db;
        protected readonly IMapper _mapper;

        public BaseService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Book> GetBookById(int? id)
        {
            if (id == null)
            {
                throw new ArgumentException("Entity does not exist.");
            }

            Book book = await _db.Books
                .Include(b => b.Rentals)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                throw new ArgumentException("Entity does not exist.");
            }

            return book;
        }

        public bool BookExists(int id)
        {
            return _db.Books.Any(b => b.Id == id);
        }

        public async Task<Rental> GetRentalById(int? id)
        {
            if (id == null)
            {
                throw new ArgumentException("Entity does not exist.");
            }

            Rental rental = await _db.Rentals
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rental == null)
            {
                throw new ArgumentException("Entity does not exist.");
            }

            return rental;
        }

        public bool RentalExists(int id)
        {
            return _db.Rentals.Any(r => r.Id == id);
        }
    }
}
