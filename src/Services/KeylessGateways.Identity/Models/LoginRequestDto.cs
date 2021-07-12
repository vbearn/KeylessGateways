using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KeylessGateways.Identity.Models
{
    public class LoginRequestDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}