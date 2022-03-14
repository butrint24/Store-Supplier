﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace StoreSupplier.Models
{
    public class User
    {
        [Required, MinLength(2, ErrorMessage = "Minimum length is 2")]
        public string Name { get; set; }

        [Display(Name = "Last Name")]
        [Required, MinLength(2, ErrorMessage = "Minimum length is 2")]
        public string LastName { get; set; }

        [Required, MinLength(2, ErrorMessage = "Minimum length is 2")]

        [Display(Name = "Username")]
        public string UserName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password), Required, MinLength(8, ErrorMessage = "Minimum length is 8")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime BirthDate { get; set; }

        [Required, MinLength(10, ErrorMessage = "Minimum length is 10 digits"), MaxLength(10, ErrorMessage = "Maximum length is 10 digits")]
        [Display(Name = "Personal Number")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Only digits are allowed")]
        public string PersonalNumber { get; set; }
        [Display(Name = "Branch")]
        [Range(1, int.MaxValue, ErrorMessage = "You must chose a branch!")]
        public int BranchId { get; set; }


    }
}
