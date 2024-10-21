using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class UserUpdateDto
    {
            public string Id { get; set; }
            [Required]
            public string FirstName { get; set; }
            public string LastName { get; set; }

            [Required]
            public bool IsRecieveOutbidEmails { get; set; }

        }
    
}
