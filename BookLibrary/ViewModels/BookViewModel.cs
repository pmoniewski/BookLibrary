using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BookLibrary.Models;

namespace BookLibrary.ViewModels
{
    public class BookViewModel
    {
        #region Simple properties

        [Key]
        public int Id { get; set; }

        [Display(Name = "ISBN")]
        [RegularExpression(@"^(?:ISBN(?:-1[03])?:? )?(?=[0-9X]{10}$|(?=(?:[0-9]+[- ]){3})[- 0-9X]{13}$|97[89][0-9]{10}$|(?=(?:[0-9]+[- ]){4})[- 0-9]{17}$)(?:97[89][- ]?)?[0-9]{1,5}[- ]?[0-9]+[- ]?[0-9]+[- ]?[0-9X]$", ErrorMessage = "The number entered in the ISBN field is not valid.")]
        public string Isbn { get; set; }

        [Required]
        public string Title { get; set; }

        public string Author { get; set; }

        public Status Status { get; set; }

        #endregion


        #region Navigation properties

        public virtual IList<RentalViewModel> Rentals { get; set; }

        #endregion
    }
}
