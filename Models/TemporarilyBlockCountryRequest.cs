using System.ComponentModel.DataAnnotations;

namespace BlockedCountriesAPI.Models
{
    public class TemporarilyBlockCountryRequest
    {
        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string CountryCode { get; set; }

        [Required]
        [Range(1, 1440, ErrorMessage = "Duration must be between 1 and 1440 minutes.")]
        public int DurationMinutes { get; set; }
    }
}
