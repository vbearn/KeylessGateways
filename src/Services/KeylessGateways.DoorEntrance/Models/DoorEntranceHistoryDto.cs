using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KeylessGateways.DoorEntrance.Models
{
    public class DoorEntranceHistoryDto
    {
        public long Id { get; set; }
        public long DoorId { get; set; }

        public long UserId { get; set; }

        public DateTime EntranceTime { get; set; }

    }

    public class DoorEntranceHistoryFilterDto
    {
        public DateTime? StartEntranceTime { get; set; }
        public DateTime? EndEntranceTime { get; set; }

    }
}