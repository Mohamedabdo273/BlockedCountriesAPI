using System.ComponentModel.DataAnnotations;

namespace BlockedCountriesAPI.Models
{
    public class BlockCountryRequest
    {
        [Required]
        [StringLength(2)] // Assuming country codes are 2 characters long
        public string CountryCode { get; set; }
    }

}
