using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KeylessGateways.Common;
using KeylessGateways.DoorEntrance.Data;
using KeylessGateways.DoorEntrance.Models;
using Microsoft.Extensions.Logging;

namespace KeylessGateways.DoorEntrance.Services
{
    public class DoorAccessPolicyFactoryService : IDoorAccessPolicyFactoryService
    {
        private readonly IEnumerable<IDoorAccessPolicy> _doorAccessPolicies;

        public DoorAccessPolicyFactoryService(
         IEnumerable<IDoorAccessPolicy> doorAccessPolicies
     )
        {
            _doorAccessPolicies = doorAccessPolicies;
        }

        /// <summary>
        /// Returns all DoorAccessPolicies (the policies to determine if user has priviledge to
        /// open a specific door), making sure they are sorted by their respective order 
        /// These policies have already been registed in the system through DI.
        /// </summary>
        public List<IDoorAccessPolicy> GetSortedDoorAccessPolicies()
        {
            var sortedAccessPolicies = _doorAccessPolicies.ToList();
            sortedAccessPolicies.Sort((x, y) => x.Order - y.Order);

            return sortedAccessPolicies;
        }

    }
}