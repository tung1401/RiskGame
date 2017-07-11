//  ==============================================================
//    Copyright (c) 2016 PranWorks Co., Ltd
//    This program is protected by copyright laws.
//    Unauthorized reproduction or distribution of this
//    program or any portion of it is Prohibited.
// ==============================================================
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
//using EntityFramework.BulkInsert.Extensions;

namespace RiskGame.Repository.Common
{
    public abstract class Repository<T> : IDisposable, IRepository<T> where T : class
    {
        protected DbContext Dbcontext;
        protected IDbSet<T> Dbset;

        protected Repository(DbContext context)
        {
            Dbcontext = context;
            Dbset = context.Set<T>();
        }

        public int Count(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate != null)
            {
                return Dbset.Count(predicate);
            }
            return Dbset.Count();
        }

        public IEnumerable<T> GetSpecifyAmount(int start, int length)
        {
            return Dbset.Skip(start).Take(length);
        }

        public void Dispose()
        {
            Dbcontext.Dispose();
        }

        public T Add(T entity)
        {            
            foreach (var entry in Dbcontext.ChangeTracker.Entries().Where(m => m.Entity != entity))
            {
                switch (entry.State)
                {                   
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    //case EntityState.Added:
                    //    entry.State = EntityState.Detached;
                    //    break;
                    // If the EntityState is the Deleted, reload the date from the database.   
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                    default: break;
                }
            }
            Dbset.AddOrUpdate(entity);
            Dbcontext.SaveChanges();
            return entity;
        }

        //public List<T> AddList(List<T> list)
        //{
        //    //foreach (var entry in Dbcontext.ChangeTracker.Entries())
        //    //{
        //    //    switch (entry.State)
        //    //    {
        //    //        case EntityState.Modified:
        //    //            entry.State = EntityState.Unchanged;
        //    //            break;
        //    //        //case EntityState.Added:
        //    //        //    entry.State = EntityState.Detached;
        //    //        //    break;
        //    //        // If the EntityState is the Deleted, reload the date from the database.   
        //    //        case EntityState.Deleted:
        //    //            entry.Reload();
        //    //            break;
        //    //        default: break;
        //    //    }
        //    //}

        //    using (var transactionScope = new TransactionScope())
        //    {
        //        if (Dbcontext != null)
        //        {
        //            Dbcontext.BulkInsert(list);
        //            Dbcontext.SaveChanges();
        //        }
        //        transactionScope.Complete();
        //        return list;
        //    }
        //}

        public T Update(T entity)
        {            
            foreach (var entry in Dbcontext.ChangeTracker.Entries().Where(m => m.Entity != entity))
            {
                switch (entry.State)
                {                  
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    //case EntityState.Added:
                    //    entry.State = EntityState.Detached;
                    //    break;
                    // If the EntityState is the Deleted, reload the date from the database.   
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                    default: break;
                }
            }
            Dbset.AddOrUpdate(entity);
            Dbcontext.SaveChanges();
            return entity;
        }

        public void Delete(T entity)
        {
            Dbset.Attach(entity);
            Dbset.Remove(entity);
            Dbcontext.SaveChanges();
        }

        public void Delete(Expression<Func<T, bool>> where)
        {
            var objects = Dbset.Where(where).AsEnumerable();
            foreach (var obj in objects)
                Dbset.Remove(obj);

            Dbcontext.SaveChanges();
        }

        T IRepository<T>.Find(int id)
        {
            return Dbset.Find(id);
        }

        public T Get(Expression<Func<T, bool>> where)
        {
            return Dbset.Where(where).FirstOrDefault();
        }

