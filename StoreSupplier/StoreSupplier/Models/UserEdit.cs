using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSupplier.Models
{
    public class UserEdit
    {
        [Required, MinLength(2, ErrorMessage = "Minimum length is 2")]
        public string Name { get; set; }

        [Display(Name = "Last Name")]
        [Required, MinLength(2, ErrorMessage = "Minimum length is 2")]
        public string LastName { get; set; }


        [Required, EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password), MinLength(8, ErrorMessage = "Minimum length is 8")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime BirthDate { get; set; }





        public UserEdit() { }

        public UserEdit(AppUser appUser)
        {
            Name = appUser.Name;
            LastName = appUser.LastName;
            Email = appUser.Email;
            Password = appUser.PasswordHash;
            BirthDate = appUser.BirthDate;
        }
    }
}
