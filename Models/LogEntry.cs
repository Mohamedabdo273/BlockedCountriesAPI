using System.ComponentModel.DataAnnotations;

namespace BlockedCountriesAPI.Models
{
    public class LogEntry
    {
        public int Id { get; set; }

        [Required]
        public string IpAddress { get; set; }

        public DateTime AttemptTime { get; set; }

        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string CountryCode { get; set; }

        public string UserAgent { get; set; }

        public bool BlockedStatus { get; set; } // True if blocked, false otherwise.
    }
}
