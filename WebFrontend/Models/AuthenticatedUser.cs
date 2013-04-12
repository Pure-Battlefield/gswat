using System.Collections.Generic;

namespace WebFrontend.Models
{
    public class AuthenticatedUser
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string BattlelogId { get; set; }
        public string Namespace { get; set; }
        public List<string> Permissions { get; set; }
    }
}