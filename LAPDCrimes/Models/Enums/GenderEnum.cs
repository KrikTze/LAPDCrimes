using System.ComponentModel.DataAnnotations;

namespace LAPDCrimes.Models.Enums
{
    public enum GenderEnum
    {
        Male,
        Female,
        [Display(Name = "Non-Binary")]
        NonBinary,
        Other
    }
}
