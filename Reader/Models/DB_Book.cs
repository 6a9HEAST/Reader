using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reader.Models
{
    public class DB_Book
    {
        public string Id { get; set; }
        public string? CoverPath { get; set; }
        public string Name { get; set; }
        public string? Author { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string Path { get; set; }
        public float Progress { get; set; }
        public DateTime LastTimeOpened { get; set; }
    }
}
