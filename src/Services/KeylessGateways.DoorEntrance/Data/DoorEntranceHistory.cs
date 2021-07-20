using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KeylessGateways.DoorEntrance.Data
{
    public class DoorEntranceHistory
    {
        public Guid Id { get; set; }
        public Guid DoorId { get; set; }

        public Guid UserId { get; set; }

        public DateTime EntranceTime { get; set; }
    }
}