        public T GetWith(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes)
        {
            var query = Dbset.Where(where);
            foreach (var inc in includes)
            {
                query = query.Include(inc);
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetManyWith(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = Dbset;
            foreach (var inc in includes)
            {
                query = query.Include(inc);
            }
            return query.Where(where);
        }

        public IEnumerable<T> GetManyWithRequired(IEnumerable<Expression<Func<T, bool>>> @where,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = Dbset;

            foreach (var wherePredicate in where)
            {
                query = query.Where(wherePredicate);
            }

            foreach (var property in includes)
            {
                query = query.Include(property);
            }

            return query.ToList();
        }

        public IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return Dbset.Where(where).ToList();
        }

        public IEnumerable<T> GetAll()
        {
            return Dbset.ToList();
        }

        public IEnumerable<T> GetAllWith(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = Dbset;
            foreach (var includeProperty in includes)
            {
                query = query.Include(includeProperty);
            }
            return query;
            //how to used
            //var blog = _blogRepository.AllIncluding(blog => b.Posts, b=>b.Tags);
        }

        public IEnumerable<T> GetWithSize(int skip = 0, int take = 0, Expression<Func<T, bool>> where = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = Dbset;
            if (where != null)
            {
                query = query.Where(where);
            }
            if (includes.Any())
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }
            if (orderBy != null)
            {
                return orderBy(query).Skip(skip).Take(take).ToList();
            }
            else
            {
                if (skip > 0 && take > 0)
                {
                    return query.Skip(skip).Take(take).ToList();
                }
                return query.ToList();
            }
        }
        public IEnumerable<T> GetWithSize(int skip = 0, int take = 0, Expression<Func<T, bool>> where = null,
          Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            IQueryable<T> query = Dbset;
            if (where != null)
            {
                query = query.Where(where);
            }
            if (orderBy != null)
            {
                return orderBy(query).Skip(skip).Take(take).ToList();
            }
            else
            {
                if(take > 0)
                {
                    return query.Skip(skip).Take(take).ToList();
                }
                return query.ToList();
            }

        }

        public int GetAllWithCount(Expression<Func<T, bool>> where = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = Dbset;

            foreach (var inc in includes)
            {
                query = query.Include(inc);
            }

            if (where != null)
            {
                query = query.Where(where);
            }
            return query.Count();
        }

        public bool CheckAny(Expression<Func<T, bool>> where)
        {
            IQueryable<T> query = Dbset;
            return query.Any(where);
        }

        public T GetLastOrderBy(Expression<Func<T, bool>> where, Func<T, object> keySelector)
        {
            IQueryable<T> query = Dbset;
            return query.Where(where).OrderBy(keySelector).LastOrDefault();
        }

        public int SumInt(Expression<Func<T, bool>> where, Func<T, int> keySelector)
        {
            IQueryable<T> query = Dbset;
            return query.Where(where).Sum(keySelector);
        }

        public decimal SumDecimal(Expression<Func<T, bool>> where, Func<T, int> keySelector)
        {
            IQueryable<T> query = Dbset;
            return query.Where(where).Sum(keySelector);
        }

        public void LogAudit(T oldData, T newData, params string[] ignore)
        {
            if (oldData != null && newData != null)
            {
                Type type = typeof (T);
                List<string> ignoreList = new List<string>(ignore);
                foreach (
                    System.Reflection.PropertyInfo pi in
                        type.GetProperties(System.Reflection.BindingFlags.Public |
                                           System.Reflection.BindingFlags.Instance))
                {
                    if (!ignoreList.Contains(pi.Name))
                    {
                        object oldDataValue = type.GetProperty(pi.Name).GetValue(oldData, null);
                        object newDataValue = type.GetProperty(pi.Name).GetValue(newData, null);

                        if (oldDataValue != newDataValue && (oldDataValue == null || !oldDataValue.Equals(newDataValue)))
                        {
                            //TODO to save this difference object in LogAudit
                        }
                    }
                }
            }
        }

        public T GetWithCulture(Expression<Func<T, bool>> @where, params Expression<Func<T, object>>[] includes)
        {
            var query = Dbset.Where(where);
            foreach (var inc in includes)
            {
                query = query.Include(inc);
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetManyWithCulture(Expression<Func<T, bool>> @where,
            params Expression<Func<T, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public List<T> AddList(List<T> list)
        {
            throw new NotImplementedException();
        }
    }
}