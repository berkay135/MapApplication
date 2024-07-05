using MapApplication.Services.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MapApplication.Services {
    public interface IPointRepository {
        public ApiResponse<List<Point>> GetAll();
        public ApiResponse<Point> GetById(int Id);
        public ApiResponse<Point> Update(int id, Point point);
        public ApiResponse<Point> Delete(int id);
        public ApiResponse<Point> Add(Point point);
    }
}
