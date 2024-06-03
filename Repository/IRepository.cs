using System.Linq.Expressions;

namespace Repository
{
    /// <summary>
    /// The where TEntity : class constraint ensures that Item (or any other type used as TEntity) must be a class. 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> where TEntity : class
    {
        public void Clear();
        public void Add(TEntity entity);
        public void ClearAndAddRange(IEnumerable<TEntity> entities);
		public void Update(TEntity entity);
        public void Delete(TEntity entity);
        public TEntity GetById(int id);
        public IEnumerable<TEntity> GetAll();
        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        void SaveChanges();
    }



}
