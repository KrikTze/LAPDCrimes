using System.ComponentModel.DataAnnotations;

namespace LAPDCrimes.Models
{
    public class LoginModel
    {
        [Required]
        [MinLength(5, ErrorMessage = "Username must be at least 5 characters")]
        [MaxLength(20)]
        public string username { get; set; }
        [Required]
        [StringLength(100)]
        public string password { get; set; }
    }
}
