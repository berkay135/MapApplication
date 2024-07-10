namespace MapApplication.Services {
    public interface IUnitOfWork {
        IPointRepository Point { get; }

        public void Save();
    }
}
