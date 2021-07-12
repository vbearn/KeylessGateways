using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KeylessGateways.DoorEntrance.Models
{
    public class OpenDoorDto
    {
        public long UserId { get; set; }
        public long DoorId { get; set; }

    }
    public class OpenDoorExtendedModel : OpenDoorDto
    {
        public long CurrentContextUserId { get; set; }
        public string CurrentContextUserRole { get; set; }

    }
}