using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Models
{
    public class Rental
    {
        #region Simple properties

        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        #endregion


        #region Navigation properties

        [Required]
        public int BookId { get; set; }

        [Required]
        public Book Book { get; set; }

        #endregion
    }
}
