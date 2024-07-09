using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MapApplication.Data;
using MapApplication.Services;
using MapApplication.Services.Responses;
using System.Drawing;

namespace MapApplication.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class PointController : ControllerBase {

        private readonly IPointRepository _pointRepository;

        public PointController(IPointRepository pointRepository) {
            _pointRepository = pointRepository;
        }

        [HttpGet]
        public ApiResponse<List<Point>> GetAll() {
            var response = _pointRepository.GetAll();
            return response;
        }

        [HttpGet("{id}")]
        public ApiResponse<Point> GetById(int id) {
            var response = _pointRepository.GetById(id);
            return response;
        }


        [HttpPut("{id}")]
        public ApiResponse<Point> Update(int id,Point point) {
            var response = _pointRepository.Update(id, point);
            return response;
        }

        [HttpDelete("Delete/{id}")]
        public ApiResponse<Point> Delete(int id) {
            var response = _pointRepository.Delete(id);
            return response;
        }

        [HttpPost]
        public ApiResponse<Point> Add(Point point) {
            var response = _pointRepository.Add(point);
            return response;
        }

    }
}
