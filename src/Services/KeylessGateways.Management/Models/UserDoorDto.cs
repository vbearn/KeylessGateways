using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KeylessGateways.Management.Models
{

    public class UserDoorCreateUpdateDto
    {
        public long DoorId { get; set; }
        
        public long UserId { get; set; }

        public bool IsTimeLimited { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
    public class UserDoorDto : UserDoorCreateUpdateDto
    {
        public long Id { get; set; }
    }
}