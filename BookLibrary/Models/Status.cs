using System;
using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Models
{
    public enum Status
    {
        [Display(Name = "Unknown")]
        Unknown = 0,

        [Display(Name = "Available")]
        Available,

        [Display(Name = "Rented")]
        Rented
    }
}
