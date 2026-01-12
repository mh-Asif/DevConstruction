using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.KeyValues
{
    public class UserKeyValues
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? BusinessOwner { get; set; }
        public string? Base64ProfileImage { get; set; }

        // [MaxLength]
        //public byte[]? ProfileImage { get; set; }


    }
}
