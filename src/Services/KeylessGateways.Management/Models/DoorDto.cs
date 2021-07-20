using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KeylessGateways.Management.Models
{
  
    public class DoorCreateUpdateDto
    {
        [Required]
        public string Name { get; set; }
    }

    public class DoorDto : DoorCreateUpdateDto
    {
        public Guid Id { get; set; }

    }
}