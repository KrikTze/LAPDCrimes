using LAPDCrimes.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LAPDCrimes.Models
{
    public class CrimeUser : IdentityUser
    {
        [Required]
        [MaxLength(30)]
        [MinLength(5)]
        public string firstName { get; set; }
        [Required]
        [MaxLength(30)]
        [MinLength(5)]
        public string lastName { get; set; }

        [Required]
        public GenderEnum gender { get; set; }
    }
}
