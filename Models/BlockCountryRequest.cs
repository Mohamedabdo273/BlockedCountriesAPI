using System.ComponentModel.DataAnnotations;

namespace BlockedCountriesAPI.Models
{
    public class BlockCountryRequest
    {
        [Required]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Country code must be exactly 2 characters.")]
        public string CountryCode { get; set; }
    }
}
