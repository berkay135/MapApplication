

using MapApplication.Data;
using MapApplication.Services.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;
using System.Data.SqlClient;

namespace MapApplication.Services {
    public class PointRepository : IPointRepository {

        private AppDbContext _db;
        public PointRepository(AppDbContext db) {
            _db = db;
        }
        string connectionString = "Host=localhost; Database=MapApplication; Username=postgres; Password=admin";

        readonly ApiResponse<List<Point>> customResponse = new ApiResponse<List<Point>>(false, null, null);

        public ApiResponse<List<Point>> GetAll() {
            try {
                //validation boş ise hata
                if (_db.Points == null) {
                    customResponse.Success = false;
                    customResponse.Message = "List is empty";
                    return customResponse;
                }
                //her seferinde new ApiResponse demeye gerek yok ??
                var points = _db.Points.ToList();
                customResponse.Success = true;
                customResponse.Message = "Points retrieved successfully";
                customResponse.Data = points;
                return customResponse;

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

                var point = _db.Points.ToList().FirstOrDefault(p => p.Id == Id);
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
                Point pointFromDb = _db.Points.ToList().FirstOrDefault(x => x.Id == id);
                if (pointFromDb == null) {
                    var notFoundResponse = new ApiResponse<Point>(false, "Not Found", null);
                    return notFoundResponse;
                }
                pointFromDb.X = point.X;
                pointFromDb.Y = point.Y;
                pointFromDb.Name = point.Name;

                _db.SaveChanges();

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
                var item = _db.Points.ToList().FirstOrDefault(u => u.Id == id);

                if (item == null) {

                    var notFoundResponse = new ApiResponse<Point>(false, "Not Found", null);
                }

                _db.Points.Remove(item);
                _db.SaveChanges();
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
                var Point = new Point();

                //liste boş ise geç
                if (_db.Points.ToList() != null) {
                    point.Id = _db.Points.ToList().OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
                }


                _db.Points.Add(point);
                _db.SaveChanges();
                var succesResponse = new ApiResponse<Point>(true, "Point added succesfully", point);
                return succesResponse;


                //var errorResponse = new ApiResponse<Point>(false, "Invalid Id", point);
                //return errorResponse;

            } catch (Exception ex) {

                Console.Error.WriteLine($"An error occurred: {ex.Message}");

                var errorResponse = new ApiResponse<Point>(false, "Unexpected Error", null);
                return errorResponse;
            }

        }
        //CRUD OPERATIONS WITH SQL
        //    public ApiResponse<Point> Add(Point newPoint) {
        //        ApiResponse<Point> response = new ApiResponse<Point>(false, "Add failed", null);

        //        try {
        //            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString)) {
        //                conn.Open();

        //                string sql = "INSERT INTO \"Points\" (\"Name\", \"X\", \"Y\") VALUES (@Name, @X, @Y) RETURNING \"Id\"";
        //                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn)) {
        //                    cmd.Parameters.AddWithValue("@Name", newPoint.Name);
        //                    cmd.Parameters.AddWithValue("@X", newPoint.X);
        //                    cmd.Parameters.AddWithValue("@Y", newPoint.Y);

        //                    object result = cmd.ExecuteScalar();
        //                    if (result != null && int.TryParse(result.ToString(), out int insertedId)) {
        //                        newPoint.Id = insertedId;
        //                        response.Success = true;
        //                        response.Message = "Add successful";
        //                        response.Data = newPoint;
        //                    }
        //                    else {
        //                        response.Message = "Failed to retrieve inserted Id";
        //                    }
        //                }
        //            }
        //        } catch (NpgsqlException ex) {
        //            response.Message = $"Error: {ex.Message}";
        //            Console.WriteLine($"Error: {ex.Message}");
        //        } 
        //        return response;
        //    }
        //}

        //public ApiResponse<Point> Delete(int id) {
        //    ApiResponse<Point> response = new ApiResponse<Point>(false, "Delete failed", null);

        //    try {
        //        using (NpgsqlConnection conn = new NpgsqlConnection(connectionString)) {
        //            conn.Open();

        //            string sql = "DELETE FROM \"Points\" WHERE \"Id\" = @Id";
        //            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn)) {
        //                cmd.Parameters.AddWithValue("@Id", id);

