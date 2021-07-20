using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KeylessGateways.DoorEntrance.Models
{
    public class OpenDoorDto
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid DoorId { get; set; }

    }
    public class OpenDoorExtendedModel : OpenDoorDto
    {
        public Guid CurrentContextUserId { get; set; }
        public string CurrentContextUserRole { get; set; }

    }
}