using Paramore.Brighter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KeylessGateways.Services.Shared.EventBus
{
    public class UserDoorCreatedUpdatedEvent 
    {
        
        public Guid Id { get; set; }

        public Guid DoorId { get; set; }

        public Guid UserId { get; set; }

        public bool IsTimeLimited { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    public class UserDoorDeletedEvent
    {
        public Guid Id { get; set; }
    }
}
