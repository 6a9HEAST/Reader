using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reader.Models
{
    public class Book
    {
        public string Id { get; set; }
        public string? Cover { get; set; }
        public string Name { get; set; }
        public string? Author { get; set; }
        public string? Collection { get; set; }
        public string? Type { get; set; }
        public float Size { get; set; }
    }
}
