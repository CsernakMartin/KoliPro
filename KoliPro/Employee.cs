using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Postgrest.Attributes;
using Postgrest.Models;

namespace KoliPro
{
    [Table("employees")]
    public class Employee : BaseModel
    {
        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("username")]
        public string Username { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("full_name")]
        public string FullName { get; set; }

        [Column("role")]
        public string Role { get; set; }
    }
}
