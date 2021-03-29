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
    public class BooksService : BaseService
    {
        public BooksService(ApplicationDbContext db, IMapper mapper)
            : base(db, mapper)
        {
        }

        public async Task<BookViewModel> GetBookDetails(int? id)
        {
            Book book = await GetBookById(id);

            return _mapper.Map<BookViewModel>(book);
        }

        public async Task<IEnumerable<BookViewModel>> GetBooksCollection()
        {
            IEnumerable<Book> booksList = await _db.Books.ToListAsync();

            return _mapper.Map<IEnumerable<BookViewModel>>(booksList);
        }

        public async Task<int> CreateBook(BookViewModel bookViewModel)
        {
            bookViewModel.Id = 0;

            Book book = _mapper.Map<Book>(bookViewModel);
            _db.Add(book);
            await _db.SaveChangesAsync();

            return book.Id;
        }

        public async Task<int> EditBook(BookViewModel bookViewModel)
        {
            Book book = await GetBookById(bookViewModel.Id);

            try
            {
                _mapper.Map(bookViewModel, book);
                _db.Update(book);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(book.Id))
                {
                    throw new ArgumentException("Entity does not exist.");
                }
                else
                {
                    throw;
                }
            }

            return book.Id;
        }

        public async Task<bool> DeleteBook(int id)
        {
            Book book = await GetBookById(id);
            _db.Books.Remove(book);
            await _db.SaveChangesAsync();

            return true;
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
