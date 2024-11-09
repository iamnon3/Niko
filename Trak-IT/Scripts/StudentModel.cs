using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trak_IT.Scripts
{
    [Table("student")]
    public class StudentModel
    {
        [PrimaryKey]
        [AutoIncrement]
        [Column("id")]
        public int Id { get; set; }
        [Column("bsitid")]
        public string Bsitid { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("section")]
        public string Section { get; set; }
        [Column("event")]
        public string Event { get; set; }
        [Column("time_in")]
        public string? Timein { get; set; }
        [Column("time_out")]
        public string? Timeout { get; set; }

    }
}
