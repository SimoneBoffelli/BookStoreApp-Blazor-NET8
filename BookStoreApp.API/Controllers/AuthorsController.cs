using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreApp.API.Data;
using BookStoreApp.API.Models.Author;
using AutoMapper;
using BookStoreApp.API.Static;

namespace BookStoreApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly BookStoreDbContext _context;
        private readonly IMapper mapper;
        private readonly ILogger<AuthorsController> logger;

        public AuthorsController(BookStoreDbContext context, IMapper mapper, ILogger<AuthorsController> logger)
        {
            _context = context;
            this.mapper = mapper;
            this.logger = logger;
        }

        // GET: api/Authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DtoAuthorReadOnly>>> GetAuthors()
        {
            // con LogInformation si registra l'azione nel log
            logger.LogInformation($"GET: Request to {nameof(GetAuthors)}");
            try
            {
                // con ToListAsync() si restituisce una lista di autori
                var authors = mapper.Map<IEnumerable<DtoAuthorReadOnly>>(await _context.Authors.ToListAsync());
                // con OK() si restituisce un codice 200
                return Ok(authors);
            }
            catch (Exception ex)
            {
                // con LogError si registra l'errore nel log e si restituisce un il metodo che ha generato l'errore
                logger.LogError(ex, $"Error Performing GET in {nameof(GetAuthors)}");

                // ritorna un codice 500 (crea un codice di errore personalizzato)
                //return StatusCode(500, "There was an error completing your request. Please Try Again Later");
                
                // in questo modo chiama il metodo statico Error500Message della classe Messages dove sono salvati gli errori personalizzati da riutilizzare
                return StatusCode(500, Messages.Error500Message);
            }
            
        }

        // GET: api/Authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DtoAuthorReadOnly>> GetAuthor(int id)
        {
            try
            {
                var author = await _context.Authors.FindAsync(id);

                if (author == null)
                {
                    // con LogWarning si registra un warning nel log
                    logger.LogWarning($"GET: Author {nameof(GetAuthor)} with id {id} not found");

                    return NotFound();
                }
                var authorDto = mapper.Map<DtoAuthorReadOnly>(author);

                return Ok(authorDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Performing GET in {nameof(GetAuthor)}");

                return StatusCode(500, Messages.Error500Message);
            }
       
        }

        // PUT: api/Authors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, DtoAuthorUpdate authorDto)
        {
            if (id != authorDto.Id)
            {
                logger.LogWarning($"Update ID invalid in {nameof(PutAuthor)} - ID: {id}");

                return BadRequest();
            }

            var author = await _context.Authors.FindAsync(id);

            if (author == null)
            {
                // con LogWarning si registra un warning nel log
                logger.LogWarning($"{nameof(Author)} record not found in {nameof(PutAuthor)} - ID: {id}");

                return NotFound();
            }

            mapper.Map(authorDto, author);
            _context.Entry(author).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            // se non viene salvato correttamente, si controlla se l'autore esiste
            catch (DbUpdateConcurrencyException ex)
            {
                // se il metodo nel controllo e' async, si deve usare await
                if (!await AuthorExists(id))
                {
                    // con LogWarning si registra un warning nel log
                    logger.LogWarning($"PUT: Author id {id} not found");

                    return NotFound();
                }
                else
                {
                    logger.LogError(ex, $"Error Performing PUT in {nameof(PutAuthor)}");
                    return StatusCode(500, Messages.Error500Message);
                }
            }

            return NoContent();
        }

        // POST: api/Authors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DtoAuthorCreate>> PostAuthor(DtoAuthorCreate authorDto)
        {
            try
            {
            /* metodo per mappare i campi di authorDto con quelli di Author (vecchio metodo)
            var author = new Author
            {
                FirstName = authorDto.FirstName,
                LastName = authorDto.LastName,
                Bio = authorDto.Bio
            };*/

                // metodo per mappare i campi di authorDto con quelli di Author con Mapper (nuovo metodo)
                var author = mapper.Map<Author>(authorDto);

                //RESO ASYNC (originariamente non lo era)
                await _context.Authors.AddAsync(author);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Performing POST in {nameof(PostAuthor)}");
                return StatusCode(500, Messages.Error500Message);
            }
            
        }

        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                var author = await _context.Authors.FindAsync(id);
                if (author == null)
                {
                    // con LogWarning si registra un warning nel log
                    logger.LogWarning($"DELETE: Author id {id} not found");

                    return NotFound();
                }

                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogWarning($"{nameof(Author)} record not found in {nameof(DeleteAuthor)} - ID: {id}");

                return StatusCode(500, Messages.Error500Message);
            }
            
        }

        // Questo metodo e' stato reso async (originalmente non lo era)
        // RICORDARSI di aggiungere await nel if del catch del PUT!!!
        // questo metodo controlla se l'autore esiste
        private async Task<bool> AuthorExists(int id)
        {
            return await _context.Authors.AnyAsync(e => e.Id == id);
        }
    }
}
