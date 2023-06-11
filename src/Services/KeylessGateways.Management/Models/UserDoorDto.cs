using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KeylessGateways.Management.Models
{

    public class UserDoorCreateUpdateDto
    {
        [Required]
        public Guid DoorId { get; set; }
        
        [Required]
        public Guid UserId { get; set; }

        public bool IsTimeLimited { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
    public class UserDoorDto : UserDoorCreateUpdateDto
    {
        public Guid Id { get; set; }
    }
}