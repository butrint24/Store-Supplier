using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSupplier.Models
{
    public class AppUser : IdentityUser
    {
        [Required, MinLength(2, ErrorMessage = "Minimum length is 2")]
        public string Name { get; set; }
        [Required, MinLength(2, ErrorMessage = "Minimum length is 2")]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"^(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d$", ErrorMessage = "Chose a proper Date")]
        public DateTime BirthDate { get; set; }

        [Required, MinLength(10, ErrorMessage = "Minimum length is 10 digits"), MaxLength(10, ErrorMessage = "Maximum length is 10 digits")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Only digits are allowed")]
        public string PersonalNumber { get; set; }
        [Display(Name = "Branch")]
        [Range(1, int.MaxValue, ErrorMessage = "You must chose a branch!")]
        public int BranchId { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }
    }
}
