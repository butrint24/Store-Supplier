using System;
using System.ComponentModel.DataAnnotations;

namespace StoreSupplier.Models
{
    public class Branch
    {
        public int Id { get; set; }
        [Required, MinLength(4, ErrorMessage = "Branch name requires at least 4 characters")]
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Sorting { get; set; }
    }
}
