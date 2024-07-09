using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MapApplication {
    public class Point {

        [Key]
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public string Name { get; set; }
    }
}
