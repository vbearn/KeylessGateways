using Paramore.Brighter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KeylessGateways.Services.Shared.EventBus
{
    public class UserDoorCreatedEvent 
    {
        
        public long DoorId { get; set; }

        public long UserId { get; set; }

        public bool IsTimeLimited { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    public class UserDoorUpdatedEvent : UserDoorCreatedEvent
    {
        public long Id { get; set; }
    }
    
    public class UserDoorDeletedEvent
    {
        public long Id { get; set; }
    }
}
