using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.API.Models.User
{
    // superclasse per la registrazione dell'utente (e' separata perche'
    // i dati email e password sono necessari anche per il login)
    public class DtoLogginUser
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
