using System.ComponentModel.DataAnnotations;

namespace BlockedCountriesAPI.Models
{
    public class GeoLocationResponse
    {
        [Required]
        public string IP { get; set; }  // Ensure the IP is captured

        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string CountryCode { get; set; }  // Country code (two-letter)

        public string CountryName { get; set; }  // Name of the country

        public string ISP { get; set; }  // Internet Service Provider
        //public string continent_code { get; set; }  // Country code (two-letter)

        //public string continent_name { get; set; }  // Name of the country
    }
}
