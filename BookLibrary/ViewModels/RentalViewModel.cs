using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BookLibrary.Models;

namespace BookLibrary.ViewModels
{
    public class RentalViewModel
    {
        #region Simple properties

        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime BeginDate { get; set; }

        public DateTime? EndDate { get; set; }

        #endregion


        #region Navigation properties

        [Required]
        public int BookId { get; set; }

        [Required]
        public BookViewModel Book { get; set; }

        #endregion
    }
}
