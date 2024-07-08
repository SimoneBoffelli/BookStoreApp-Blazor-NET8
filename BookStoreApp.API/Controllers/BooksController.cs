using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreApp.API.Data;
using AutoMapper;
using BookStoreApp.API.Models.Book;
using AutoMapper.QueryableExtensions;
using BookStoreApp.API.Models.Author;
using BookStoreApp.API.Static;
using Microsoft.AspNetCore.Authorization;

namespace BookStoreApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // specifica che l'accesso a questo controller e' consentito solo agli utenti autenticati (se no ritorna 401 Unauthorized)
    public class BooksController : ControllerBase
    {
        private readonly BookStoreDbContext _context;
        private readonly IMapper mapper;
        private readonly ILogger logger;

        public BooksController(BookStoreDbContext context, IMapper mapper, ILogger<BooksController> logger)
        {
            _context = context;
            this.mapper = mapper;
            this.logger = logger;
        }

        // GET: api/Books
        [HttpGet]
        [AllowAnonymous] // specifica che l'accesso a questo controller e' consentito a tutti gli utenti (se no ritorna 401 Unauthorized)
        public async Task<ActionResult<IEnumerable<DtoBookReadOnly>>> GetBooks()
        {
            // con LogInformation si registra l'azione nel log
            logger.LogInformation($"GET: Request to {nameof(GetBook)}");
            try
            {
                // in questo modo posso mappare direttamente la variabile di tipo book con il dto
                // grazie al metodo ProjectTo di AutoMapper.QueryableExtensions
                var books = await _context.Books.Include(q => q.Author)
                    .ProjectTo<DtoBookReadOnly>(mapper.ConfigurationProvider)
                    .ToListAsync(); // usare ToListAsync() per restituire una lista di libri!!!!!!!

                return Ok(books);
            }
            catch (Exception ex)
            {
                // con LogError si registra l'errore nel log e si restituisce un il metodo che ha generato l'errore
                logger.LogError(ex, $"Error Performing GET in {nameof(GetBook)}");

                // ritorna un codice 500 (crea un codice di errore personalizzato)
                //return StatusCode(500, "There was an error completing your request. Please Try Again Later");

                // in questo modo chiama il metodo statico Error500Message della classe Messages dove sono salvati gli errori personalizzati da riutilizzare
                return StatusCode(500, Messages.Error500Message);
            }
            
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DtoBookDetails>> GetBook(int id)
        {
            try
            {
                var book = await _context.Books
                    .Include(q => q.Author)
                    .ProjectTo<DtoBookDetails>(mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(q => q.Id == id); // usare FirstOrDefaultAsync per restituire un singolo libro!!!!!!!

                if (book == null)
                {
                    // con LogWarning si registra un warning nel log
                    logger.LogWarning($"GET: Author {nameof(GetBook)} with id {id} not found");

                    return NotFound();
                }

                return Ok(book);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Performing GET in {nameof(GetBook)}");

                return StatusCode(500, Messages.Error500Message);
            }            
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")] // specifica che l'accesso a questo controller e' consentito solo agli utenti autenticati (se no ritorna 401 Unauthorized)
        public async Task<IActionResult> PutBook(int id, DtoBookUpdate bookDto)
        {
            if (id != bookDto.Id)
            {
                logger.LogWarning($"Update ID invalid in {nameof(PutBook)} - ID: {id}");

                return BadRequest();
            }

            //fatch dei dati dal database
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                // con LogWarning si registra un warning nel log
                logger.LogWarning($"{nameof(Book)} record not found in {nameof(PutBook)} - ID: {id}");

                return NotFound();
            }

            mapper.Map(bookDto, book);
            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // ATTENZIONE: il metodo BookExists è da modificare in asincrono per funzionare con await
                // (vedi sotto)
                if (!await BookExists(id))
                {
                    // con LogWarning si registra un warning nel log
                    logger.LogWarning($"PUT: Author id {id} not found");

                    return NotFound();
                }
                else
                {
                    logger.LogError(ex, $"Error Performing PUT in {nameof(PutBook)}");
                    return StatusCode(500, Messages.Error500Message);
                }
            }

            return NoContent();
        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Administrator")] // specifica che l'accesso a questo controller e' consentito solo agli utenti autenticati (se no ritorna 401 Unauthorized)
        public async Task<ActionResult<DtoBookCreate>> PostBook(DtoBookCreate bookDto)
        {
            try
            {
                var book = mapper.Map<Book>(bookDto);

                await _context.Books.AddAsync(book);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Performing POST in {nameof(PostBook)}");
                return StatusCode(500, Messages.Error500Message);
            }            
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator ")] // specifica che l'accesso a questo controller e' consentito solo agli utenti autenticati (se no ritorna 401 Unauthorized)
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    // con LogWarning si registra un warning nel log
                    logger.LogWarning($"DELETE: Author id {id} not found");

                    return NotFound();
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogWarning($"{nameof(Book)} record not found in {nameof(DeleteBook)} - ID: {id}");

                return StatusCode(500, Messages.Error500Message);
            }
        }

        // ATTENZIONE: il metodo BookExists è da modificare in asincrono per funzionare con await
        private async Task<bool> BookExists(int id)
        {
            return await _context.Books.AnyAsync(e => e.Id == id);
        }
    }
}
