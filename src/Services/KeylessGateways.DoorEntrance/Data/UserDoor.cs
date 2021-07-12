using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KeylessGateways.DoorEntrance.Data
{
    public class UserDoor
    {
        public long Id { get; set; }
        public long DoorId { get; set; }

        public long UserId { get; set; }

        public bool IsTimeLimited { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
