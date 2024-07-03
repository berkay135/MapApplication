using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MapApplication.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class PointController : ControllerBase {

        private static readonly List<Point> points = new List<Point>();

        [HttpGet]

        public List<Point> GetAll() {
            return points;
        }

        [HttpGet("GetById")]
        public ActionResult<Point> GetById(int Id) {
            if (Id <= 0) {
                return NotFound("Invalid Id!");
            }

            Point point = points.FirstOrDefault(u => u.Id == Id);
            if (point == null) {
                return NotFound("Not Found");
            }

            return point;

        }

        
        [HttpPost("Update")]
        public ActionResult Update(Point point) {

            var item = points.FirstOrDefault(u => u.Id == point.Id);

            if (item == null) {

                return NotFound();
            }

            item.X = point.X;
            item.Y = point.Y;
            item.Name = point.Name;

            return Ok(item);
        }

        [HttpDelete("Delete/{id}")]
        public ActionResult Delete(int id) {

            var item = points.FirstOrDefault(u => u.Id == id);

            if (item == null) {

                return NotFound();
            }

            points.Remove(item);

            return Ok($"Deleted Item's Name: {item.Name}");
        }

        [HttpPost]
        public Point Add(Point point) {
            var item = new Point();

            item.Id = point.Id;
            item.X = point.X;
            item.Y = point.Y;
            item.Name = point.Name;
            points.Add(item);

            return item;
        }

    }
}
