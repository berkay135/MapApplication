using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MapApplication {
    public class Point {

        [Key]
        public int Id { get; set; }
        public string Wkt { get; set; }
        public string Name { get; set; }
    }
}
