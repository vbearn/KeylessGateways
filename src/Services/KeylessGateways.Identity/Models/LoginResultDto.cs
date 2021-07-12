namespace KeylessGateways.Identity.Models
{
    public class LoginResultDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }

        public string Role { get; set; }

        public string AccessToken { get; set; }
    }
}