using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KeylessGateways.DoorEntrance.Models
{
    public class DoorEntranceHistoryDto
    {
        public Guid Id { get; set; }
        public Guid DoorId { get; set; }

        public Guid UserId { get; set; }

        public DateTime EntranceTime { get; set; }

    }

    public class DoorEntranceHistoryFilterDto
    {
        public DateTime? StartEntranceTime { get; set; }
        public DateTime? EndEntranceTime { get; set; }

    }
}