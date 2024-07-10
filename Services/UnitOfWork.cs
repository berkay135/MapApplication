using MapApplication.Data;

namespace MapApplication.Services {
    public class UnitOfWork : IUnitOfWork {
        
        private AppDbContext _db;

        public IPointRepository Point { get; private set; }

        public UnitOfWork(AppDbContext db)
        {
            _db = db;
            Point = new PointRepository(_db);
        }

        public void Save() {
            _db.SaveChanges();
        }
    }
}
