using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KPI.Repositories
{
    public interface IRepo<T> where T : IModel
    {
        T Create(T item);
        T Update(T item);
        void Delete(string key);
        T Get(string key);
        IEnumerable<T> Get(Func<T, bool> filter);
    }
}
