using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trak_IT.Scripts
{
    public class UsersProfile
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("user_role")]
        public string User_role { get; set; }
        [Column("profile")]
        public string? Photostring { get; set; }
    }
}
