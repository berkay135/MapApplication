using MapApplication.Services.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MapApplication.Services {
    public interface IPointRepository : IRepository<Point> {

        ApiResponse<Point> Update(int id, Point point);

    }
}
