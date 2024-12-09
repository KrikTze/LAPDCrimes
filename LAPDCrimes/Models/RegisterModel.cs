using LAPDCrimes.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace LAPDCrimes.Models
{
    public class RegisterModel
    {
        [Required]
        [MinLength(5)]
        [MaxLength(20)]
        public string username { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "This is not a valid email")]
        [MaxLength(255, ErrorMessage = ("Wrong Email Size"))]
        public string Email { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "First Name must be at least 5 characters")]
        [MaxLength(30, ErrorMessage = "First Name must be smaller")]

        public string firstName { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "Last Name must be at least 5 characters")]
        [MaxLength(30, ErrorMessage = "Last Name must be smaller")]

        public string lastName { get; set; }

        [Required]
        public GenderEnum gender { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(15)]
        [DataType(DataType.Password)]

        public string password { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(15)]
        [DataType(DataType.Password)]

        [Compare("password", ErrorMessage = "Passwords do not match")]
        public string confirmPassword { get; set; }

        public bool con_pass()
        {
            if (password == confirmPassword)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
