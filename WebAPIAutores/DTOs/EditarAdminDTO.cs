using System.ComponentModel.DataAnnotations;

namespace WebAPIAutores.DTOs
{
    public class EditarAdminDTO
    {
        [Required]
        public string Email{ get; set; }
    }
}
