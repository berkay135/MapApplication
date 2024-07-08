

using MapApplication.Data;
using MapApplication.Services.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MapApplication.Services {
    public class PointRepository : IPointRepository {

        public ApiResponse<List<Point>> GetAll() {
            try {
                //validation boş ise hata
                //her seferinde new ApiResponse demeye gerek yok
                var points = TestData.points;
                var response = new ApiResponse<List<Point>>(true, "Points retrieved successfully", points);
                return response;

            } catch (Exception ex) {

                Console.Error.WriteLine($"An error occurred: {ex.Message}");

                var errorResponse = new ApiResponse<List<Point>>(false, "Unexpected Error", null);
                return errorResponse;
            }
            
        }

        public ApiResponse<Point> GetById(int Id) {
            try {
                if (Id <= 0) {
                    var badResponse = new ApiResponse<Point>(false, "Invalid Id!", null);
                    return badResponse;
                }

                var point = TestData.points.FirstOrDefault(p => p.Id == Id);
                if (point == null) {
                    var notFoundResponse = new ApiResponse<Point>(false, "Not Found", null);
                    return notFoundResponse;
                }

                var response = new ApiResponse<Point>(true, "Point retrieved successfully", point);
                return response;
            } catch (Exception ex) {

                Console.Error.WriteLine($"An error occurred: {ex.Message}");

                var errorResponse = new ApiResponse<Point>(false, "Unexpected Error", null);
                return errorResponse;
            }
            
        }
            
        public ApiResponse<Point> Update(int id, Point point) {
            try {
                if (point == null || id != point.Id) {
                    var badResponse = new ApiResponse<Point>(false, "Invalid Id!", null);
                    return badResponse;
                }
                Point pointFromDb = TestData.points.FirstOrDefault(x => x.Id == id);
                if (pointFromDb == null) {
                    var notFoundResponse = new ApiResponse<Point>(false, "Not Found", null);
                    return notFoundResponse;
                }
                pointFromDb.X = point.X;
                pointFromDb.Y = point.Y;
                pointFromDb.Name = point.Name;

                var response = new ApiResponse<Point>(true, "Point updated succesfully", pointFromDb);
                return response;
            } catch (Exception ex) {

                Console.Error.WriteLine($"An error occurred: {ex.Message}");

                var errorResponse = new ApiResponse<Point>(false, "Unexpected Error", null);
                return errorResponse;
            }
            
        }

        public ApiResponse<Point> Delete(int id) {
            try {
                var item = TestData.points.FirstOrDefault(u => u.Id == id);

                if (item == null) {

                    var notFoundResponse = new ApiResponse<Point>(false, "Not Found", null);
                }

                TestData.points.Remove(item);
                var response = new ApiResponse<Point>(true, "Point deleted succesfully", item);
                return response;
            } catch (Exception ex) {

                Console.Error.WriteLine($"An error occurred: {ex.Message}");

                var errorResponse = new ApiResponse<Point>(false, "Unexpected Error", null);
                return errorResponse;
            }

            
        }

        public ApiResponse<Point> Add(Point point) {
            try {
                //if (TestData.points.FirstOrDefault(x => x.Name.ToLower() == point.Name.ToLower()) != null) {
                //    return new ApiResponse<Point>(false, "Point already exists!", point);
                //}

                var Point = new Point();

                //liste boş ise geç
                point.Id = TestData.points.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;

                TestData.points.Add(point);

                var response = new ApiResponse<Point>(true, "Point added succesfully", point);
                return response;

            } catch (Exception ex) {

                Console.Error.WriteLine($"An error occurred: {ex.Message}");

                var errorResponse = new ApiResponse<Point>(false, "Unexpected Error", null);
                return errorResponse;
            }

        }
    }

}
