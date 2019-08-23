using System.ComponentModel.DataAnnotations;
using System;

namespace EXAMcsharp.Models
{
    public class LoginUser
    {
        
        [Required]
        [EmailAddress]
        public string LoginEmail {get;set;}

        [Required]
        [DataType(DataType.Password)]
        public string LoginPassword {get;set;}


    }
    
}