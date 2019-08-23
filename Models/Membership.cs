using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;


namespace EXAMcsharp.Models
{
    public class Membership 
    {
        [Key]
        public int MembershipId { get; set; }

        public int UserId { get ; set; }  
        public int ActvtyId { get; set; } 
        public User User{ get; set;}  

        public Actvty Actvty { get; set; }

    }
}
