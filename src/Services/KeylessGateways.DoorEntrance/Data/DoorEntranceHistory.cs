using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KeylessGateways.DoorEntrance.Data
{
    public class DoorEntranceHistory
    {
        public long Id { get; set; }
        public long DoorId { get; set; }

        public long UserId { get; set; }

        public DateTime EntranceTime { get; set; }
    }
}
