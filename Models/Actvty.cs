using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;


namespace EXAMcsharp.Models
{
    public class Actvty 
    {
        [Key]
        public int ActvtyId { get; set; }


        [Required]
        [MinLength(2)]
        [Display(Name="Activity")]
        public string Title { get; set; }

        [Required]
        [FutureDateTime]
        public DateTime Date {get;set;}

        [Required]
        public string Duration { get; set; }


        [Required]
        public string Coordinator { get; set; }


        [Required]
        [MinLength(10, ErrorMessage = "Description cannot be less than 10 characters")]
        public string Description { get; set; }


        public User Creator {get;set;}

        public int UserId {get;set;}
        
        public List<Membership> Memberships {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
    public class FutureDateTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(((DateTime)value) <= DateTime.Now)
            {
                return new ValidationResult("Only dates/times in the future are allowed");
            }
            return ValidationResult.Success;
        }
    }
}