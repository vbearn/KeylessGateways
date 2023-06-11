using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KeylessGateways.Identity.Models
{
   
    public class UserDto {

        public Guid Id { get; set; }

        public string Email { get; set; }

        public bool Admin { get; set; }

        public string FullName { get; set; }

    }
 
    public class UserCreateUpdateDto {

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public bool Admin { get; set; }

        public string FullName { get; set; }

    }
}