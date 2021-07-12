using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KeylessGateways.Management.Models
{
    public class DoorDto
    {
        public long Id { get; set; }
        public string Name { get; set; }

    }
    public class DoorUpdateDto
    {
        public string Name { get; set; }

    }
}