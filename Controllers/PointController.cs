using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MapApplication.Data;
using MapApplication.Services;
using MapApplication.Services.Responses;
using System.Drawing;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
            var response = _unitOfWork.Point.Add(point);
            _unitOfWork.Save();
            return response;
        }

        //[HttpPost]
        //public async Task<IActionResult> SavePoint([FromBody] Point point) {
        //    if (point == null || string.IsNullOrEmpty(point.Name)) {
        //        return BadRequest("Invalid point data.");
        //    }

        //    try {
        //        var result = _unitOfWork.Point.Add(point);
        //        _unitOfWork.Save();

        //        return Ok(new { message = "Point saved successfully." });
        //    } catch (Exception ex) {
        //        // Log the exception (not shown here)
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}

        #region API CALLS

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetAllJSON() {
            ApiResponse<List<Point>> ObjPointList = _unitOfWork.Point.GetAll();
            if (ObjPointList.Data != null) {
                return Json(new { data = ObjPointList.Data.ToList() });
            }
            else {
                return NotFound();
            }
            
        }

        #endregion

    }
}
