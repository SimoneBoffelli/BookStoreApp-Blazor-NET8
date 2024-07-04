using AutoMapper;
using BookStoreApp.API.Data;
using BookStoreApp.API.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> logger;
        private readonly IMapper mapper;
        private readonly UserManager<ApiUser> userManager;

        // UserManager<> e' una classe di Identity che permette di gestire le operazioni sugli utenti
        public AuthenticationController(ILogger<AuthenticationController> logger, IMapper mapper,
            UserManager<ApiUser> userManager)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        // il metodo post permette di inviare dati al server
        [HttpPost]
        // il metodo Route permette di registrare un nuovo utente (il percorso sara' api/authentication/register)
        [Route("register")]
        public async Task<IActionResult> Register(DtoUser userDto)
        {
            // logga il tentativo di registrazione
            logger.LogInformation($"Registration Attempt for {userDto.Email}");

            /* -------------------------------------------------------------------
            //questo comportamento viene gestito automaticamente dal modelState 
            //senza bisogno di controllarlo manualmente
            -------------------------------------------------------------------
            if (userDto == null) 
            {
                return BadRequest("Insufficent Data Provided");
            }
            ---------------------------------------------------------------------*/

            try
            {
                // mappatura tra DtoUser e ApiUser
                var user = mapper.Map<ApiUser>(userDto);

                // IMPORTANTE!!!!!!!!!!!!!!!
                // dato che la mail e, usata come username si deve specificare
                // che il campo username viene assenato usando il campo email
                user.UserName = userDto.Email;

                // creazione dell'utente con il metodo CreateAsync di UserManager
                var result = await userManager.CreateAsync(user, userDto.Password);

                if (result.Succeeded == false) // se la creazione dell'utente non e' andata a buon fine
                {
                    foreach (var error in result.Errors) // per ogni errore
                    {
                        // aggiungi l'errore al modelState
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }

                // assegna il ruolo di default "User"
                await userManager.AddToRoleAsync(user, "User");

                /*-------------------------------------------------------------------
                //questo codice non e' necessario perche' il ruolo non puo' essere specificato
                //nel form di registrazione quindi l'utente viene sempre creato con il ruolo di default "User"
                -------------------------------------------------------------------
                if (string.IsNullOrEmpty(userDto.Role))
                {
                    await userManager.AddToRoleAsync(user, "User");
                }
                else
                {
                    await userManager.AddToRoleAsync(user, userDto.Role);
                }
                -------------------------------------------------------------------*/

                // ritorna un messaggio di successo
                return Accepted();
            }
            catch (Exception ex)
            {
                // logga l'errore
                logger.LogError(ex, $"Something Went Wrong in the {nameof(Register)}");

                // ritorna un errore 500 (metodo alternativo per ritornare un errore rispetto
                // al messaggio personalizzato usato finora)
                return Problem($"Something Went Wrong in the {nameof(Register)}", statusCode: 500);
            }
        }

        // il metodo post permette di inviare dati al server
        [HttpPost]
        // il metodo Route permette di loggare un utente (il percorso sara' api/authentication/login)
        [Route("login")]
        public async Task<IActionResult> Login(DtoLogginUser userDto)
        {
            // logga il tentativo di login
            logger.LogInformation($"Login Attempt for {userDto.Email}");

            try
            {
                // cerca l'utente tramite l'email
                var user = await userManager.FindByEmailAsync(userDto.Email);

                // controlla se la password e' valida
                var passwordValid = await userManager.CheckPasswordAsync(user, userDto.Password);

                // se l'utente non esiste o la password non e' valida ritorna un errore 404
                if (user == null || passwordValid == false)
                    return Unauthorized(userDto);

                // ritorna un messaggio di successo
                return Accepted();
            }
            catch (Exception ex)
            {
                // logga l'errore
                logger.LogError(ex, $"Something Went Wrong in the {nameof(Login)}");

                // ritorna un errore 500 (metodo alternativo per ritornare un errore rispetto
                // al messaggio personalizzato usato finora)
                return Problem($"Something Went Wrong in the {nameof(Login)}", statusCode: 500);
            }
        }
    }
}
