﻿using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.API.Models.Author
{
    public class DtoAuthorCreate
    {
        // in qusto caso non viene mostrato l'id all'utente
        // e anche la lista di libri non viene mostrata

        [Required]
        [StringLength(50)]
        public string? FirstName { get; set; }
        [Required]
        [StringLength(50)]
        public string? LastName { get; set; }
        [StringLength(250)]
        public string? Bio { get; set; }
    }
}
