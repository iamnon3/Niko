using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trak_IT.Scripts
{
    [Table("user")]
    public class UserModel
    {
        [PrimaryKey]
        [AutoIncrement]
        [Column("id")]
        public int Id { get; set; }
        [Column("user")]
        public string Username { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("user_role")]
        public string User_Role { get; set; }
        [Column("photo")]
        public string? PhotoString { get; set; }
    }
}
