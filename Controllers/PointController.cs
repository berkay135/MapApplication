using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MapApplication.Data;
using MapApplication.Services;
using MapApplication.Services.Responses;
using System.Drawing;

namespace MapApplication.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class PointController : Controller {

        private readonly IUnitOfWork _unitOfWork;

        public PointController(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public ApiResponse<List<Point>> GetAll() {
            return _unitOfWork.Point.GetAll();
        }

        [HttpGet("{id}")]
        public ApiResponse<Point> GetById(int id) {
            return _unitOfWork.Point.GetById(id);
        }


        [HttpPut("{id}")]
        public ApiResponse<Point> Update(int id,Point point) {
            var response = _unitOfWork.Point.Update(id, point);
            _unitOfWork.Save();
            return response;
        }

        [HttpDelete("Delete/{id}")]
        public ApiResponse<Point> Delete(int id) {
            var response = _unitOfWork.Point.Delete(id);
            _unitOfWork.Save();
            return response;
            
        }

        [HttpPost]
        public ApiResponse<Point> Add(Point point) {
            var response =  _unitOfWork.Point.Add(point);
            _unitOfWork.Save();
            return response;
        }

    }
}
