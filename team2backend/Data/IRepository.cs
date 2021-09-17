using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace team2backend.Data
{
    public interface IRepository<T>
        where T : class, IEntity
    {
        List<T> GetAll();

        T Get(int id);

        void Add(T entity);

        void Update(int id, T entity);

        void Delete(int id);
    }
}
