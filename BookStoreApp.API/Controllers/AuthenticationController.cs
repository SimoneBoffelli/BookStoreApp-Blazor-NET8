using AutoMapper;
using BookStoreApp.API.Data;
using BookStoreApp.API.Models.User;
using BookStoreApp.API.Static;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookStoreApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> logger;
        private readonly IMapper mapper;
        private readonly UserManager<ApiUser> userManager;
        // permette di accedere alle configurazioni del progetto (es. appsettings.json e program.cs)
        private readonly IConfiguration configuration;

        // UserManager<> e' una classe di Identity che permette di gestire le operazioni sugli utenti
        public AuthenticationController(ILogger<AuthenticationController> logger, IMapper mapper,
            UserManager<ApiUser> userManager, IConfiguration configuration)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.userManager = userManager;
            // permette di accedere alle configurazioni del progetto (es. appsettings.json e program.cs)
            this.configuration = configuration;
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

                //-------------------------------------------------------------------
                // assegna un claim all'utente (es. id dell'utente) che verranno inserite nel database
                //userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.Uid, user.Id));
                //-------------------------------------------------------------------

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
        public async Task<ActionResult<AuthResponse>> Login(DtoLogginUser userDto)
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
                    return Unauthorized(userDto); // ritorna un errore 401

                //-------------------------------------------------------------------
                // genera il token per l'utente
                string tokenString = await GenerateToken(user);

                // crea un oggetto di tipo AuthResponse per ritornare il token
                var response = new AuthResponse
                {
                    Email = userDto.Email,
                    Token = tokenString,
                    UserId = user.Id
                };
                //-------------------------------------------------------------------

                // ritorna un messaggio di successo
                return Ok(response);
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

        //-------------------------------------------------------------------
        // metodo per la generazione del token
        private async Task<string> GenerateToken(ApiUser user)
        {
            // specifica la chiave di sicurezza per la generazione del token
            // stesso codice presente in program.cs nel builder di JWT
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]));

            // specifica i parametri per la generazione del token per il certificato
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // definizione delle regole per il token
            var roles = await userManager.GetRolesAsync(user);

            // definizione una lista informazioni per il token dalle regole
            var roleClaims = roles.Select(q => new Claim(ClaimTypes.Role, q)).ToList();

            // definizione delle informazioni per il token dal database
            var userClaims = await userManager.GetClaimsAsync(user);

            // genera le informazioni per il token che verranno inserite nel token (es. issuer, audience, scadenza)
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName), // potrebbe usare anche user.Email (sono uguali in questo caso)
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // specifica un id univoco per il token
                new Claim(JwtRegisteredClaimNames.Email, user.Email), // specifica l'email dell'utente
                // nome claim sara' "uid" e valore claim sara' l'id dell'utente (chiave = valore)
                new Claim(CustomClaimTypes.Uid, user.Id) // specifica l'id dell'utente definito nella classe CustomClaimTypes (nella cartella Static)
            }
            .Union(userClaims) // unione delle informazioni per il token dal database
            .Union(roleClaims); // unione delle informazioni per il token dalle regole

            // genera il token
            var token = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"], // specifica l'issuer del token
                audience: configuration["JwtSettings:Audience"], // specifica l'audience del token
                claims: claims, // specifica le informazioni per il token
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(configuration["JwtSettings:Duration"])), // specifica la scadenza del token
                signingCredentials: credentials // specifica le credenziali per la generazione del token
                );

            // ritorna il token generato convertito in stringa
            return new JwtSecurityTokenHandler().WriteToken(token); 
        }
        //-------------------------------------------------------------------
    }
}
