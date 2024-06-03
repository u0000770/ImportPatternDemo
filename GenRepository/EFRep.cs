namespace GenRepository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Microsoft.EntityFrameworkCore;
    using SharpCompress.Common;
    using SqlItems;

    public class Repository<TEntity> : Repository.IRepository<TEntity> where TEntity : class
    {
        private readonly MyDbContext _context;
        private readonly DbSet<TEntity> _dbSet;


        public Repository(MyDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<TEntity>();
        }

        public void Clear()
        {
            var all = _dbSet.ToList();
            _dbSet.RemoveRange(all);
            SaveChanges();          
        }

        public void ClearAndAddRange(IEnumerable<TEntity> entities)
        {
            Clear();
            foreach (var entity in entities)
            {
                _dbSet.Add(entity);
            }
            SaveChanges();
        }

        public void Add(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public TEntity GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _dbSet.ToList();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate).ToList();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
