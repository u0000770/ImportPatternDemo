using Domain;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GenRepository
{
    public class MongoRepository<T> : IRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(IMongoCollection<T> collection)
        {
            _collection = collection;
        }

        public void Clear()
        {
            var filter = Builders<T>.Filter.Empty;
            _collection.DeleteMany(filter);
        }

        public void Add(T entity)
        {
            // Check if the entity has an Id property
            var idProperty = typeof(T).GetProperty("Id");

            if (idProperty != null)
            {
                // Check if the Id property type is integer
                if (idProperty.PropertyType == typeof(int))
                {
                    // Generate a new GUID and convert it to an integer
                    var guid = Guid.NewGuid();
                    var guidBytes = guid.ToByteArray();
                    var integerId = BitConverter.ToInt32(guidBytes, 0);

                    // Set the integer value to the Id property
                    idProperty.SetValue(entity, integerId);
                }
                // If the Id property type is not integer, MongoDB will handle its generation
            }



            _collection.InsertOne(entity);
        }

        public IEnumerable<T> GetAll()
        {
            var entities = _collection.Find(FilterDefinition<T>.Empty).ToList();

            return entities;
        }

        public void ClearAndAddRange(IEnumerable<T> entities)
        {
			Clear();
			foreach (var entity in entities)
			{
				Add(entity);
			}
		}

        void IRepository<T>.SaveChanges()
        {
            return;
        }

        #region NOT IMPLEMENTED ( YET )
        void IRepository<T>.Delete(T entity)
        {
            throw new NotImplementedException();
        }

        IEnumerable<T> IRepository<T>.Find(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        T IRepository<T>.GetById(int id)
        {
            throw new NotImplementedException();
        }

        void IRepository<T>.Update(T entity)
        {
            throw new NotImplementedException();
        }



        #endregion


    }

}
