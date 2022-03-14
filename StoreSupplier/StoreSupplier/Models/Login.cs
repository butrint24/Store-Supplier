using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSupplier.Models
{
    public class Login
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password), Required, MinLength(8, ErrorMessage = "Minimum length is 8")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}