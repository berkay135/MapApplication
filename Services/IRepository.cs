using MapApplication.Services.Responses;

namespace MapApplication.Services {
    public interface IRepository<T> where T : class {
        ApiResponse<List<T>> GetAll();
        ApiResponse<T> GetById(int id);
        ApiResponse<T> Delete(int id);
        ApiResponse<T> Add(T entity);
    }
}
