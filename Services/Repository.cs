using MapApplication.Data;
using MapApplication.Services.Responses;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MapApplication.Services {
    public class Repository<T> : IRepository<T> where T : class {

        private readonly AppDbContext _db;
        internal DbSet<T> dbSet;
        public Repository(AppDbContext db) {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        public ApiResponse<T> Add(T entity) {
            try {
                dbSet.Add(entity);
                return ApiResponse<T>.SuccessResponse("Point Successfuly Added", entity);
            } catch (Exception ex) {

                return ApiResponse<T>.ErrorResponse($"An error occurred: {ex.Message}");

            }
        }

        public ApiResponse<T> Delete(int id) {
            try {
                var item = dbSet.Find(id);
                if (item == null) {

                    return ApiResponse<T>.ErrorResponse("Not found");
                }
                dbSet.Remove(item);
                return ApiResponse<T>.SuccessResponse("Point Successfuly Deleted", item);
            } catch (Exception ex) {

                return ApiResponse<T>.ErrorResponse($"An error occurred: {ex.Message}");
            }
        }

        public ApiResponse<List<T>> GetAll() {
            try {
                IQueryable<T> query = dbSet;

                if (dbSet.Count() == 0) {
                    return ApiResponse<List<T>>.ErrorResponse("List is Empty");
                }

                return ApiResponse<List<T>>.SuccessResponse("Points Successfuly retrieved", query.ToList());

            } catch (Exception ex) {
                return ApiResponse<List<T>>.ErrorResponse($"An error occurred: {ex.Message}");
            }
        }

        public ApiResponse<T> GetById(int id) {
            try {
                if (id <= 0) {
                    return ApiResponse<T>.ErrorResponse("Invalid Id");
                }
                var point = dbSet.Find(id);
                if (point == null) {
                    return ApiResponse<T>.ErrorResponse("Not found");
                }
                return ApiResponse<T>.SuccessResponse("Point Successfuly retrieved", point);
            } catch (Exception ex) {

                return ApiResponse<T>.ErrorResponse($"An error occurred: {ex.Message}");
            }
        }

    }
}
