using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Models.Domain;
using WebAPI.Models.DTO;

namespace WebAPI.Controllers
{
    public class BookController : Controller
    {
        private readonly AppDbContext _dbContext;

        public BookController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //GET http://localhost:port/api/get-all-books
        [HttpGet("get-all-books")]
        public IActionResult GetAll()
        {
            //var allBooksDomain = _dbContext.Books.ToList();
            //Get Data form Database -Domain Models
            var allBooksDomain = _dbContext.Books;
            //Map Domain Models to DTOs
            var allBooksDTO = allBooksDomain.Select(Books => new Models.DTO.BookWithAuthorAndPublisherDTO()
            {
                Id = Books.Id,
                Title = Books.Title,
                Description = Books.Description,
                IsRead = Books.IsRead,
                DateRead = Books.IsRead ? Books.DateRead.Value : null,
                Rate = Books.IsRead ? Books.Rate.Value : null,
                Genre = Books.Genre,
                CoverUrl = Books.CoverUrl,
                PublisherName = Books.Publisher.Name,
                AuthorsNames = Books.Book_Authors.Select(ba => ba.Author.FullName).ToList()
            }).ToList();
            //return DTOs
            return Ok(allBooksDTO);
        }
        [HttpGet]
        [Route("get-book-by-id/{int}")]
        public IActionResult GetBookById([FromRoute] int id)
        {
            //get book Domain model from database
            var bookWithDomain = _dbContext.Books.Where(n => n.Id == id);
            if (bookWithDomain == null)
            {
                return NotFound();
            }
            //Map Domain model to DTO
            var bookWithIdDTO = bookWithDomain.Select(book => new BookWithAuthorAndPublisherDTO()
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                IsRead = book.IsRead,
                DateRead = book.DateRead,
                Rate = book.Rate,
                Genre = book.Genre,
                CoverUrl = book.CoverUrl,
                PublisherName = book.Publisher.Name,
                AuthorsNames = book.Book_Authors.Select(n => n.Author.FullName).ToList()
            });
            return Ok(bookWithIdDTO);
        }

        [HttpPost("add-book")]
        public IActionResult AddBook([FromBody] AddBookRequestDTO addBookRequestDTO)
        {
            //Map DTO to Domain Model
            var bookDomainModel = new Books()
            {
                Title = addBookRequestDTO.Title,
                Description = addBookRequestDTO.Description,
                IsRead = addBookRequestDTO.IsRead,
                DateRead = addBookRequestDTO.DateRead,
                Rate = addBookRequestDTO.Rate,
                Genre = addBookRequestDTO.Genre,
                CoverUrl = addBookRequestDTO.CoverUrl,
                DateAdded = addBookRequestDTO.DateAdded,
                PublisherID = addBookRequestDTO.PublisherId
            };
            //use Domain Model to create Book
            _dbContext.Books.Add(bookDomainModel);
            _dbContext.SaveChanges();

            foreach (var id in addBookRequestDTO.AuthorIds)
            {
                var _book_author = new Book_Author()
                {
                    BookId = bookDomainModel.Id,
                    AuthorId = id
                };
                _dbContext.Book_Authors.Add(_book_author);
                _dbContext.SaveChanges();
            }
            _dbContext.SaveChanges();
            return Ok(bookDomainModel);
        }
        [HttpPut("update-book-by-id/{id}")]
        public IActionResult UpdateBookById(int id, [FromBody] AddBookRequestDTO bookDTO)
        {
            var bookDomainModel = _dbContext.Books.FirstOrDefault(n => n.Id == id);
            if (bookDomainModel == null)
            {
                bookDomainModel.Title = bookDTO.Title;
                bookDomainModel.Description = bookDTO.Description;
                bookDomainModel.IsRead = bookDTO.IsRead;
                bookDomainModel.DateRead = bookDTO.DateRead;
                bookDomainModel.Rate = bookDTO.Rate;
                bookDomainModel.Genre = bookDTO.Genre;
                bookDomainModel.CoverUrl = bookDTO.CoverUrl;
                bookDomainModel.DateAdded = bookDTO.DateAdded;
                bookDomainModel.PublisherID = bookDTO.PublisherId;
                _dbContext.SaveChanges();
            }
            var authorDomain = _dbContext.Book_Authors.Where(a => a.BookId == id).ToList();
            if (authorDomain != null)
            {
                _dbContext.Book_Authors.RemoveRange(authorDomain);
                _dbContext.SaveChanges();
            }
            foreach (var authorId in bookDTO.AuthorIds)
            {
                var book_author = new Book_Author()
                {
                    BookId = id,
                    AuthorId = authorId
                };
                _dbContext.Book_Authors.Add(book_author);
                _dbContext.SaveChanges();
            }
            return Ok(bookDTO);
        }
        [HttpDelete("delete-book-by-id/{id}")]
        public IActionResult DeleteBookById(int id)
        {
            var bookDomain = _dbContext.Books.FirstOrDefault(n => n.Id == id);
            if (bookDomain == null)
            {
                _dbContext.Books.Remove(bookDomain);
                _dbContext.SaveChanges();
   
            }
            return Ok();
        }
    }
}
   
