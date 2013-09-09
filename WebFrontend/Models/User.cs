using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebFrontend.Models
{
    public class User
    {
        public long GoogleId { get; set; }
        public string Email { get; set; }
        public string Namespace { get; set; }
        public List<string> Permissions { get; set; }
    }
}