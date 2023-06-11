using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KeylessGateways.DoorEntrance.Data
{
    public class UserDoor
    {
        public Guid Id { get; set; }
        public Guid DoorId { get; set; }

        public Guid UserId { get; set; }

        public bool IsTimeLimited { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
