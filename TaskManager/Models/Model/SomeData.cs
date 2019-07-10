using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Models
{
    public class SomeData
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("data1")]
        public string Data1 { get; set; }

        [Column("data2")]
        public string Data2 { get; set; }
    }
}