        //                int rowsAffected = cmd.ExecuteNonQuery();
        //                if (rowsAffected > 0) {
        //                    response.Success = true;
        //                    response.Message = "Delete successful";
        //                    response.Data = null;
        //                }
        //                else {
        //                    response.Message = "No records deleted or not found";
        //                }
        //            }
        //        }
        //    } catch (NpgsqlException ex) {
        //        response.Message = $"Error: {ex.Message}";
        //        Console.WriteLine($"Error: {ex.Message}");
        //    } catch (Exception ex) {
        //        response.Message = $"Error: {ex.Message}";
        //        Console.WriteLine($"Error: {ex.Message}");
        //    }

        //    return response;
        //}

        //public ApiResponse<Point> Update(Point updatedPoint) {
        //    ApiResponse<Point> response = new ApiResponse<Point>(false, "Update failed", null);

        //    try {
        //        using (NpgsqlConnection conn = new NpgsqlConnection(connectionString)) {
        //            conn.Open();

        //            string sql = "UPDATE \"Points\" SET \"Name\" = @Name, \"X\" = @X, \"Y\" = @Y WHERE \"Id\" = @Id";
        //            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn)) {
        //                cmd.Parameters.AddWithValue("@Id", updatedPoint.Id);
        //                cmd.Parameters.AddWithValue("@Name", updatedPoint.Name);
        //                cmd.Parameters.AddWithValue("@X", updatedPoint.X);
        //                cmd.Parameters.AddWithValue("@Y", updatedPoint.Y);

        //                int rowsAffected = cmd.ExecuteNonQuery();
        //                if (rowsAffected > 0) {
        //                    response.Success = true;
        //                    response.Message = "Update successful";
        //                    response.Data = updatedPoint;
        //                }
        //                else {
        //                    response.Message = "No records updated";
        //                }
        //            }
        //        }
        //    } catch (NpgsqlException ex) {
        //        response.Message = $"Error: {ex.Message}";
        //        Console.WriteLine($"Error: {ex.Message}");
        //    } 

        //    return response;
        //}

        //public ApiResponse<Point> GetById (int id) {
        //    ApiResponse<Point> points = new ApiResponse<Point>(false, "Not Found", null);

        //    try {

        //        using (NpgsqlConnection conn = new NpgsqlConnection(connectionString)) {
        //            conn.Open();

        //            NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"Points\" WHERE \"Id\"=@id", conn);
        //            cmd.Parameters.AddWithValue("@id", id);

        //            using (NpgsqlDataReader reader = cmd.ExecuteReader()) {
        //                if (reader.Read()) {
        //                    Point point = new Point {
        //                        Id = Convert.ToInt32(reader["Id"]),
        //                        Name = reader["Name"].ToString(),
        //                        X = Convert.ToDouble(reader["x"]),
        //                        Y = Convert.ToDouble(reader["y"])
        //                    };
        //                    points.Success = true;
        //                    points.Message = "Data retrieved successfully";
        //                    points.Data = point;
        //                }
        //            }
        //        }
        //    } catch (NpgsqlException ex) {
        //        points.Success = false;
        //        points.Message = $"Error: {ex.Message}";
        //        Console.WriteLine($"Error: {ex.Message}");
        //    }
        //    return points;
        //}

        //public ApiResponse<List<Point>> GetAll() {
        //    ApiResponse<List<Point>> points = new ApiResponse<List<Point>>(false, "Not Found", null);

        //    List<Point> pointList = new List<Point>();
        //    try {

        //        using (NpgsqlConnection conn = new NpgsqlConnection(connectionString)) {
        //            conn.Open();

        //            NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"Points\"", conn);
        //            NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
        //            DataTable dt = new DataTable();
        //            da.Fill(dt);


        //            foreach (DataRow row in dt.Rows) {
        //                Point point = new Point();

        //                point.Id = Convert.ToInt32(row["Id"]);
        //                point.Name = row["Name"].ToString();
        //                point.X = Convert.ToDouble(row["x"]);
        //                point.Y = Convert.ToDouble(row["y"]);

        //                pointList.Add(point);
        //            }

        //            if (pointList.Count == 0) {
        //                points.Message = "No points found";
        //                return points;
        //            }

        //            points.Success = true;
        //            points.Message = "Data retrieved successfully";
        //            points.Data = pointList;
        //        }
        //    } catch (NpgsqlException ex) {
        //        points.Success = false;
        //        points.Message = $"Error: {ex.Message}";
        //        Console.WriteLine($"Error: {ex.Message}");
        //    }
        //    return points;
        //}


    }
}

