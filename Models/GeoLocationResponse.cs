using System.ComponentModel.DataAnnotations;

namespace BlockedCountriesAPI.Models
{
    public class GeoLocationResponse
    {
        [Required]
        [RegularExpression(@"^(\d{1,3}\.){3}\d{1,3}$", ErrorMessage = "Invalid IP format.")]
        public string IP { get; set; }

        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string CountryCode { get; set; }

        public string CountryName { get; set; }

        public string ISP { get; set; }
    }
}
