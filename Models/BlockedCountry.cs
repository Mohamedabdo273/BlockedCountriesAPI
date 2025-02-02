﻿using System.ComponentModel.DataAnnotations;

namespace BlockedCountriesAPI.Models
{
    public class BlockedCountry
    {
        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string CountryCode { get; set; }
     
        public string CountryName { get; set; }
      
        public DateTime BlockDate { get; set; } = DateTime.UtcNow;

        public DateTime? BlockExpiryDate { get; set; }

        public bool IsExpired => BlockExpiryDate.HasValue && BlockExpiryDate.Value <= DateTime.UtcNow;
    }
}
