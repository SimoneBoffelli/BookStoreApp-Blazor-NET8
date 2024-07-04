using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.API.Models.User
{
    public class DtoUser : DtoLogginUser // eredita dalla classe DtoLogginUser
    {
        [Required]
        [MinLength(3)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(3)]
        public string LastName { get; set; }

        // il ruolo dell'utente (es. admin, user) per controllare i permessi nel controller
        // Viene automaticamente settato a "User" durante la creazione dal controller
        // perche' non e' possibile specificare il ruolo nel form di registrazione
        [Required]
        public string Role { get; set; }
    }
}
